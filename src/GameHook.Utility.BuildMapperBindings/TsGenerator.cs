using System.Text;
using System.Xml.Linq;
using GameHook.Application;
using GameHook.Domain;

namespace GameHook.Utility.BuildMapperBindings;

public static class TsGenerator
{
    static string GetTypescriptInterface(string name)
    {
        return $"I" + name.First().ToString().ToUpper() + name.Substring(1, name.Length - 1);
    }

    static string GetTypescriptEnumName(string name)
    {
        return name.CapitalizeFirstLetter();
    }

    static string GetEnumKeyName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return "NONE";
        }

        var strippedString = new String(name
                                .Where(x => char.IsLetterOrDigit(x) || char.IsWhiteSpace(x))
                                .ToArray());

        if (strippedString.Length > 0 && char.IsNumber(strippedString[0]))
        {
            return string.Empty;
        }

        return strippedString.Replace(" ", "_").ToUpper();
    }

    static string GetTypescriptType(this XElement el)
    {
        switch (el.Name.LocalName)
        {
            case "property":
            {
                var referenceType = el.GetOptionalAttributeValue("reference");
                if (string.IsNullOrEmpty(referenceType) == false)
                {
                    var referenceNode = el?.Document?.Descendants("references")?.Descendants(referenceType)?.Single() ??
                                        throw new Exception($"Unable to determine reference type {referenceType}.");

                    var referenceNodeType = referenceNode.GetOptionalAttributeValue("type") ?? "string";
                    if (referenceNodeType == "string")
                    {
                        return "GameHookProperty<string>";
                    }
                    else if (referenceNodeType == "number")
                    {
                        return "GameHookProperty<number>";
                    }
                    else
                    {
                        throw new Exception(
                            $"Unable to translate reference node type of {referenceNodeType} for reference {referenceType}.");
                    }
                }

                var attributeType = el.GetAttributeValue("type");

                if (attributeType == "int") return "GameHookProperty<number>";
                else if (attributeType == "string") return "GameHookProperty<string>";
                else return $"GameHookProperty<string>";
                throw new Exception($"Invalid property type {attributeType}.");
            }
            case "class":
            {
                var attributeType = el.GetAttributeValue("type");
                return GetTypescriptInterface(attributeType);
            }
            default:
                throw new Exception($"Cannot get typescript type for element. ${el}");
        }
    }

    static string GetTypescriptTupleTypes(this XElement el)
    {
        if (el.IsArray() == false)
        {
            throw new Exception($"Cannot get typescript tuple types for element because it is not an array. ${el}");
        }

        var tupleTypes = el.Elements().Select(x => x.GetTypescriptType()).ToArray();
        return $"[{string.Join(',', tupleTypes)}]";
    }

    static void TransverseHierarchy(XElement el, StringBuilder result, int depth, string separator,
        string endingCharacter)
    {
        if (el.IsParentAnArray())
        {
            result.AppendLine("{");

            foreach (var childEl in el.Elements())
            {
                TransverseProperties(childEl, result, depth + 1);
            }

            result.AppendLine("},");
        }
        else
        {
            if (el.IsArray())
            {
                result.AppendLine($"{el.GetElementActualName()}{separator} [");

                foreach (var childEl in el.Elements())
                {
                    TransverseProperties(childEl, result, depth + 1);
                }

                result.AppendLine($"] as {GetTypescriptTupleTypes(el)}{endingCharacter}");
            }
            else
            {
                result.AppendLine($"{el.GetElementActualName()}{separator} {{");

                foreach (var childEl in el.Elements())
                {
                    TransverseProperties(childEl, result, depth + 1);
                }

                result.AppendLine($"}}{endingCharacter}");
            }
        }
    }

    static void TransverseProperties(XElement el, StringBuilder result, int depth)
    {
        var separator = depth == 0 ? "=" : ":";
        var endingCharacter = depth == 0 ? "" : ",";

        switch (el.Name.LocalName)
        {
            case "property":
                if (el.IsParentAnArray())
                {
                    result.AppendLine(
                        $"    new {el.GetTypescriptType()}(this, '{el.GetElementPath()}'){endingCharacter}");
                }
                else
                {
                    result.AppendLine(
                        $"    {el.GetElementActualName()}{separator} new {el.GetTypescriptType()}(this, '{el.GetElementPath()}'){endingCharacter}");
                }

                break;
            case "script":
                // result.AppendLine(el.Value);
                break;
            default:
                TransverseHierarchy(el, result, depth, separator, endingCharacter);
                break;
        }
    }

    static void IterateInterfaceElement(StringBuilder builder, XElement el)
    {
        if (el.Name.LocalName == "property")
        {
            builder.AppendLine($"{el.GetElementActualName()}: {el.GetTypescriptType()}");
        }
        else if (el.IsArray())
        {
            builder.AppendLine($"{el.GetElementActualName()}: {el.GetTypescriptTupleTypes()}");
        }
        else if (el.IsParentAnArray())
        {
            // Do nothing, as this is covered via tuple types directly above.
        }
        else if (el.Name.LocalName == "script")
        {
            //result.AppendLine(x.Value);
        }
        else
        {
            builder.AppendLine($"{el.GetElementActualName()}: {{");

            foreach (var childEl in el.Elements())
            {
                IterateInterfaceElement(builder, childEl);
            }

            builder.AppendLine("}");
        }
    }

    public static string FromMapper(XDocument doc)
    {
        var result = new StringBuilder();

        result.AppendLine("import { GameHookMapper, GameHookProperty } from \"../core.js\"");
        result.AppendLine(string.Empty);
        
        // References
        var references = GameHookMapperXmlFactory.GetGlossary(doc);
        foreach (var reference in references)
        {
            if (reference.Type == "number")
            {
                continue;
            }

            if (reference.Name.Contains("CharacterMap", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            result.AppendLine($"export enum {GetTypescriptEnumName(reference.Name)} {{");

            foreach (var x in reference.Values.DistinctBy(x => x.Value))
            {
                if (x.Value == null)
                {
                    continue;
                }

                var enumName = GetEnumKeyName(x.Value?.ToString() ?? string.Empty);

                if (string.IsNullOrEmpty(enumName) == false)
                {
                    result.AppendLine($"{enumName} = '{x.Value?.ToString()?.Replace("'", "\\'")}',");
                }
            }

            result.AppendLine($"}}");
        }
        
        result.AppendLine(string.Empty);

        // Interfaces
        foreach (var x in doc.Descendants("classes").Elements())
        {
            result.AppendLine($"export class {GetTypescriptInterface(x.Name.LocalName)} {{");

            foreach (var y in x.Elements())
            {
                IterateInterfaceElement(result, y);
            }

            result.AppendLine($"}}");
            result.AppendLine(string.Empty);
        }

        result.AppendLine(string.Empty);

        // Properties
        var meta = GameHookMapperXmlFactory.GetMetadata(doc);
        result.AppendLine($"export class {meta.UniqueIdentifier}MapperClient extends GameHookMapper {{");

        foreach (var el in doc.Descendants("properties").Elements())
        {
            TransverseProperties(el, result, 0);
        }

        result.AppendLine($"}}");

        return result.ToString();
    }
}