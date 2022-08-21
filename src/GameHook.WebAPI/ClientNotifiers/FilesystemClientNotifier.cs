using GameHook.Domain.DTOs;
using GameHook.Domain.Infrastructure;
using GameHook.Domain.Interfaces;

namespace GameHook.WebAPI.ClientNotifiers
{
    static class FilesystemHelper
    {
        public static string AddFileSuffix(this string filename, string suffix)
        {
            string fDir = Path.GetDirectoryName(filename) ?? string.Empty;
            string fName = Path.GetFileNameWithoutExtension(filename);
            string fExt = Path.GetExtension(filename);
            return Path.Combine(fDir, string.Concat(fName, suffix, fExt));
        }

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

        public static string FormatFilename(this string filename, int index)
        {
            if (filename.Contains("..") || filename.Contains(":") || filename.Contains("\\"))
            {
                throw new Exception($"Filename {filename} contains invalid characters.");
            }

            if (index > 0)
            {
                filename = filename.AddFileSuffix($"_{index}");
            }

            return filename.Replace(".", "_");
        }

        public static string FormatProperty(this string? format, object? value)
        {
            if (value == null) return string.Empty;

            return string.Format(new CustomStringFormat(), format ?? "{0}", value);
        }
    }

    public class FilesystemClientNotifier : IClientNotifier
    {
        private readonly ILogger<FilesystemClientNotifier> _logger;
        private readonly HttpClient _httpClient;

        private readonly byte[] PlaceholderImageBytes = new byte[] {
            137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 32, 0, 0, 0, 32, 8, 2, 0, 0,
            0, 252, 24, 237, 163, 0, 0, 0, 1, 115, 82, 71, 66, 0, 174, 206, 28, 233, 0, 0, 0, 4, 103, 65, 77,
            65, 0, 0, 177, 143, 11, 252, 97, 5, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 14, 195, 0, 0, 14, 195, 1,
            199, 111, 168, 100, 0, 0, 0, 44, 73, 68, 65, 84, 72, 75, 237, 205, 49, 1, 0, 48, 12, 4, 161, 250,
            55, 253, 149, 192, 148, 237, 48, 192, 219, 177, 2, 42, 160, 2, 42, 160, 2, 42, 160, 2, 42, 160, 2,
            42, 160, 227, 96, 251, 139, 85, 244, 166, 48, 247, 218, 125, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66,
            96, 130 };

        public FilesystemClientNotifier(ILogger<FilesystemClientNotifier> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        private async Task WriteUrlContents(string path, string url, string filename)
        {
            try
            {
                var bytes = await _httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(Path.Combine(BuildEnvironment.OutputPropertiesDirectory, filename), bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to download url '{url}' for path {path}.");
            }
        }

        private async Task OutputPropertyToFilesystem(IGameHookProperty property, OutputPropertyToFilesystemItem item, int index)
        {
            var filename = property.Path.FormatFilename(index);
            var writeValue = item.Format.FormatProperty(property.Value);

            // This is a file we need to download
            // from a remote source.
            if (item.Format?.StartsWith("https://") ?? false)
            {
                if (item.Format?.GetUrlFileExtension() == null)
                {
                    _logger.LogWarning($"Could not determine the file extension for URL {item.Format}.");
                }
                else
                {
                    var urlFilename = filename.GetUrlFilename(item.Format);

                    if (property.Value == null)
                    {
                        await File.WriteAllBytesAsync(Path.Combine(BuildEnvironment.OutputPropertiesDirectory, urlFilename), PlaceholderImageBytes);
                    }
                    else
                    {
                        await WriteUrlContents(item.Path, writeValue, urlFilename);
                    }
                }
            }
            else
            {
                await File.WriteAllTextAsync(Path.Combine(BuildEnvironment.OutputPropertiesDirectory, $"{filename}.txt"), writeValue);
            }
        }

        public Task SendGameHookError(ProblemDetailsForClientDTO _) => Task.CompletedTask;

        public async Task SendInstanceReset()
        {
            Directory.CreateDirectory(BuildEnvironment.MapperUserSettingsDirectory);

            if (Directory.Exists(BuildEnvironment.OutputPropertiesDirectory))
            {
                Directory.Delete(BuildEnvironment.OutputPropertiesDirectory, true);
            }

            await Task.CompletedTask;
        }

        public async Task SendMapperLoaded(IGameHookMapper mapper)
        {
            if (mapper.UserSettings?.OutputPropertiesToFilesystem != null)
            {
                Directory.CreateDirectory(BuildEnvironment.OutputPropertiesDirectory);

                foreach (var propertyGroup in mapper.UserSettings.OutputPropertiesToFilesystem.GroupBy(x => x.Path))
                {
                    var path = propertyGroup.Key;

                    if (string.IsNullOrEmpty(path)) { continue; }

                    if (mapper.Properties.Any(x => x.Path == path) == false)
                    {
                        _logger.LogWarning($"Cannot find path in mapper {path}.");

                        continue;
                    }

                    var property = mapper.Properties.Single(x => x.Path == path);

                    int i = 0;
                    foreach (var item in propertyGroup)
                    {
                        await OutputPropertyToFilesystem(property, item, i);

                        i++;
                    }
                }
            }
        }

        public Task SendDriverError(ProblemDetailsForClientDTO _) => Task.CompletedTask;

        public async Task SendPropertyChanged(IGameHookProperty property, string[] fieldsChanged, MapperUserSettingsDTO? mapperUserConfig)
        {
            if (mapperUserConfig?.OutputPropertiesToFilesystem != null)
            {
                if (mapperUserConfig.OutputPropertiesToFilesystem.Any(x => x.Path == property.Path) && fieldsChanged.Contains("value"))
                {
                    int i = 0;
                    foreach (var item in mapperUserConfig.OutputPropertiesToFilesystem.Where(x => x.Path == property.Path))
                    {
                        await OutputPropertyToFilesystem(property, item, i);

                        i++;
                    }
                }
            }
        }
    }
}
