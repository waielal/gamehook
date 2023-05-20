using Microsoft.Extensions.Configuration;

namespace GameHook.Domain
{
    public class GameHookConfiguration
    {
        public GameHookConfiguration(IConfiguration configuration)
        {
            var urls = configuration["Urls"];
            if (string.IsNullOrEmpty(urls) == false)
            {
                Urls = urls.Split(',');
            }
            else
            {
                Urls = Array.Empty<string>();
            }

            OutputAllPropertiesToFilesystem = bool.Parse(configuration.GetRequiredValue("OUTPUT_ALL_PROPERTIES_TO_FILESYSTEM"));
            OutputTransformedMapper = bool.Parse(configuration.GetRequiredValue("OUTPUT_TRANSFORMED_MAPPER"));
        }

        public IEnumerable<string> Urls { get; }
        public bool OutputAllPropertiesToFilesystem { get; }
        public bool OutputTransformedMapper { get; }
    }
}
