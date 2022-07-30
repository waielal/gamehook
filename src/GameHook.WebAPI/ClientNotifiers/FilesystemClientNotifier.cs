using GameHook.Application;
using GameHook.Domain.DTOs;
using GameHook.Domain.Interfaces;

namespace GameHook.WebAPI.ClientNotifiers
{
    public class FilesystemClientNotifier : IClientNotifier
    {
        public Task SendGameHookError(ProblemDetailsForClientDTO _) => Task.CompletedTask;

        public async Task SendInstanceReset()
        {
            if (Directory.Exists(BuildEnvironment.OutputPropertiesDirectory))
            {
                Directory.Delete(BuildEnvironment.OutputPropertiesDirectory, true);
            }

            Directory.CreateDirectory(BuildEnvironment.OutputPropertiesDirectory);

            await Task.CompletedTask;
        }

        public async Task SendMapperLoaded(IGameHookMapper mapper)
        {
            foreach (var property in mapper.Properties)
            {
                var key = property.Path;
                var value = property.Value;

                await File.WriteAllTextAsync(Path.Combine(BuildEnvironment.OutputPropertiesDirectory, $"{key}.txt"), value?.ToString());
            }
        }

        public Task SendMapperLoadError() => Task.CompletedTask;

        public Task SendDriverError(ProblemDetailsForClientDTO _) => Task.CompletedTask;

        public async Task SendPropertyChanged(string key, uint? address, object? value, byte[]? bytes, bool frozen, string[] fieldsChanged)
        {
            if (fieldsChanged.Contains("value"))
            {
                await File.WriteAllTextAsync(Path.Combine(BuildEnvironment.OutputPropertiesDirectory, $"{key}.txt"), value?.ToString());
            }
        }
    }
}
