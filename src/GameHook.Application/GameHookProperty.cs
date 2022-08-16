using GameHook.Domain;
using GameHook.Domain.Interfaces;
using GameHook.Domain.Preprocessors;
using GameHook.Domain.ValueTransformers;

namespace GameHook.Application
{
    public class GameHookProperty : IGameHookProperty
    {
        public GameHookProperty(GameHookInstance gameHookInstance, GameHookMapperVariables mapperVariables)
        {
            GameHookInstance = gameHookInstance;
            MapperVariables = mapperVariables;
        }

        protected GameHookInstance GameHookInstance { get; }
        public GameHookMapperVariables MapperVariables { get; }

        public string Path => MapperVariables.Path;
        public string Type => MapperVariables.Type;
        public int Size => MapperVariables.Size;
        public uint? Address { get; private set; }
        public bool IsDynamicAddress => MapperVariables.Address == null;

        public int? Position => MapperVariables.Position;
        public string? Reference => MapperVariables.Reference;

        public object? Value { get; private set; }
        public byte[]? Bytes { get; private set; }
        public byte[]? BytesFrozen { get; private set; }

        public bool Frozen => BytesFrozen != null;

        public string? Description => MapperVariables.Description;

        public bool IsReadOnly
        {
            get
            {
                if (Address == null) return true;
                if (string.IsNullOrEmpty(MapperVariables.Preprocessor) == false) return true;

                return false;
            }
        }

        private byte[] ReverseBytesIfLE(byte[] bytes)
        {
            if (bytes.Length == 1) { return bytes; }

            var workingBytes = (byte[])bytes.Clone();

            // Little Endian has the least signifant byte first, so we need to reverse the byte array
            // when translating it to a value.
            if (GameHookInstance.PlatformOptions?.EndianType == EndianTypes.LittleEndian) Array.Reverse(workingBytes);

            return workingBytes;
        }

        public async Task<GameHookPropertyProcessResult> Process(IEnumerable<MemoryAddressBlockResult> driverResult, PreprocessorCache preprocessorCache)
        {
            var result = new GameHookPropertyProcessResult();

            uint? address = null;
            byte[]? bytes = null;

            // Fetch address and bytes.
            if (MapperVariables.Address != null)
            {
                address = MapperVariables.Address;
                bytes = driverResult.GetAddress((uint)address, MapperVariables.Size);
            }
            else if (MapperVariables.Preprocessor != null && MapperVariables.Preprocessor.Contains("data_block_a245dcac"))
            {
                var baseAddress = MapperVariables.Address ?? throw new Exception($"Property {Path} does not have a base address.");
                var decryptedDataBlock = preprocessorCache.data_block_a245dcac?[baseAddress] ?? throw new Exception($"Unable to retrieve data_block_a245dcac for property {Path} and address {Address?.ToHexdecimalString()}.");

                var structureIndex = MapperVariables.Preprocessor.GetIntParameterFromFunctionString(0);
                var offset = MapperVariables.Preprocessor.GetIntParameterFromFunctionString(1);

                var preprocessorResult = Preprocessors.data_block_a245dcac(structureIndex, offset, MapperVariables.Size, decryptedDataBlock);
                if (preprocessorResult.Address == null || preprocessorResult.Bytes == null)
                {
                    throw new Exception($"Preprocessor data_block_a245dcac returned null on path '{Path}'.");
                }

                address = preprocessorResult.Address;
                bytes = preprocessorResult.Bytes;
            }
            else if (MapperVariables.Preprocessor != null && MapperVariables.Preprocessor.Contains("dma_967d10cc"))
            {
                var memoryAddress = MapperVariables.Preprocessor.GetHexdecimalParameterFromFunctionString(0);
                var offset = MapperVariables.Preprocessor.GetIntParameterFromFunctionString(1);

                var memoryBlock = driverResult.GetResultWithinRange(memoryAddress);
                if (memoryBlock == null)
                {
                    throw new Exception($"Unable to retrieve memory block for property {Path} at address {memoryAddress.ToHexdecimalString()}.");
                }

                var preprocessorResult = Preprocessors.dma_967d10cc(memoryAddress, size: 4, offset, memoryBlock);
                if (preprocessorResult.Address == null)
                {
                    throw new Exception($"Preprocessor dma_967d10cc returned null on path '{Path}'.");
                }

                address = preprocessorResult.Address;
                bytes = driverResult.GetAddress((uint)address, MapperVariables.Size);
            }

            // Data validation.
            if (address == null)
            {
                throw new Exception($"Unable to retrieve address for property '{Path}'");
            }
            if (bytes == null)
            {
                throw new Exception($"Unable to retrieve bytes for property '{Path}' at address {address?.ToHexdecimalString()}. Is the address within the drivers' memory address block ranges?");
            }

            // Determine if we need to reset a frozen property.
            if (Bytes?.SequenceEqual(bytes) == false && Frozen)
            {
                await GameHookInstance.GetDriver().WriteBytes(address ?? 0, BytesFrozen ?? throw new Exception("Attempted to force a frozen bytes, but BytesFrozen was NULL."));
            }

            object? value;
            if (Address == address && Bytes?.SequenceEqual(bytes) == true)
            {
                // Fast path - if the bytes match, then we can assume the property has not been
                // updated since last poll.

                // Do nothing, we don't need to calculate the new value as
                // the bytes are the same.

                value = Value;
            }
            else
            {
                value = Type switch
                {
                    "binaryCodedDecimal" => BinaryCodedDecimalTransformer.ToValue(bytes),
                    "bitArray" => BitFieldTransformer.ToValue(bytes),
                    "bit" => BitTransformer.ToValue(bytes, MapperVariables.Position ?? throw new Exception("Missing property variable: Position")),
                    "bool" => BooleanTransformer.ToValue(bytes),
                    "int" => IntegerTransformer.ToValue(ReverseBytesIfLE(bytes)),
                    "reference" => ReferenceTransformer.ToValue(ReverseBytesIfLE(bytes), GameHookInstance.GetMapper().Glossary[MapperVariables.Reference ?? throw new Exception("Missing property variable: reference")]),
                    "string" => StringTransformer.ToValue(bytes, GameHookInstance.GetMapper().Glossary[MapperVariables.Reference ?? "defaultCharacterMap"]),
                    "uint" => UnsignedIntegerTransformer.ToValue(ReverseBytesIfLE(bytes)),
                    _ => throw new Exception($"Unknown type defined for {Path}, {Type}")
                };
            }

            if (Address?.Equals(address) == false)
            {
                result.FieldsChanged.Add("address");
            }

            if (Bytes?.SequenceEqual(bytes) == false)
            {
                result.FieldsChanged.Add("bytes");
            }

            // Depending on the data type, we might need to compare values differently.
            if (Type == "bitArray")
            {
                if (Value != null && value != null && ((bool[])Value).SequenceEqual(((bool[])value)) == false)
                {
                    result.FieldsChanged.Add("value");
                }
            }
            else
            {
                if (Value?.Equals(value) == false)
                {
                    result.FieldsChanged.Add("value");
                }
            }

            Address = address;
            Bytes = bytes;
            Value = value;

            if (result.FieldsChanged.Count > 0)
            {
                foreach (var notifier in GameHookInstance.ClientNotifiers)
                {
                    _ = notifier.SendPropertyChanged(Path, Address, Value, Bytes, Frozen, result.FieldsChanged.ToArray());
                }
            }

            return result;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task WriteValue(object value, bool? freeze)
        {
            if (IsReadOnly) throw new Exception($"Property '{Path}' is read-only and cannot be modified.");

            throw new NotSupportedException();
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task WriteBytes(byte[] bytes, bool? freeze)
        {
            if (IsReadOnly) throw new Exception($"Property '{Path}' is read-only and cannot be modified.");

            if (Address == null)
            {
                throw new Exception($"Property '{Path}' address is NULL.");
            }

            await GameHookInstance.GetDriver().WriteBytes((uint)Address, bytes);

            if (freeze == true)
            {
                await FreezeProperty(bytes);
            }
            else if (freeze == false)
            {
                await UnfreezeProperty();
            }
        }

        public async Task FreezeProperty(byte[] bytesFrozen)
        {
            BytesFrozen = bytesFrozen;

            foreach (var notifier in GameHookInstance.ClientNotifiers)
            {
                await notifier.SendPropertyChanged(Path, Address, Value, Bytes, Frozen, new string[] { "frozen" });
            }
        }

        public async Task UnfreezeProperty()
        {
            BytesFrozen = null;

            foreach (var notifier in GameHookInstance.ClientNotifiers)
            {
                await notifier.SendPropertyChanged(Path, Address, Value, Bytes, Frozen, new string[] { "frozen" });
            }
        }

        public override string ToString()
        {
            if (Bytes == null || Bytes.Any() == false)
            {
                return "N/A";
            }

            return $"{Value} [{string.Join(' ', Bytes)}]" ?? string.Empty;
        }
    }
}
