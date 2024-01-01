using GameHook.Domain.DTOs;
using GameHook.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GameHook.Domain.Infrastructure
{
    public class MapperFilesystemProvider : IMapperFilesystemProvider
    {
        private ILogger<MapperFilesystemProvider> Logger { get; }

        public string MapperFolder { get; }
        public string? BinaryMapperFolder { get; }

        public IEnumerable<MapperFilesystemDTO> MapperFiles { get; private set; } = new List<MapperFilesystemDTO>();

        public MapperFilesystemProvider(ILogger<MapperFilesystemProvider> logger, IConfiguration configuration)
        {
            Logger = logger;

            MapperFolder = Path.Combine(BuildEnvironment.ConfigurationDirectory, "Mappers");
            BinaryMapperFolder = GetCustomMappersFolder();

            if (BuildEnvironment.IsDebug)
            {
                var alternativeMapperDirectory = configuration["ALTERNATIVE_MAPPER_DIRECTORY"];

                if (string.IsNullOrEmpty(alternativeMapperDirectory) == false)
                {
                    MapperFolder = alternativeMapperDirectory;
                }
            }

            RefreshMapperFiles();
        }

        public void RefreshMapperFiles()
        {
            MapperFiles = GetAllMapperFiles();
        }

        private string? GetCustomMappersFolder()
        {
            var processPath = Path.GetDirectoryName(Environment.ProcessPath);
            if (processPath == null)
            {
                Logger.LogWarning("Unable to determine the process path for executable. Cannot determine the custom mapper folder.");
                return null;
            }

            var folder = Path.Combine(processPath, "Mappers");
            if (Directory.Exists(folder) == false)
            {
                return null;
            }

            return folder;
        }

        private string GetId(MapperFilesystemTypes type, string filePath)
        {
            if (filePath.Contains(".."))
            {
                throw new Exception("Invalid characters in file path.");
            }

            var pathParts = filePath.Split(Path.DirectorySeparatorChar);

            var directory = pathParts.Length > 1 ? pathParts[pathParts.Length - 2] : "";
            var filenameWithExtension = pathParts[pathParts.Length - 1];

            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(filenameWithExtension);

            var formattedFilename = filenameWithoutExtension.Replace(' ', '_');

            return $"{type}_{directory}_{formattedFilename}".ToLower();
        }

        private string GetDisplayName(string filePath)
        {
            if (filePath.Contains(".."))
            {
                throw new Exception("Invalid characters in file path.");
            }

            var pathParts = filePath.Split(Path.DirectorySeparatorChar);

            var directory = pathParts.Length > 1 ? pathParts[pathParts.Length - 2] : "";
            var filenameWithExtension = pathParts[pathParts.Length - 1];

            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(filenameWithExtension);

            var formattedFilename = filenameWithoutExtension.Replace('_', ' ');

            return $"({directory.ToUpper()}) {formattedFilename}";
        }

        /// <summary>
        /// We replace the base path with an empty string
        /// as to not expose the absolute path of the filesystem.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<MapperFilesystemDTO> GetAllMapperFiles()
        {
            if (MapperFolder.Contains(".."))
            {
                throw new Exception("Invalid characters in mapper folder path.");
            }

            var mappers = new DirectoryInfo(MapperFolder)
                .GetFiles("*.xml", SearchOption.AllDirectories)
                .Select(x => new MapperFilesystemDTO()
                {
                    Id = GetId(MapperFilesystemTypes.Official, x.FullName),
                    Type = MapperFilesystemTypes.Official,
                    AbsolutePath = x.FullName,
                    DisplayName = $"{GetDisplayName(x.FullName)}"
                })
                .ToList();

            if (BinaryMapperFolder != null)
            {
                if (BinaryMapperFolder.Contains('.') || BinaryMapperFolder.Contains(".."))
                {
                    throw new Exception("Invalid characters in mapper folder path.");
                }

                var localMappers = new DirectoryInfo(BinaryMapperFolder)
                    .GetFiles("*.xml", SearchOption.AllDirectories)
                    .Select(x => new MapperFilesystemDTO()
                    {
                        Id = GetId(MapperFilesystemTypes.Local, x.FullName),
                        Type = MapperFilesystemTypes.Local,
                        AbsolutePath = x.FullName,
                        DisplayName = $"(Local) {GetDisplayName(x.FullName)}"
                    })
                    .ToList();

                mappers.AddRange(localMappers);
            }

            return mappers;
        }
    }
}
