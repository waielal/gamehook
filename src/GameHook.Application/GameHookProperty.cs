﻿using GameHook.Domain;
using GameHook.Domain.DTOs;
using GameHook.Domain.Interfaces;
using GameHook.Domain.Preprocessors;
using GameHook.Domain.ValueTransformers;
using NCalc;

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
        protected MapperUserSettingsDTO? MapperUserSettings => GameHookInstance?.Mapper?.UserSettings;
        public GameHookMapperVariables MapperVariables { get; }

        public string Path => MapperVariables.Path;
        public string Type => MapperVariables.Type;
        public int Size => MapperVariables.Size;
        public uint? Address { get; private set; }
        public bool IsDynamicAddress => MapperVariables.Address == null;

        public int? Position => MapperVariables.Position;
        public string? Reference => MapperVariables.Reference;
        public string? CharacterMap => MapperVariables.CharacterMap;

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

                return false;
            }
        }

        private bool ShouldReverseBytesIfLE()
        {
            // Little Endian has the least signifant byte first, so we need to reverse the byte array
            // when translating it to a value.

            return GameHookInstance.PlatformOptions?.EndianType == EndianTypes.LittleEndian;
        }

        private byte[] ReverseBytesIfLE(byte[] bytes)
        {
            if (bytes.Length == 1) { return bytes; }

            var workingBytes = (byte[])bytes.Clone();

            if (ShouldReverseBytesIfLE()) Array.Reverse(workingBytes);

            return workingBytes;
        }

        private static readonly PropertyValueResult EmptyPropertyValueResult = new PropertyValueResult();
        public PropertyValueResult Process(IEnumerable<MemoryAddressBlockResult> driverResult)
        {
            var preprocessorCache = GameHookInstance.PreprocessorCache ?? throw new Exception("GameHookInstance.PreprocessorCache is NULL.");

            // preBytes is used by preprocessors if
            // it needed to decrypt something.
            var fieldsChanged = new List<string>();
            uint? address = null;
            byte[]? rawBytes = null;
            byte[]? bytes = null;

            // Fetch address and bytes.
            if (MapperVariables.Preprocessor != null && MapperVariables.Preprocessor.Contains("data_block_a245dcac"))
            {
                var baseAddress = MapperVariables.Address ?? throw new Exception($"Property {Path} does not have a base address.");
                var decryptedDataBlock = preprocessorCache.data_block_a245dcac?[baseAddress] ?? throw new Exception($"Unable to retrieve data_block_a245dcac for property {Path} and address {Address?.ToHexdecimalString()}.");

                var structureIndex = MapperVariables.Preprocessor.GetIntParameterFromFunctionString(0);
                var offset = MapperVariables.Preprocessor.GetIntParameterFromFunctionString(1);

                var preprocessorResult = Preprocessors.read_data_block_a245dcac(structureIndex, offset, MapperVariables.Size, decryptedDataBlock);
                address = preprocessorResult.Address;
                rawBytes = preprocessorResult.EncryptedData;
                bytes = preprocessorResult.DecryptedData;
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

                var dmaAddress = Preprocessors.dma_967d10cc(memoryAddress, size: 4, offset, memoryBlock);
                if (dmaAddress == null) { return EmptyPropertyValueResult; }

                address = dmaAddress;
                bytes = driverResult.GetAddress((uint)address, MapperVariables.Size);
            }
            else if (MapperVariables.Address != null)
            {
                address = MapperVariables.Address;
                bytes = driverResult.GetAddress((uint)address, MapperVariables.Size);
            }
            else
            {
                throw new Exception("Unable to determine which code path to take for calculating address and bytes.");
            }

            // Check to see if neither returned a result.
            // This can happen if the game is in the middle of a reset.
            if (address == null && bytes == null)
            {
                return EmptyPropertyValueResult;
            }

            // Data validation.
            if (address == null) throw new Exception($"Unable to retrieve address for property '{Path}'");
            if (bytes == null) throw new Exception($"Unable to retrieve bytes for property '{Path}' at address {address?.ToHexdecimalString()}. Is the address within the drivers' memory address block ranges?");

            object? value;
            if (Address == address && Bytes?.SequenceEqual(rawBytes ?? bytes) == true)
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
                    "string" => StringTransformer.ToValue(bytes, GameHookInstance.GetMapper().Glossary[MapperVariables.CharacterMap ?? "defaultCharacterMap"]),
                    "uint" => UnsignedIntegerTransformer.ToValue(ReverseBytesIfLE(bytes)),
                    _ => throw new Exception($"Unknown type defined for {Path}, {Type}")
                };

                if (string.IsNullOrEmpty(MapperVariables.Postprocessor) == false)
                {
                    var postprocessorExpression = new Expression(MapperVariables.Postprocessor);
                    postprocessorExpression.Parameters["x"] = value;

                    // TODO: We probably shouldn't hardcode int32 here -- probably should be dependent on the platform?
                    value = Convert.ToInt32(postprocessorExpression.Evaluate());
                }

                if ((Type == "bit" || Type == "bool" || Type == "int" || Type == "uint") && string.IsNullOrEmpty(MapperVariables.Reference) == false)
                {
                    value = ReferenceTransformer.ToValue((int)value, GameHookInstance.GetMapper().Glossary[MapperVariables.Reference ?? throw new Exception("Missing property variable: reference")]);
                }
            }

            if (Address?.Equals(address) == false)
            {
                fieldsChanged.Add("address");
            }

            if (Bytes?.SequenceEqual(rawBytes ?? bytes) == false)
            {
                fieldsChanged.Add("bytes");
            }

            // Depending on the data type, we might need to compare values differently.
            if (Type == "bitArray")
            {
                if (Value != null && value != null && ((bool[])Value).SequenceEqual(((bool[])value)) == false)
                {
                    fieldsChanged.Add("value");
                }
            }
            else
            {
                if (Value == null && value != null)
                {
                    fieldsChanged.Add("value");
                }
                else if (Value?.Equals(value) == false)
                {
                    fieldsChanged.Add("value");
                }
            }

            Address = address;
            Bytes = rawBytes ?? bytes;
            Value = value;

            return new PropertyValueResult()
            {
                FieldsChanged = fieldsChanged
            };
        }

        public async Task<byte[]> WriteValue(string value, bool? freeze)
        {
            if (IsReadOnly) throw new Exception($"Property '{Path}' is read-only and cannot be modified.");

            var bytes = Type switch
            {
                "binaryCodedDecimal" => BinaryCodedDecimalTransformer.FromValue(int.Parse(value)),
                "bitArray" => BitFieldTransformer.FromValue(value.Split(' ').Select(bool.Parse).ToArray()),
                "bit" => BitTransformer.FromValue(Bytes ?? throw new Exception("Bytes is NULL."), MapperVariables.Position ?? throw new Exception("Position is NULL."), bool.Parse(value)),
                "bool" => BooleanTransformer.FromValue(bool.Parse(value)),
                "int" => IntegerTransformer.FromValue(int.Parse(value), ShouldReverseBytesIfLE()),
                "string" => StringTransformer.FromValue(value, Size, GameHookInstance.GetMapper().Glossary[MapperVariables.CharacterMap ?? "defaultCharacterMap"]),
                "uint" => UnsignedIntegerTransformer.FromValue(uint.Parse(value)),
                _ => throw new Exception($"Unknown type defined for {Path}, {Type}")
            };

            if (GameHookInstance.Driver == null) { throw new Exception("Driver is not defined."); }
            if (Address == null) { throw new Exception("Address is not defined."); }
            if (bytes == null) { throw new Exception("Bytes is not defined."); }

            await GameHookInstance.Driver.WriteBytes((uint)Address, bytes.Take(Size).ToArray());

            if (freeze == true)
            {
                await FreezeProperty(bytes);
            }
            else if (freeze == false)
            {
                await UnfreezeProperty();
            }

            return bytes;
        }

        public async Task WriteBytes(byte[] bytes, bool? freeze)
        {
            if (IsReadOnly) throw new Exception($"Property '{Path}' is read-only and cannot be modified.");

            if (Address == null)
            {
                throw new Exception($"Property '{Path}' address is NULL.");
            }

            if (MapperVariables.Preprocessor != null && MapperVariables.Preprocessor.Contains("data_block_a245dcac"))
            {
                var baseAddress = MapperVariables.Address ?? throw new Exception($"Property {Path} does not have a base address.");
                var dataBlock = GameHookInstance.PreprocessorCache?.data_block_a245dcac[baseAddress] ?? throw new Exception($"Unable to retrieve data_block_a245dcac for property {Path} and address {Address?.ToHexdecimalString()}.");

                var writeResults = Preprocessors.write_data_block_a245dcac((uint)Address, bytes, dataBlock);
                foreach (var result in writeResults)
                {
                    await GameHookInstance.GetDriver().WriteBytes(result.Address, result.Bytes);
                }
            }
            else
            {
                await GameHookInstance.GetDriver().WriteBytes((uint)Address, bytes);
            }

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
                await notifier.SendPropertyChanged(this, new string[] { "frozen" }, MapperUserSettings);
            }
        }

        public async Task UnfreezeProperty()
        {
            BytesFrozen = null;

            foreach (var notifier in GameHookInstance.ClientNotifiers)
            {
                await notifier.SendPropertyChanged(this, new string[] { "frozen" }, MapperUserSettings);
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
