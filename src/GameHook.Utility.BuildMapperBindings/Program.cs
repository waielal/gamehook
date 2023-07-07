using System.Text;
using System.Xml.Linq;
using GameHook.Application;
using GameHook.Domain.Interfaces;
using GameHook.Utility.BuildMapperBindings;

// Load XML file paths.
var mapperInputDirectoryPath = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../../mappers");
var typescriptOutputDirectoryPath = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../../bindings/src");
var filePaths = Directory.GetFiles(mapperInputDirectoryPath, "*.xml", SearchOption.AllDirectories);

foreach (var xmlFilePath in filePaths)
{
    var contents = await File.ReadAllTextAsync(xmlFilePath);

    var doc = XDocument.Parse(contents);
    var mapper = GameHookMapperXmlFactory.LoadMapperFromFile(null, doc);

    // Create child directory if not exists.
    if (mapper.Metadata.GamePlatform.Any(x => char.IsLetter(x) == false && char.IsNumber(x) == false))
    {
        throw new Exception("Invalid characters in game platform.");
    }

    Directory.CreateDirectory(Path.Combine(typescriptOutputDirectoryPath, mapper.Metadata.GamePlatform));
    
    var tsFilePath = Path.Combine(typescriptOutputDirectoryPath, mapper.Metadata.GamePlatform, mapper.Metadata.UniqueIdentifier + ".ts");

    // Look for state manager, if it exists.
    var stateTsFilePath = tsFilePath.Replace(".ts", ".state.ts");
    string? stateTsRelativeFilePath = null;
    if (File.Exists(stateTsFilePath))
    {
        stateTsRelativeFilePath = $"./{Path.GetFileName(stateTsFilePath)}";
    }

    // Generate typescript bindings.
    var tsResult = TsGenerator.FromMapper(doc, stateTsRelativeFilePath);
    await File.WriteAllTextAsync(tsFilePath, tsResult);
}

Console.WriteLine("Done");