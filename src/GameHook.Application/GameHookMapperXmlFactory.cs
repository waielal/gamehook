using GameHook.Domain;
using GameHook.Domain.Interfaces;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace GameHook.Application
{
    static class GameHookMapperXmlHelpers
    {
        public static ulong ToULong(this string value)
        {
            try
            {
                if (value.StartsWith("0x"))
                {
                    return Convert.ToUInt64(value, 16);
                }

                return Convert.ToUInt64(value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to translate {value} into a ulong.", ex);
            }
        }

        public static string NormalizeMemoryAddresses(this string value)
        {
            if (value.Contains("0x"))
            {
                // Use a regular expression to match and replace hexadecimal strings with decimal equivalents.
                string pattern = @"0x[a-fA-F\d]+";
                return Regex.Replace(value, pattern, match =>
                {
                    try
                    {
                        return Convert.ToInt32(match.Value, 16).ToString();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Unable to translate {match.Value} into a MemoryAddress.", ex);
                    }
                });
            }

            return value;
        }

        public static string TransformParametersOfFunction(this string input)
        {
            // Regular expression pattern to match text between parentheses
            string pattern = @"\(([^)]+)\)";

            // Match the text between parentheses
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                string parameterText = match.Groups[1].Value; // Get the text between parentheses

                // Split the parameter text by commas
                string[] parameters = parameterText.Split(',');

                // Apply the transform function on each parameter and collect the results
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = parameters[i].NormalizeMemoryAddresses().PerformBasicMath<ulong>().ToString();
                }

                // Join the transformed parameters with commas and replace the original parameter text
                string transformedParameters = string.Join(", ", parameters);
                return input.Replace(parameterText, transformedParameters);
            }

            // No parameters found, return the original input
            return input;
        }

        public static T PerformBasicMath<T>(this string input)
        {
            try
            {
                if (input.Any(c => MathOperators.Contains(c)) == false)
                {
                    return (T)Convert.ChangeType(input, typeof(T));
                }

                object computedResult = new DataTable().Compute(input, null);
                return (T)Convert.ChangeType(computedResult, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot perform basic math on input {input}.", ex);
            }
        }

        public static IEnumerable<XAttribute> GetAttributesWithVars(this XDocument doc)
        {
            var properties = doc.Descendants("properties") ?? throw new Exception("Unable to locate <properties>");

            return properties
                .Descendants()
                .Attributes()
                .Where(x => x.Name.NamespaceName == "https://schemas.gamehook.io/attributes/var");
        }

        public static string[] AttributesThatCanBeMathed { get; } = new string[] { "address", "preprocessor" };
        public static char[] MathOperators { get; } = new char[] { '+', '-', '*', '/', '%' };
        public static List<XAttribute> GetAttributesThatCanBeMathed(this XDocument doc)
        {
            var properties = doc.Descendants("properties") ?? throw new Exception("Unable to locate <properties>");

            return properties
                .Descendants()
                .Attributes()
                .Where(x => AttributesThatCanBeMathed.Contains(x.Name.LocalName))
                .Where(x => x.Value.Any(c => MathOperators.Contains(c)))
                .ToList();
        }

        public static string[] AttributesThatCanBeNormalized { get; } = new string[] { "address", "preprocessor" };
        public static List<XAttribute> GetAttributesThatCanBeNormalized(this XDocument doc)
        {
            var properties = doc.Descendants("properties") ?? throw new Exception("Unable to locate <properties>");

            return properties
                .Descendants()
                .Attributes()
                .Where(x => AttributesThatCanBeNormalized.Contains(x.Name.LocalName))
                .Where(x => x.Value.Contains("0x"))
                .ToList();
        }
    }

    public static class GameHookMapperXmlFactory
    {
        public static MapperMetadata GetMetadata(XDocument doc)
        {
            var root = doc.Element("mapper");
            if (root == null) throw new Exception($"Unable to find mapper root.");

            return new MapperMetadata()
            {
                Id = Guid.Parse(root.GetAttributeValue("id")),
                GameName = root.GetAttributeValue("name"),
                GamePlatform = root.GetAttributeValue("platform")
            };
        }

        static IEnumerable<IGameHookProperty> GetProperties(XDocument doc, IGameHookInstance? instance)
        {
            return doc.Descendants("properties").Descendants("property")
                .Select(x =>
                {
                    try
                    {
                        var address = x.GetOptionalAttributeValue("address");

                        return new GameHookProperty(instance, new GameHookMapperVariables()
                        {
                            Path = x.GetElementPath(),
                            Type = x.GetAttributeValue("type"),
                            Address = string.IsNullOrEmpty(address) ? null : address.ToMemoryAddress(),
                            Length = x.GetOptionalAttributeValueAsInt("length") ?? 1,
                            Position = x.GetOptionalAttributeValueAsInt("position"),
                            Reference = x.GetOptionalAttributeValue("reference"),
                            Description = x.GetOptionalAttributeValue("description"),
                            Expression = x.GetOptionalAttributeValue("expression"),
                            Preprocessor = x.GetOptionalAttributeValue("preprocessor"),
                            PostprocessorReader = x.GetOptionalAttributeValue("postprocessor-reader"),
                            PostprocessorWriter = x.GetOptionalAttributeValue("postprocessor-writer"),
                            StaticValue = x.GetOptionalAttributeValue("value")
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Unable to parse {x.GetElementPath()}. {x}", innerException: ex);
                    }
                })
                .ToArray();
        }

        public static IEnumerable<GlossaryList> GetGlossary(XDocument doc)
        {
            return doc.Descendants("references")
                .Elements()
                .Select(el =>
                {
                    var name = el.Name.LocalName;
                    var type = el.GetOptionalAttributeValue("type") ?? "string";

                    return new GlossaryList()
                    {
                        Name = name,
                        Type = type,
                        Values = el.Elements().Select(y =>
                        {
                            var key = y.GetAttributeValue("key").ToULong();
                            object? value = null;

                            var valueStr = y.GetOptionalAttributeValue("value");
                            if (string.IsNullOrEmpty(valueStr)) { value = null; }
                            else if (type == "string") { value = valueStr; }
                            else if (type == "number") { value = int.Parse(valueStr); }
                            else throw new Exception($"Unknown type for reference list {type}.");

                            return new GlossaryListItem()
                            {
                                Key = key,
                                Value = value
                            };

                        })
                    };
                });
        }

        public static GameHookMapper LoadMapperFromFile(IGameHookInstance? instance, XDocument doc)
        {
            // Apply Macros
            var destinationMacros = doc.Descendants("macro").ToArray();
            foreach (var destinationMacro in destinationMacros)
            {
                var macroType = destinationMacro.GetAttributeValue("type");
                var sourceMacro = doc.Descendants("macros").Descendants(macroType).FirstOrDefault() ??
                                  throw new XmlException($"Unable to find macro in <macros> tag of {macroType}.");

                destinationMacro.ReplaceWith(sourceMacro.Elements());
            }

            // Apply Classes.
            var destinationClasses = doc.Descendants("properties").Descendants("class");
            foreach (var destinationClass in destinationClasses)
            {
                var classType = destinationClass.GetAttributeValue("type");
                var sourceClass = doc.Descendants("classes").Descendants(classType).FirstOrDefault() ??
                                  throw new XmlException($"Unable to find class in <classes> tag of {classType}.");

                destinationClass.ReplaceNodes(sourceClass.Elements());
            }

            // Apply variable replacement.
            var attributesWithVars = doc.GetAttributesWithVars();
            foreach (var attr in attributesWithVars)
            {
                if (attr.Parent == null) throw new Exception($"Cannot get parent from attribute {attr}.");

                var varName = attr.Name.LocalName;
                var varValue = attr.Value;

                foreach (var attribute in attr.Parent.Descendants().Attributes())
                {
                    attribute.Value = attribute.Value.Replace(varName, varValue);
                }
            }

            // Apply normalization of hexdecimals.
            foreach (var attr in doc.GetAttributesThatCanBeNormalized())
            {
                attr.Value = attr.Value.NormalizeMemoryAddresses();
            }

            // Apply basic math.
            foreach (var attr in doc.GetAttributesThatCanBeMathed())
            {
                if (attr.Value.Contains('(') || attr.Value.Contains(')'))
                {
                    attr.Value = attr.Value.TransformParametersOfFunction();
                }
                else
                {
                    attr.Value = attr.Value.PerformBasicMath<ulong>().ToString();
                }
            }

            if (instance?.Configuration.OutputTransformedMapper ?? false)
            {
                File.WriteAllText(BuildEnvironment.TransformedMapperFilePath, doc.ToString());
            }

            return new GameHookMapper(GetMetadata(doc), GetProperties(doc, instance), GetGlossary(doc));
        }
    }
}