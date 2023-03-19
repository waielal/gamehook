using GameHook.Application;
using GameHook.Domain.Infrastructure;
using GameHook.Domain.Interfaces;
using GameHook.GenerateBindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text;

var baseSrcPath = @"D:\Repos\gamehook-io\mappers-js\src";
var mapperClasses = new List<string>();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IMapperFilesystemProvider, MapperFilesystemProvider>();
    })
    .Build();

var fakeInstance = new FakeGameHookInstance();
var filesystemProvider = host.Services.GetRequiredService<IMapperFilesystemProvider>();

foreach (var mapperDto in filesystemProvider.MapperFiles)
{
    var mapper = GameHookMapperFactory.ReadMapper(fakeInstance, filesystemProvider, mapperDto.Id);

    Console.WriteLine($"Creating mapper of {mapper.Metadata.GameName}.");

    var propertyKeys = mapper.Properties
        .OrderBy(x => x.Path)
        .Select(x => x.Path)
        .Distinct()
        .ToArray();

    Dictionary<string, object> propertyTree = new Dictionary<string, object>();
    foreach (string propertyKey in propertyKeys)
    {
        string[] keyParts = propertyKey.Split('.');
        string propertyName = keyParts.Last();
        Dictionary<string, object> currentTree = propertyTree;

        for (int i = 0; i < keyParts.Length - 1; i++)
        {
            string keyPart = keyParts[i];
            if (!currentTree.ContainsKey(keyPart))
            {
                currentTree[keyPart] = new Dictionary<string, object>();
            }
            currentTree = (Dictionary<string, object>)currentTree[keyPart];
        }

        currentTree[propertyName] = null;
    }

    string GetMapperClassName(IGameHookMapper mapper)
    {
        return $"{mapper.Metadata.GamePlatform.ToUpper()}_{new string(mapper.Metadata.GameName.Where(char.IsLetterOrDigit).ToArray())}_Mapper";
    }

    StringBuilder classBuilder = new StringBuilder();
    classBuilder.AppendLine("import { GameHookMapper, GameHookProperty } from '../base'");
    classBuilder.AppendLine(string.Empty);
    classBuilder.AppendLine($"export default class {GetMapperClassName(mapper)} extends GameHookMapper {{");

    void AppendProperty(StringBuilder builder, string rootPath, string propertyKey, int depth)
    {
        string indent = new string(' ', (depth - 1) * 4);

        var rootPrefix = string.IsNullOrEmpty(rootPath) ? string.Empty : $"{rootPath}.";
        var property = mapper.Properties.Single(x => x.Path == $"{rootPrefix}{propertyKey}");

        string propertyName = property.Path.Split('.').Last().Replace("-", "");

        var propertyType = string.Empty;

        if (string.IsNullOrEmpty(property.MapperVariables.Reference) == false)
        {
            // TODO: Create a new enum type.
            propertyType = "GameHookProperty<string>";
        }
        else if (property.Type == "binaryCodedDecimal")
        {
            propertyType = "GameHookProperty<number>";
        }
        else if (property.Type == "bitArray")
        {
            propertyType = "GameHookProperty<Array<boolean>>";
        }
        else if (property.Type == "bit")
        {
            propertyType = "GameHookProperty<boolean>";
        }
        else if (property.Type == "bool")
        {
            propertyType = "GameHookProperty<boolean>";
        }
        else if (property.Type == "int")
        {
            propertyType = "GameHookProperty<number>";
        }
        else if (property.Type == "string")
        {
            propertyType = "GameHookProperty<string>";
        }
        else if (property.Type == "uint")
        {
            propertyType = "GameHookProperty<number>";
        }
        else
        {
            throw new Exception($"Unknown property type of {propertyType}.");
        }

        if (propertyName.All(char.IsDigit))
        {
            // We are inside of an array -- probably.
            builder.AppendLine($"{indent}    {propertyName}: new {propertyType}(this, '{property.Path}'),");
        }
        else
        {
            if (depth == 1)
            {
                builder.AppendLine($"{indent}    {propertyName} = new {propertyType}(this, '{property.Path}')");
            }
            else
            {
                builder.AppendLine($"{indent}    {propertyName}: new {propertyType}(this, '{property.Path}'),");
            }
        }
    }

    string BuildRootPath(string root, string key)
    {
        if (string.IsNullOrEmpty(root)) return key;
        return $"{root}.{key}";
    }

    void PropertiesLoop(StringBuilder builder, string rootPath, Dictionary<string, object> propertyTree, int depth)
    {
        foreach (KeyValuePair<string, object> property in propertyTree)
        {
            var indent = new string(' ', depth * 4);
            var startString = $"{indent}{property.Key}: {{";
            var endString = $"{indent}}},";

            if (depth == 1)
            {
                // Start of the class object, so use normal typescript objects.
                startString = $"{indent}{property.Key} = {{";
                endString = $"{indent}}}";
            }

            if (property.Value == null)
            {
                AppendProperty(builder, rootPath, property.Key, depth);
            }
            else
            {
                builder.AppendLine(startString);

                var propertyValues = property.Value as Dictionary<string, object>;

                if (propertyValues != null)
                {
                    PropertiesLoop(builder, BuildRootPath(rootPath, property.Key), propertyValues, depth + 1);
                }
                else
                {
                    // We got to an empty object.
                    // Do nothing and continue.
                }

                builder.AppendLine(endString);
            }
        }
    }

    PropertiesLoop(classBuilder, string.Empty, propertyTree, 1);

    classBuilder.AppendLine("}");

    string classCode = classBuilder.ToString();

    File.WriteAllText($@"{baseSrcPath}\mappers\{GetMapperClassName(mapper)}.ts", classCode);
    mapperClasses.Add(GetMapperClassName(mapper));
}

Console.WriteLine("Writing gamehook.ts file.");
var gameHookTs = new List<string>() { };

foreach (var x in mapperClasses)
{
    gameHookTs.Add(@$"import {x} from ""./mappers/{x}""");
    gameHookTs.Add(@$"export {{ {x} }}");
}

File.WriteAllLines($@"{baseSrcPath}\gamehook.ts", gameHookTs);

Console.WriteLine("Done.");