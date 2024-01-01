using GameHook.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GameHook.Domain.Infrastructure
{
#pragma warning disable IDE1006 // Naming Styles
    class LatestRelease
    {
        public string tag_name { get; set; } = string.Empty;
        public bool prerelease { get; set; } = false;
    }
#pragma warning restore IDE1006 // Naming Styles

    class MapperData
    {
        public string? LastLocalApplicationVersion { get; set; }
        public string? LastLocalMapperVersion { get; set; }
    }

    public class MapperUpdateManager : IMapperUpdateManager
    {
        private readonly ILogger<MapperUpdateManager> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private MapperData MapperData { get; }

        private const string GithubReleasesApiUrl = "https://api.github.com/repos/gamehook-io/mappers/releases";

        public string MapperVersion => MapperData?.LastLocalMapperVersion ?? string.Empty;

        public MapperUpdateManager(ILogger<MapperUpdateManager> logger, AppSettings appSettings, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _appSettings = appSettings;
            _httpClientFactory = httpClientFactory;

            if (Directory.Exists(BuildEnvironment.ConfigurationDirectory) == false)
            {
                _logger.LogInformation("Creating configuration directory for GameHook.");

                Directory.CreateDirectory(BuildEnvironment.ConfigurationDirectory);
            }

            if (File.Exists(MapperDataFilepath) == false)
            {
                MapperData = new MapperData();
            }
            else
            {
                MapperData = JsonSerializer.Deserialize<MapperData>(File.ReadAllText(MapperDataFilepath))
                    ?? throw new Exception($"Could not deserialize mapper data from {MapperDataFilepath}");
            }
        }

        private async Task WriteMapperData() => await File.WriteAllTextAsync(MapperDataFilepath, JsonSerializer.Serialize(MapperData));

        private static string MapperDataFilepath => Path.Combine(BuildEnvironment.ConfigurationDirectory, "mappers.json");
        private static string MapperLocalDirectory => Path.Combine(BuildEnvironment.ConfigurationDirectory, "Mappers");

        private static string MapperZipTemporaryFilepath => Path.Combine(BuildEnvironment.ConfigurationDirectory, $"gamehook_mappers_main_temp.zip");
        private static string MapperTemporaryExtractionDirectory => Path.Combine(BuildEnvironment.ConfigurationDirectory, $"gamehook_mappers_temp\\");

        private void CleanupTemporaryFiles()
        {
            if (File.Exists(MapperZipTemporaryFilepath))
            {
                File.Delete(MapperZipTemporaryFilepath);
            }

            if (Directory.Exists(MapperTemporaryExtractionDirectory))
            {
                Directory.Delete(MapperTemporaryExtractionDirectory, true);
            }
        }

        private async Task DownloadMappers(HttpClient httpClient, string distUrl)
        {
            try
            {
                CleanupTemporaryFiles();

                // Download the ZIP from Github.
                var bytes = await httpClient.GetByteArrayAsync(distUrl);
                File.WriteAllBytes(MapperZipTemporaryFilepath, bytes);

                // Extract to the temporary directory.
                using var zout = ZipFile.OpenRead(MapperZipTemporaryFilepath);
                zout.ExtractToDirectory(MapperTemporaryExtractionDirectory);

                if (Directory.Exists(MapperLocalDirectory))
                {
                    Directory.Delete(MapperLocalDirectory, true);
                }

                // Move from inside of the temporary directory into the main mapper folder.
                Directory.Move(MapperTemporaryExtractionDirectory, MapperLocalDirectory);
            }
            finally
            {
                CleanupTemporaryFiles();
            }
        }

        public async Task<bool> CheckForUpdates()
        {
            try
            {
                if (_appSettings.AUTOMATIC_MAPPER_UPDATES == false)
                {
                    return false;
                }

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GameHook", BuildEnvironment.AssemblyProductVersion));

                var releasesJson = await httpClient.GetStringAsync(GithubReleasesApiUrl);
                var releases = JsonSerializer.Deserialize<IEnumerable<LatestRelease>>(releasesJson)
                    ?? throw new Exception("Could not deserialize the release manifest.");

                LatestRelease? wantedMapperRelease = null;

                if (_appSettings.MAPPER_VERSION == "latest")
                {
                    // Get the latest release that isn't considered a prerelease.
                    wantedMapperRelease = releases.FirstOrDefault(x => x.prerelease == false);
                }
                else if (_appSettings.MAPPER_VERSION == "prerelease")
                {
                    // Get the absolute latest release regardless of it it's a prerelease or not.
                    wantedMapperRelease = releases.FirstOrDefault();
                }
                else
                {
                    // If we're after a specific version, fetch that instead.
                    wantedMapperRelease = releases.FirstOrDefault(x => x.tag_name == _appSettings.MAPPER_VERSION);
                }

                if (wantedMapperRelease == null)
                {
                    throw new Exception($"Could not find a release tagged {_appSettings.MAPPER_VERSION} from release manifest.");
                }

                if (MapperData.LastLocalMapperVersion != wantedMapperRelease.tag_name)
                {
                    await DownloadMappers(httpClient, $"https://github.com/gamehook-io/mappers/releases/download/{wantedMapperRelease}/dist.zip");

                    MapperData.LastLocalApplicationVersion = BuildEnvironment.AssemblyProductVersion;
                    MapperData.LastLocalMapperVersion = wantedMapperRelease.tag_name;
                    await WriteMapperData();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not perform update check for mappers.");

                return false;
            }
        }
    }
}
