using GameHook.Domain.DTOs;
using GameHook.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameHook.Domain
{
    public record GameHookContainer : IGameHookContainer, IContainerForDriver
    {
        public ILogger Logger { get; }
        public IGameHookDriver Driver { get; }
        public IClientNotifier ClientNotifier { get; }
        public IPlatformOptions PlatformOptions { get; }
        public GameHookMapperMeta Meta { get; }
        public GameHookGlossary Glossary { get; }
        public GameHookMacros Macros { get; }
        public IDictionary<string, IGameHookProperty> Properties { get; } = new Dictionary<string, IGameHookProperty>();
        public const string PropertyPathDelimiter = ".";

        public GameHookContainer(ILogger logger, IClientNotifier updateTransmitter, IGameHookDriver driver, GameHookMapperMeta meta, GameHookMacros macros, GameHookGlossary glossary)
        {
            Logger = logger;
            ClientNotifier = updateTransmitter;
            Driver = driver;
            Meta = meta;
            Glossary = glossary;
            Macros = macros;

            if (meta.SchemaVersion != 1)
            {
                throw new GenericUserPresentableException("Mapper Schema is not supported by this version of GameHook.\nPlease upgrade the mapper file, or check for updates to the mapper.");
            }
            if (string.IsNullOrEmpty(meta.GameName))
            {
                throw new GenericUserPresentableException("Game name list not defined in metadata.\nPlease check the mapper file for errors.");
            }
            if (string.IsNullOrEmpty(meta.GamePlatform))
            {
                throw new GenericUserPresentableException("Game platform is not defined in metadata.\nPlease check the mapper file for errors.");
            }

            if (meta.GamePlatform == "NES") { PlatformOptions = new NES_PlatformOptions(); }
            else if (meta.GamePlatform == "GB") { PlatformOptions = new GB_PlatformOptions(); }
            else
            {
                throw new GenericUserPresentableException($"Platform {meta.GamePlatform} is not supported.\nPlease check the mapper file for errors.");
            }

        }

        public async Task Initialize()
        {
            Logger.LogInformation("Initializing mapper...");

            Driver.StopWatchingAndReset();

            var addressesToWatch = Properties.Values
                .GroupBy(x => new { x.Address, x.Length })
                .Select(x => x.Key)
                .ToList();

            foreach (var address in addressesToWatch)
            {
                Driver.AddAddressToWatch(address.Address, address.Length);
            }

            // Wait for the driver to populate the container.
            if (Driver.StartWatching(this) == false)
            {
                throw new GameHookContainerInitializationException($"Cannot establish a connection to {Driver.ProperName}.", null);
            }

            Logger.LogInformation("Initialization of mapper complete.");

            await Task.CompletedTask;
        }

        public IGameHookProperty GetRequiredPropertyByPath(string path) =>
            GetPropertyByPath(path) ?? throw new Exception($"Unable to find property by path '{path}'");
        public IGameHookProperty? GetPropertyByPath(string path)
        {
            Properties.TryGetValue(path, out var property);
            return property;
        }

        public void AddHookProperty(string path, IGameHookProperty property)
        {
            Driver.AddAddressToWatch(property.Address, property.Length);

            if (Properties.ContainsKey(path) == false)
            {
                Properties.Add(path, property);
            }
        }

        public async Task OnDriverMemoryChanged(int memoryAddress, int length, byte[] value)
        {
            var propertiesFrozen = Properties
                .Where(x => x.Value.Address.Equals(memoryAddress) && x.Value.Length.Equals(length) && x.Value.FreezeToBytes != null)
                .ToList();

            foreach (var keyValuePair in propertiesFrozen)
            {
                _ = Driver.WriteBytes(keyValuePair.Value.Address, keyValuePair.Value.FreezeToBytes ?? new byte[0]);
            }

            var propertiesToUpdate = Properties
                .Where(x => x.Value.Address.Equals(memoryAddress) && x.Value.Length.Equals(length))
                .ToList();

            foreach (var keyValuePair in propertiesToUpdate)
            {
                keyValuePair.Value.OnDriverMemoryChanged(value);

                await ClientNotifier.SendPropertyChanged(keyValuePair.Key, keyValuePair.Value.Value, keyValuePair.Value.Bytes.ToIntegerArray(), keyValuePair.Value.Frozen);
            }
        }

        public async Task OnDriverMemoryTimeout(DriverTimeoutException ex)
        {
            Logger.LogWarning(ex.Message);

            await ClientNotifier.SendDriverError(new ProblemDetailsForClientDTO() { Title = "DRIVER_TIMEOUT", Detail = $"{Driver.ProperName} timed out when reading memory address {ex.MemoryAddress.ToHexdecimalString()}." });
        }

        public async Task OnDriverError(ProblemDetailsForClientDTO problemDetails, Exception ex)
        {
            Logger.LogError(ex, ex.Message);

            await ClientNotifier.SendDriverError(problemDetails);
        }
    }
}