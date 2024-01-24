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

    public class ReferenceItems
    {
        public string Name { get; init; } = string.Empty;
        public string? Type { get; init; }

        public IEnumerable<ReferenceItem> Values { get; init; } = new List<ReferenceItem>();

        public ReferenceItem? GetSingleOrDefaultByValue(object? value)
        {
            return Values.SingleOrDefault(x => string.Equals(x.Value?.ToString(), value?.ToString(), StringComparison.Ordinal));
        }
        public ReferenceItem? GetSingleOrDefaultByKey(ulong key)
        {
            return Values.SingleOrDefault(x => x.Key == key);
        }
        public ReferenceItem GetSingleByValue(object? value)
        {
            return GetSingleOrDefaultByValue(value) ?? throw new Exception($"Missing dictionary value for '{value}', value was not found in reference list {Name}.");
        }
        public ReferenceItem GetSingleByKey(ulong key)
        {
            return GetSingleOrDefaultByKey(key) ?? throw new Exception($"Missing dictionary key for '{key}', key was not found in reference list {Name}.");
        }
    }

    public record ReferenceItem(ulong Key, object? Value);
}
