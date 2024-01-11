using Microsoft.Extensions.Configuration;

namespace GameHook.Domain
{
    static class AppSettingsHelper
    {
        public static string GetRequiredValue(this IConfiguration configuration, string key)
        {
            var value = configuration[key] ?? throw new Exception($"Configuration '{key}' is missing from appsettings.json");
            if (string.IsNullOrWhiteSpace(value)) throw new Exception($"Configuration '{key}' is empty.");

            return value;
        }
    }

    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            Urls = configuration["Urls"] ?? string.Empty;

            RETROARCH_LISTEN_IP_ADDRESS = configuration.GetRequiredValue("RETROARCH_LISTEN_IP_ADDRESS");
            RETROARCH_LISTEN_PORT = int.Parse(configuration.GetRequiredValue("RETROARCH_LISTEN_PORT"));
            RETROARCH_READ_PACKET_TIMEOUT_MS = int.Parse(configuration.GetRequiredValue("RETROARCH_READ_PACKET_TIMEOUT_MS"));
            RETROARCH_DELAY_MS_BETWEEN_READS = int.Parse(configuration.GetRequiredValue("RETROARCH_DELAY_MS_BETWEEN_READS"));

            RETROARCH_DELAY_MS_BETWEEN_READS = int.Parse(configuration.GetRequiredValue("RETROARCH_DELAY_MS_BETWEEN_READS"));

            BIZHAWK_DELAY_MS_BETWEEN_READS = int.Parse(configuration.GetRequiredValue("BIZHAWK_DELAY_MS_BETWEEN_READS"));

            AUTOMATIC_MAPPER_UPDATES = bool.Parse(configuration.GetRequiredValue("AUTOMATIC_MAPPER_UPDATES"));
            MAPPER_VERSION = configuration.GetRequiredValue("MAPPER_VERSION");

            MAPPER_DIRECTORY = Path.Combine(BuildEnvironment.ConfigurationDirectory, "Mappers");

            var processPath = Path.GetDirectoryName(Environment.ProcessPath) ?? throw new Exception("Unable to determine process path.");
            var localMapperDirectory = Path.Combine(processPath, "mappers");

            if (Directory.Exists(localMapperDirectory))
            {
                MAPPER_LOCAL_DIRECTORY = localMapperDirectory;
            }

            if (BuildEnvironment.IsDebug || BuildEnvironment.IsTestingBuild)
            {
                var overrideMapperDirectory = configuration["MAPPER_DIRECTORY"];
                if (string.IsNullOrEmpty(overrideMapperDirectory) == false)
                {
                    SET_CUSTOM_MAPPER_DIRECTORY = true;
                    MAPPER_DIRECTORY = overrideMapperDirectory;

                    AUTOMATIC_MAPPER_UPDATES = false;
                }
            }

            LOG_HTTP_TRAFFIC = bool.Parse(configuration.GetRequiredValue("LOG_HTTP_TRAFFIC"));
        }

        public string Urls { get; }

        public string RETROARCH_LISTEN_IP_ADDRESS { get; }
        public int RETROARCH_LISTEN_PORT { get; }
        public int RETROARCH_READ_PACKET_TIMEOUT_MS { get; }
        public int RETROARCH_DELAY_MS_BETWEEN_READS { get; }

        public int BIZHAWK_DELAY_MS_BETWEEN_READS { get; }

        public bool SHOW_READ_LOOP_STATISTICS { get; }

        public bool AUTOMATIC_MAPPER_UPDATES { get; }
        public string MAPPER_VERSION { get; }

        public string MAPPER_DIRECTORY { get; }
        public string? MAPPER_LOCAL_DIRECTORY { get; }
        public bool SET_CUSTOM_MAPPER_DIRECTORY { get; } = false;

        public bool LOG_HTTP_TRAFFIC { get; }
    }
}
