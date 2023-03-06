namespace GameHook.Domain.Interfaces
{
    public interface IGameHookInstance
    {
        List<IClientNotifier> ClientNotifiers { get; }
        bool Initalized { get; }
        IGameHookDriver? Driver { get; }
        IGameHookMapper? Mapper { get; }
        PreprocessorCache? PreprocessorCache { get; }
        IPlatformOptions? PlatformOptions { get; }
        IEnumerable<MemoryAddressBlock>? BlocksToRead { get; }

        const int DELAY_MS_BETWEEN_READS = 25;

        IPlatformOptions GetPlatformOptions();
        IGameHookDriver GetDriver();
        IGameHookMapper GetMapper();

        Task ResetState();

        Task Load(IGameHookDriver driver, string mapperId);
        Task ReadLoop();
        Task Read();
    }
}
