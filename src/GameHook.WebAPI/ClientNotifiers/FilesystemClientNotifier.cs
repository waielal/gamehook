using GameHook.Domain;
using GameHook.Domain.DTOs;
using GameHook.Domain.Interfaces;

namespace GameHook.WebAPI.ClientNotifiers
{
    static class FilesystemHelper
    {
        public static string? GetUrlFileExtension(this string? format)
        {
            return Path.GetExtension(format);
        }

        public static string GetUrlFilename(this string originalFilename, string? format)
        {
            var originalFilenameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilename);
            var extension = GetUrlFileExtension(format);

            return $"{originalFilenameWithoutExtension}{extension}";
        }

        public static string FormatFilename(this string path, int index = 0)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception($"Filename output filename is NULL.");
            }

            if (path.Contains("..") || path.Contains(":") || path.Contains("\\"))
            {
                throw new Exception($"File name {path} contains invalid characters.");
            }

            if (index > 0)
            {
                path += $"_{index}";
            }

            return path.Replace(".", "_");
        }
    }

    public class FilesystemClientNotifier : IClientNotifier
    {
        private readonly ILogger<FilesystemClientNotifier> _logger;
        private readonly HttpClient _httpClient;
        private readonly GameHookConfiguration _gameHookConfiguration;

        private string OutputPropertiesDirectory => Path.Combine(BuildEnvironment.ConfigurationDirectory, "OutputProperties");


        private readonly byte[] PlaceholderImageBytes = new byte[] {
            137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 32, 0, 0, 0, 32, 8, 2, 0, 0,
            0, 252, 24, 237, 163, 0, 0, 0, 1, 115, 82, 71, 66, 0, 174, 206, 28, 233, 0, 0, 0, 4, 103, 65, 77,
            65, 0, 0, 177, 143, 11, 252, 97, 5, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 14, 195, 0, 0, 14, 195, 1,
            199, 111, 168, 100, 0, 0, 0, 44, 73, 68, 65, 84, 72, 75, 237, 205, 49, 1, 0, 48, 12, 4, 161, 250,
            55, 253, 149, 192, 148, 237, 48, 192, 219, 177, 2, 42, 160, 2, 42, 160, 2, 42, 160, 2, 42, 160, 2,
            42, 160, 227, 96, 251, 139, 85, 244, 166, 48, 247, 218, 125, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66,
            96, 130 };

        public FilesystemClientNotifier(ILogger<FilesystemClientNotifier> logger, IHttpClientFactory httpClientFactory, GameHookConfiguration gameHookConfiguration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _gameHookConfiguration = gameHookConfiguration;
        }

        private async Task WriteUrlContents(string path, string url, string filename)
        {
            try
            {
                var bytes = await _httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(Path.Combine(OutputPropertiesDirectory, filename), bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to download url '{url}' for path {path}.");
            }
        }

        private async Task OutputPropertyToFilesystem(IGameHookProperty property, IEnumerable<OutputPropertyOverrideItem> items)
        {
            bool outputDefaultFilenameAlready = false;

            var defaultFilename = property.Path.FormatFilename(0) + ".txt";
            var defaultValue = property.Value;

            // Convert special types.
            if (property.Value != null && (property.Type == "bit" || property.Type == "bool"))
            {
                defaultValue = (bool)property.Value == true ? 1 : 0;
            }

            var i = 0;
            foreach (var item in items)
            {
                var writeFilename = property.Path.FormatFilename(i) + ".txt";
                var writeValue = string.Empty;

                // Convert special types (required for formatting).
                if (property.Value != null && (property.Type == "int" || property.Type == "uint"))
                {
                    defaultValue = int.Parse(defaultValue?.ToString() ?? "0");
                }

                if (defaultValue != null)
                {
                    writeValue = string.Format(item.Format ?? "{0}", defaultValue);
                }

                // This is a file we need to download
                // from a remote source.
                if (item?.Format?.StartsWith("https://") == true)
                {
                    if (item.Format?.GetUrlFileExtension() == null)
                    {
                        _logger.LogWarning($"Could not determine the file extension for URL {item.Format}.");
                    }
                    else
                    {
                        writeFilename = writeFilename.GetUrlFilename(item.Format);

                        if (property.Value == null)
                        {
                            await File.WriteAllBytesAsync(Path.Combine(OutputPropertiesDirectory, writeFilename), PlaceholderImageBytes);
                        }
                        else
                        {
                            await WriteUrlContents(item.Path, writeValue, writeFilename);
                        }
                    }
                }
                else
                {
                    await File.WriteAllTextAsync(Path.Combine(OutputPropertiesDirectory, writeFilename), writeValue);
                }

                if (writeFilename == defaultFilename)
                {
                    outputDefaultFilenameAlready = true;
                }

                i++;
            }

            if (outputDefaultFilenameAlready == false)
            {
                await File.WriteAllTextAsync(Path.Combine(OutputPropertiesDirectory, defaultFilename), defaultValue?.ToString());
            }
        }

        public Task SendGameHookError(ProblemDetailsForClientDTO _) => Task.CompletedTask;

        public async Task SendInstanceReset()
        {
            Directory.CreateDirectory(BuildEnvironment.MapperUserSettingsDirectory);

            if (Directory.Exists(OutputPropertiesDirectory))
            {
                Directory.Delete(OutputPropertiesDirectory, true);
            }

            if (_gameHookConfiguration.OutputAllPropertiesToFilesystem)
            {
                Directory.CreateDirectory(OutputPropertiesDirectory);
            }

            await Task.CompletedTask;
        }

        public async Task SendMapperLoaded(IGameHookMapper mapper)
        {
            if (_gameHookConfiguration.OutputAllPropertiesToFilesystem)
            {
                Directory.CreateDirectory(OutputPropertiesDirectory);

                foreach (var property in mapper.Properties)
                {
                    var overrideItems = mapper.UserSettings?.OutputPropertyOverrides.Where(x => x.Path == property.Path)
                        ?? new List<OutputPropertyOverrideItem>();

                    await OutputPropertyToFilesystem(property, overrideItems);
                }
            }
        }

        public Task SendDriverError(ProblemDetailsForClientDTO _) => Task.CompletedTask;

        public async Task SendPropertyChanged(IGameHookProperty property, string[] fieldsChanged, MapperUserSettingsDTO? mapperUserConfig)
        {
            if (_gameHookConfiguration.OutputAllPropertiesToFilesystem)
            {
                if (fieldsChanged.Contains("value"))
                {
                    var overrideItems = mapperUserConfig?.OutputPropertyOverrides.Where(x => x.Path == property.Path)
                        ?? new List<OutputPropertyOverrideItem>();

                    await OutputPropertyToFilesystem(property, overrideItems);
                }
            }
        }

        public Task SendUiConfigurationChanged(Guid id) => Task.CompletedTask;
    }
}
