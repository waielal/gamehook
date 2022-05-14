using GameHook.Domain;

namespace GameHook.WebAPI
{
    public class AppConfiguration
    {
        public AppConfiguration(ConfigurationManager configuration)
        {
            OutputPropertyValuesToFilesystem = bool.Parse(configuration.GetRequiredValue("OUTPUT_PROPERTY_VALUES_TO_FILESYSTEM"));
        }

        public bool OutputPropertyValuesToFilesystem { get; }
    }
}
