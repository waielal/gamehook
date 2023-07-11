namespace GameHook.Domain.Interfaces
{
    /// <summary>
    /// Driver interface for interacting with a emulator.
    /// 
    /// - Driver should not log anything above LogDebug.
    /// - Any errors encountered should be thrown as exceptions.
    /// </summary>
    public interface IGameHookDriver
    {
        string ProperName { get; }

        Task<IEnumerable<MemoryAddressBlockResult>> ReadBytes(IPlatformOptions? platformOptions, IEnumerable<MemoryAddressBlock> blocks);

        Task WriteBytes(IPlatformOptions? platformOptions, MemoryAddress startingMemoryAddress, byte[] values);
    }
}