namespace GameHook.Domain.Interfaces
{
    public record PlatformRange(string Name, MemoryAddress StartingAddress, MemoryAddress EndingAddress);

    public interface IPlatformOptions
    {
        public EndianTypeEnum EndianType { get; }

        public IEnumerable<PlatformRange> Ranges { get; }
    }
}
