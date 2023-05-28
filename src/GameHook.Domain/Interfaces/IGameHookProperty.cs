using GameHook.Domain.Preprocessors;

namespace GameHook.Domain.Interfaces
{
    public class PreprocessorCache
    {
        public Dictionary<MemoryAddress, DataBlock_a245dcac_Cache> data_block_a245dcac { get; set; } = new Dictionary<MemoryAddress, DataBlock_a245dcac_Cache>();
    }

    public class PropertyValueResult
    {
        public IEnumerable<string> FieldsChanged { get; init; } = new List<string>();
    }

    public class GameHookMapperVariables
    {
        public string Path { get; init; } = string.Empty;

        public string Type { get; init; } = string.Empty;
        public MemoryAddress? Address { get; init; }
        public int Length { get; init; } = 1;
        public int? Position { get; init; }
        public string? Reference { get; init; }
        public string? Description { get; init; }

        public string? Expression { get; init; }
        public string? Preprocessor { get; init; }
        public string? PostprocessorReader { get; init; }
        public string? PostprocessorWriter { get; init; }

        public string? StaticValue { get; init; }
    }

    public interface IGameHookProperty
    {
        GameHookMapperVariables MapperVariables { get; }
        GlossaryList? Glossary { get; }

        string Path { get; }
        string Type { get; }
        int Length { get; }
        uint? Address { get; }
        bool IsDynamicAddress { get; }

        int? Position { get; }

        string? Reference { get; }

        object? Value { get; }
        byte[]? Bytes { get; }
        byte[]? BytesFrozen { get; }

        bool Frozen { get; }

        string? Description { get; }

        PropertyValueResult Process(IEnumerable<MemoryAddressBlockResult> driverResult);
        Task<byte[]> WriteValue(string? value, bool? freeze);
        Task WriteBytes(byte[] bytes, bool? freeze);
        Task FreezeProperty(byte[] bytesFrozen);
        Task UnfreezeProperty();
    }
}
