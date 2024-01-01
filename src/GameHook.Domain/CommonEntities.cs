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

    public class GlossaryList
    {
        public string Name { get; init; } = string.Empty;
        public string? Type { get; init; }

        public IEnumerable<GlossaryListItem> Values { get; init; } = new List<GlossaryListItem>();

        public GlossaryListItem? GetSingleOrDefaultByValue(object? value)
        {
            return Values.SingleOrDefault(x => string.Equals(x.Value?.ToString(), value?.ToString(), StringComparison.Ordinal));
        }
        public GlossaryListItem? GetSingleOrDefaultByKey(ulong key)
        {
            return Values.SingleOrDefault(x => x.Key == key);
        }
        public GlossaryListItem GetSingleByValue(object? value)
        {
            return GetSingleOrDefaultByValue(value) ?? throw new Exception($"Missing dictionary value for '{value}', value was not found in reference list {Name}.");
        }
        public GlossaryListItem GetSingleByKey(ulong key)
        {
            return GetSingleOrDefaultByKey(key) ?? throw new Exception($"Missing dictionary key for '{key}', key was not found in reference list {Name}.");
        }
    }

    public class GlossaryListItem
    {
        public GlossaryListItem(ulong key, object? value)
        {
            Key = key;
            Value = value;
        }

        public ulong Key { get; }
        public object? Value { get; }
    }
}
