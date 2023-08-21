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
        public MemoryAddressBlock(string name, MemoryAddress startingAddress, MemoryAddress endingAddress)
        {
            Name = name;
            StartingAddress = startingAddress;
            EndingAddress = endingAddress;
        }

        public string Name { get; init; }
        public MemoryAddress StartingAddress { get; init; }
        public MemoryAddress EndingAddress { get; init; }
    }

    public record MemoryAddressBlockResult
    {
        public string Name { get; init; }
        public MemoryAddress StartingAddress { get; init; }
        public MemoryAddress EndingAddress { get; init; }
        public byte[] Data { get; init; }
    }

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

    public class GlossaryList
    {
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; }
        public IEnumerable<GlossaryListItem> Values { get; set; } = new List<GlossaryListItem>();

        public GlossaryListItem? GetFirstOrDefaultByValue(object? value)
        {
            return Values.FirstOrDefault(x => x.Value == value);
        }

        public GlossaryListItem? GetFirstOrDefaultByKey(string key)
        {
            return Values.FirstOrDefault(x => x.Key.ToString() == key);
        }

        public GlossaryListItem? GetFirstOrDefaultByKey(ulong key)
        {
            return Values.FirstOrDefault(x => x.Key == key);
        }
    }

    public class GlossaryListItem
    {
        public ulong Key { get; set; }
        public object? Value { get; set; }
    }
}
