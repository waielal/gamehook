using GameHook.Domain;
using GameHook.Domain.Interfaces;
using GameHook.Domain.Preprocessors;
using Microsoft.Extensions.Logging;

namespace GameHook.Application
{
    public class GameHookInstance : IGameHookInstance
    {
        private ILogger<GameHookInstance> Logger { get; }
        private IMapperFilesystemProvider MapperFilesystemProvider { get; }
        public List<IClientNotifier> ClientNotifiers { get; }
        public bool Initalized { get; private set; } = false;
        private CancellationTokenSource? ReadLoopToken { get; set; }
        public IGameHookDriver? Driver { get; private set; }
        public IGameHookMapper? Mapper { get; private set; }
        public PreprocessorCache? PreprocessorCache { get; private set; }
        public IPlatformOptions? PlatformOptions { get; private set; }
        public IEnumerable<MemoryAddressBlock>? BlocksToRead { get; private set; }
        public const int DELAY_MS_BETWEEN_READS = 25;

        public GameHookInstance(ILogger<GameHookInstance> logger, IMapperFilesystemProvider provider, IEnumerable<IClientNotifier> clientNotifiers)
        {
            Logger = logger;
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
            await ResetState();

            try
            {
                Logger.LogDebug("Creating GameHook mapper instance...");

                Driver = driver;
                Mapper = GameHookMapperFactory.ReadMapper(this, MapperFilesystemProvider, mapperId);
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
                    .Cast<uint>()
                    .ToList();

                var addressesFromDma = Mapper.Properties
                    .Where(x => x.MapperVariables.Preprocessor?.Contains("dma_967d10cc") ?? false)
                    .Select(x => x.MapperVariables.Preprocessor?.GetHexdecimalParameterFromFunctionString(0))
                    .Cast<uint>()
                    .ToList();

                addressesToWatch.AddRange(addressesFromDma);

                addressesToWatch = addressesToWatch
                    .Distinct()
                    .ToList();

                BlocksToRead = PlatformOptions.Ranges
                                .Where(x => addressesToWatch.Any(y => y.Between(x.StartingAddress, x.EndingAddress)))
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
                                await notifier.SendPropertyChanged(x, result.FieldsChanged.ToArray(), Mapper.UserSettings);
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
