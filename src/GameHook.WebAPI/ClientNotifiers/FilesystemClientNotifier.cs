using GameHook.Domain.DTOs;
using GameHook.Domain.Interfaces;

namespace GameHook.WebAPI.ClientNotifiers
{
    public class FilesystemClientNotifier : IClientNotifier
    {
        private string FormatFilename(string filename)
        {
            if (filename.Contains("..") || filename.Contains(":") || filename.Contains("\\"))
            {
                throw new Exception($"Filename {filename} contains invalid characters.");
            }

            return filename.Replace(".", "_");
        }

        private string FormatProperty(string? format, object? value)
        {
            if (value == null) return string.Empty;

            return string.Format(format ?? "{0}", value).Trim();
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
                    var value = mapper.Properties.Single(x => x.Path == path).Value;

                    foreach (var item in propertyGroup)
                    {
                        var filename = FormatFilename(path);
                        var writeValue = FormatProperty(item.Format, value);

                        await File.WriteAllTextAsync(Path.Combine(BuildEnvironment.OutputPropertiesDirectory, $"{filename}.txt"), writeValue);
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
                    foreach (var item in mapperUserConfig.OutputPropertiesToFilesystem.Where(x => x.Path == property.Path))
                    {
                        var filename = FormatFilename(property.Path);
                        var writeValue = FormatProperty(item.Format, property.Value);

                        await File.WriteAllTextAsync(Path.Combine(BuildEnvironment.OutputPropertiesDirectory, $"{filename}.txt"), writeValue);
                    }
                }
            }
        }
    }
}
