using System.Text;
using System.Xml.Linq;
using GameHook.Application;
using GameHook.Domain.Interfaces;
using GameHook.Utility.BuildMapperBindings;

// Load XML file paths.
var mapperInputDirectoryPath = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../../mappers");
var typescriptOutputDirectoryPath = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../../bindings/src");
var filePaths = Directory.GetFiles(mapperInputDirectoryPath, "*.xml", SearchOption.AllDirectories);

var mappers = new List<IGameHookMapper>();
foreach (var xmlFilePath in filePaths)
{
    var contents = await File.ReadAllTextAsync(xmlFilePath);
    var doc = XDocument.Parse(contents);
    var mapper = GameHookMapperXmlFactory.LoadMapperFromFile(null, doc);

    // Generate typescript bindings.
    var tsResult = TsGenerator.FromMapper(doc);
    await File.WriteAllTextAsync(Path.Combine(typescriptOutputDirectoryPath, "mappers/", mapper.Metadata.UniqueIdentifier + ".ts"), tsResult);
    
    mappers.Add(mapper);
}

var imports = new StringBuilder();
foreach (var mapper in mappers)
{
    imports.AppendLine($"import {{ {mapper.Metadata.UniqueIdentifier} }} from './mappers/{mapper.Metadata.UniqueIdentifier}.js'");
}

imports.AppendLine(string.Empty);
imports.AppendLine($"export {{ {string.Join(',', mappers.Select(x => x.Metadata.UniqueIdentifier))} }}");

await File.WriteAllTextAsync(Path.Combine(typescriptOutputDirectoryPath, "gamehook.ts"), imports.ToString());

Console.WriteLine("Done");