using Microsoft.Extensions.Configuration;

namespace GameHook.Domain
{
    public class GameHookConfiguration
    {
        public GameHookConfiguration(IConfiguration configuration)
        {
            OutputAllPropertiesToFilesystem = bool.Parse(configuration.GetRequiredValue("OUTPUT_ALL_PROPERTIES_TO_FILESYSTEM"));
        }

        public bool OutputAllPropertiesToFilesystem { get; }
    }
}
