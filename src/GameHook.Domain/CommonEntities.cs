using Microsoft.Extensions.Configuration;

namespace GameHook.Domain
{
    public enum EndianTypes
    {
        BigEndian,
        LittleEndian
    }

    public record MemoryAddressBlock
    {
        public MemoryAddressBlock(int index, string name, MemoryAddress startingAddress, MemoryAddress endingAddress)
        {
            Index = index;
            Name = name;
            StartingAddress = startingAddress;
            EndingAddress = endingAddress;
        }

        public int Index { get; init; }
        public string Name { get; init; }
        public MemoryAddress StartingAddress { get; init; }
        public MemoryAddress EndingAddress { get; init; }
    }
    public record MemoryAddressBlockResult(int Index, string Name, MemoryAddress StartingAddress, MemoryAddress EndingAddress, byte[] Data);

    public class DriverOptions
    {
        public DriverOptions(IConfiguration configuration)
        {
            IpAddress = configuration.GetRequiredValue("DRIVER_LISTEN_IP_ADDRESS");
            Port = int.Parse(configuration.GetRequiredValue("DRIVER_LISTEN_PORT"));
            DriverTimeoutCounter = int.Parse(configuration.GetRequiredValue("DRIVER_TIMEOUT_COUNTER"));
        }

        public string IpAddress { get; }
        public int Port { get; }
        public int DriverTimeoutCounter { get; }
    }

    public class GlossaryItem
    {
        public GlossaryItem(ulong key, object? value)
        {
            Key = key;
            Value = value;
        }

        public ulong Key { get; private set; }
        public object? Value { get; private set; }
    }
}
