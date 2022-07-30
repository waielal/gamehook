using GameHook.Domain.Preprocessors;

namespace GameHook.Domain.Interfaces
{
    public class PreprocessorCache
    {
        public Dictionary<MemoryAddress, DataBlock_a245dcac>? data_block_a245dcac { get; set; }
    }

    public class GameHookPropertyProcessResult
    {
        public List<string> FieldsChanged { get; init; } = new List<string>();
    }

    public class GameHookMapperVariables
    {
        public string Path { get; init; } = string.Empty;

        public string Type { get; init; } = string.Empty;
        public MemoryAddress? Address { get; init; }
        public int Size { get; init; } = 1;
        public int? Position { get; init; }
        public string? Reference { get; init; }
        public string? Description { get; init; }

        public string? Expression { get; init; }
        public string? Preprocessor { get; init; }
    }

    public interface IGameHookProperty
    {
        GameHookMapperVariables MapperVariables { get; }

        string Path { get; }
        string Type { get; }
        int Size { get; }
        uint? Address { get; }
        bool IsDynamicAddress { get; }

        int? Position { get; }

        string? Reference { get; }

        object? Value { get; }
        byte[]? Bytes { get; }
        byte[]? BytesFrozen { get; }

        bool Frozen { get; }

        string? Description { get; }

        Task<GameHookPropertyProcessResult> Process(IEnumerable<MemoryAddressBlockResult> driverResult, PreprocessorCache preprocessorCache);
        Task WriteValue(object value, bool? freeze);
        Task WriteBytes(byte[] bytes, bool? freeze);
        Task FreezeProperty(byte[] bytesFrozen);
        Task UnfreezeProperty();
    }
}
