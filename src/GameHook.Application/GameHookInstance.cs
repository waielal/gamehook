using System.Xml.Linq;
using GameHook.Domain;
using GameHook.Domain.Interfaces;
using GameHook.Domain.Preprocessors;
using Microsoft.Extensions.Logging;

namespace GameHook.Application
{
    public class GameHookInstance : IGameHookInstance
    {
        private ILogger<GameHookInstance> Logger { get; }
        public GameHookConfiguration Configuration { get; }
        private IMapperFilesystemProvider MapperFilesystemProvider { get; }
        private bool OutputDriverJson { get; set; } = false;
        public List<IClientNotifier> ClientNotifiers { get; }
        public bool Initalized { get; private set; }
        private CancellationTokenSource? ReadLoopToken { get; set; }
        public IGameHookDriver? Driver { get; private set; }
        public IGameHookMapper? Mapper { get; private set; }
        public PreprocessorCache? PreprocessorCache { get; private set; }
        public IPlatformOptions? PlatformOptions { get; private set; }
        public IEnumerable<MemoryAddressBlock>? BlocksToRead { get; private set; }
        public const int DELAY_MS_BETWEEN_READS = 25;

        public GameHookInstance(ILogger<GameHookInstance> logger, GameHookConfiguration configuration, IMapperFilesystemProvider provider, IEnumerable<IClientNotifier> clientNotifiers)
        {
            Logger = logger;
            Configuration = configuration;
            MapperFilesystemProvider = provider;
            ClientNotifiers = clientNotifiers.ToList();
        }

        public IPlatformOptions GetPlatformOptions() => PlatformOptions ?? throw new Exception("PlatformOptions is null.");
        public IGameHookDriver GetDriver() => Driver ?? throw new Exception("Driver is null.");
        public IGameHookMapper GetMapper() => Mapper ?? throw new Exception("Mapper is null.");

        public async Task ResetState()
        {
            if (ReadLoopToken != null && ReadLoopToken.Token.CanBeCanceled)
            {
                ReadLoopToken.Cancel();
            }

            Initalized = false;
            ReadLoopToken = null;

            Driver = null;
            Mapper = null;
            PlatformOptions = null;
            BlocksToRead = null;

            await ClientNotifiers.ForEachAsync(async x => await x.SendInstanceReset());
        }

        public async Task Load(IGameHookDriver driver, string mapperId)
        {
            try
            {
                await ResetState();

                Logger.LogDebug("Creating GameHook mapper instance...");

                Driver = driver;

                // Load the mapper file.
                if (string.IsNullOrEmpty(mapperId))
                {
                    throw new ArgumentException("ID was NULL or empty.", nameof(mapperId));
                }

                // Get the file path from the filesystem provider.
                var mapperFile = MapperFilesystemProvider.MapperFiles.SingleOrDefault(x => x.Id == mapperId) ??
                                 throw new Exception($"Unable to determine a mapper with the ID of {mapperId}.");

                if (File.Exists(mapperFile.AbsolutePath) == false)
                {
                    throw new FileNotFoundException($"File was not found in the {mapperFile.Type} mapper folder.", mapperFile.DisplayName);
                }

                var mapperContents = await File.ReadAllTextAsync(mapperFile.AbsolutePath);
                if (mapperFile.AbsolutePath.EndsWith(".yml"))
                {
                    Mapper = GameHookMapperYamlFactory.ReadMapper(this, MapperFilesystemProvider, mapperFile.Id);
                }
                else if (mapperFile.AbsolutePath.EndsWith(".xml"))
                {
                    var mapperXDocument = XDocument.Parse(mapperContents);
                    Mapper = GameHookMapperXmlFactory.LoadMapperFromFile(this, mapperXDocument);
                }
                else
                {
                    throw new Exception($"Invalid extension for mapper.");
                }

                PlatformOptions = Mapper.Metadata.GamePlatform switch
                {
                    "NES" => new NES_PlatformOptions(),
                    "SNES" => new SNES_PlatformOptions(),
                    "GB" => new GB_PlatformOptions(),
                    "GBA" => new GBA_PlatformOptions(),
                    "PSX" => new PSX_PlatformOptions(),
                    _ => throw new Exception($"Unknown game platform {Mapper.Metadata.GamePlatform}.")
                };

                // Calculate the blocks to read from the mapper memory addresses.
                var addressesToWatch = Mapper.Properties
                    .Where(x => x.MapperVariables.Address != null)
                    .Select(x => x.MapperVariables.Address)
                    .ToList();

                var addressesFromDma = Mapper.Properties
                    .Where(x => x.MapperVariables.Preprocessor?.Contains("dma_967d10cc") ?? false)
                    .Select(x => x.MapperVariables.Preprocessor?.GetMemoryAddressFromFunction(0))
                    .ToList();

                addressesToWatch.AddRange(addressesFromDma);
                addressesToWatch = addressesToWatch.Distinct().ToList();

                BlocksToRead = PlatformOptions.Ranges
                                .Where(x => addressesToWatch.Any(y => y?.Between(x.StartingAddress, x.EndingAddress) ?? false))
                                .ToList();

                Logger.LogDebug($"Requested {BlocksToRead.Count()}/{PlatformOptions.Ranges.Count()} ranges of memory.");
                Logger.LogDebug($"Requested ranges: {string.Join(", ", BlocksToRead.Select(x => x.Name))}");

                await Read();

                Initalized = true;

                await ClientNotifiers.ForEachAsync(async x => await x.SendMapperLoaded(Mapper));

                // Start the read loop once successfully running once.
                ReadLoopToken = new CancellationTokenSource();
                _ = Task.Run(ReadLoop, ReadLoopToken.Token);

                Logger.LogInformation($"Loaded mapper for {Mapper.Metadata.GameName} ({Mapper.Metadata.Id}).");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured when loading the mapper.");

                await ResetState();

                throw;
            }
        }

        public async Task ReadLoop()
        {
            while (ReadLoopToken != null && ReadLoopToken.IsCancellationRequested == false)
            {
                try
                {
                    await Read();
                    await Task.Delay(DELAY_MS_BETWEEN_READS);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "An error occured when read looping the mapper.");

                    await ResetState();
                }
            }
        }

        public async Task Read()
        {
            if (Driver == null) throw new Exception("Driver is null.");
            if (PlatformOptions == null) throw new Exception("Platform options are null.");
            if (Mapper == null) throw new Exception("Mapper is null.");
            if (BlocksToRead == null) throw new Exception("BlocksToRead is null.");

            var driverResult = await Driver.ReadBytes(BlocksToRead);

            /*
            #if DEBUG
                        if (OutputDriverJson == false)
                        {
                            File.WriteAllText(Path.Combine(BuildEnvironment.ConfigurationDirectory, "driver.json"), System.Text.Json.JsonSerializer.Serialize(driverResult));
                            OutputDriverJson = true;
                        }
            #endif
            */

            // Preprocessor Cache
            // Certain preprocessors are costly to run for every property, so cache them here.
            if (PreprocessorCache == null)
            {
                PreprocessorCache = new PreprocessorCache();
            }

            // data_block_a245dcac
            var dataBlock_a245dcac_Properties = Mapper.Properties
                .Where(x => x.MapperVariables.Preprocessor?.StartsWith("data_block_a245dcac(") ?? false)
                .GroupBy(x => x.MapperVariables.Address ?? 0)
                .ToList();

            // Key is the starting memory address block.
            dataBlock_a245dcac_Properties.ForEach(x =>
            {
                PreprocessorCache.data_block_a245dcac.TryGetValue(x.Key, out var existingCache);
                PreprocessorCache.data_block_a245dcac[x.Key] = Preprocessors.decrypt_data_block_a245dcac(existingCache, driverResult, x.Key);
            });

            // Processor
            Task.WaitAll(Mapper.Properties.Select(async x =>
            {
                try
                {
                    var result = x.Process(driverResult);

                    if (result.FieldsChanged.Any())
                    {
                        if (x.Frozen && x.BytesFrozen != null)
                        {
                            await x.WriteBytes(x.BytesFrozen, null);
                        }
                        else
                        {
                            Task.WaitAll(ClientNotifiers.Select(async notifier =>
                            {
                                await notifier.SendPropertyChanged(x, result.FieldsChanged.ToArray());
                            }).ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Failed to process propery {x.Path}.");
                    throw new PropertyProcessException($"Failed to process propery {x.Path}.", ex);
                }
            }).ToArray());
        }
    }
}
