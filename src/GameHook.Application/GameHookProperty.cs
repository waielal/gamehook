using GameHook.Domain;
using GameHook.Domain.Interfaces;
using GameHook.Domain.Preprocessors;
using GameHook.Domain.ValueTransformers;
using NCalc;

namespace GameHook.Application
{
    public class GameHookProperty : IGameHookProperty
    {
        public GameHookProperty(IGameHookInstance gameHookInstance, GameHookMapperVariables mapperVariables)
        {
            GameHookInstance = gameHookInstance;
            MapperVariables = mapperVariables;

            if (IsStaticValue)
            {
                Value = mapperVariables.StaticValue;
            }
        }

        protected IGameHookInstance GameHookInstance { get; }
        public GameHookMapperVariables MapperVariables { get; }

        public GlossaryList? Glossary
        {
            get
            {
                if (GameHookInstance.Mapper == null)
                {
                    throw new Exception("Mapper is NULL.");
                }

                if (Type == "string" && string.IsNullOrEmpty(MapperVariables.Reference))
                {
                    return GameHookInstance.Mapper.Glossary.Single(x => x.Name == "defaultCharacterMap");
                }
                else if (Type == "string2" && string.IsNullOrEmpty(MapperVariables.Reference))
                {
                    return GameHookInstance.Mapper.Glossary.Single(x => x.Name == "defaultCharacterMap");
                }

                if (string.IsNullOrEmpty(MapperVariables.Reference)) { return null; }
                return GameHookInstance.Mapper.Glossary.Single(x => x.Name == MapperVariables.Reference);
            }
        }

        public string Path => MapperVariables.Path;
        public string Type => MapperVariables.Type;
        public int Length => MapperVariables.Length;
        public uint? Address { get; private set; }
        public bool IsDynamicAddress => MapperVariables.Address == null;

        public int? Position => MapperVariables.Position;
        public string? Reference => MapperVariables.Reference;

        public object? Value { get; private set; }
        public byte[]? Bytes { get; private set; }
        public byte[]? BytesFrozen { get; private set; }

        public bool Frozen => BytesFrozen != null;

        public string? Description => MapperVariables.Description;

        public bool IsStaticValue
        {
            get { return string.IsNullOrEmpty(MapperVariables.StaticValue) == false; }
        }

        public bool IsReadOnly
        {
            get
            {
                if (Address == null) return true;
                if (IsStaticValue) return true;

                return false;
            }
        }

        private static readonly PropertyValueResult EmptyPropertyValueResult = new PropertyValueResult();
        public PropertyValueResult Process(IEnumerable<MemoryAddressBlockResult> driverResult)
        {
            if (IsStaticValue)
            {
                return new PropertyValueResult();
            }

            if (GameHookInstance == null || GameHookInstance.PlatformOptions == null)
            {
                throw new Exception("GameHookInstance is not initalized properly.");
            }

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
                try
                {
                    var baseAddress = MapperVariables.Address ?? throw new Exception($"Property {Path} does not have a base address.");
                    var decryptedDataBlock = preprocessorCache.data_block_a245dcac?[baseAddress] ?? throw new Exception($"Unable to retrieve data_block_a245dcac for property {Path} and address {Address?.ToHexdecimalString()}.");

                    var structureIndex = MapperVariables.Preprocessor.GetIntParameterFromFunction(0);
                    var offset = MapperVariables.Preprocessor.GetIntParameterFromFunction(1);

                    var preprocessorResult = Preprocessor_a245dcac.Read(structureIndex, offset, MapperVariables.Length, decryptedDataBlock);
                    address = preprocessorResult.Address;
                    rawBytes = preprocessorResult.EncryptedData;
                    bytes = preprocessorResult.DecryptedData;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to process preprocessor {MapperVariables.Preprocessor}.", ex);
                }
            }
            else if (MapperVariables.Preprocessor != null && MapperVariables.Preprocessor.Contains("data_block_fa7545e6"))
            {
                try
                {
                    if (MapperVariables.Address == null) { throw new Exception($"Property {Path} does not have a MapperVariables.Address."); }

                    var baseAddress = (MemoryAddress)MapperVariables.Address;
                    var offset = MapperVariables.Preprocessor.GetIntParameterFromFunction(0);

                    var preprocessorResult = Preprocessor_fa7545e6.Read(baseAddress, driverResult.GetAddressData(baseAddress, 236), offset, MapperVariables.Length);
                    address = preprocessorResult.Address;
                    rawBytes = preprocessorResult.EncryptedData;
                    bytes = preprocessorResult.DecryptedData;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to process preprocessor {MapperVariables.Preprocessor}.", ex);
                }
            }
            else if (MapperVariables.Preprocessor != null && MapperVariables.Preprocessor.Contains("dma_967d10cc"))
            {
                try
                {
                    var memoryAddress = MapperVariables.Preprocessor.GetMemoryAddressFromFunction(0);
                    var offset = MapperVariables.Preprocessor.GetIntParameterFromFunction(1);

                    var memoryBlock = driverResult.GetResultWithinRange(memoryAddress);
                    if (memoryBlock == null)
                    {
                        throw new Exception($"Unable to retrieve memory block for property {Path} at address {memoryAddress.ToHexdecimalString()}.");
                    }

                    var dmaAddress = Preprocessor_967d10cc.Read(memoryAddress, size: 4, offset, memoryBlock);
                    if (dmaAddress == null) { return EmptyPropertyValueResult; }

                    address = dmaAddress;
                    bytes = driverResult.GetAddressData((uint)address, MapperVariables.Length);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Unable to process preprocessor {MapperVariables.Preprocessor}.", ex);
                }
            }
            else if (MapperVariables.Address != null)
            {
                address = MapperVariables.Address;
                bytes = driverResult.GetAddressData((uint)address, MapperVariables.Length);
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
                    "nibble" => NibbleTransformer.ToValue(bytes, (NibblePosition)(MapperVariables.Position ?? throw new Exception("Missing property variable: Position"))),
                    "int" => IntegerTransformer.ToValue(bytes, GameHookInstance.PlatformOptions.EndianType),
                    "string" => StringTransformer.ToValue(bytes, Glossary ?? throw new Exception("ReferenceList returned NULL")),
                    "string2" => String2Transformer.ToValue(bytes, Glossary ?? throw new Exception("ReferenceList returned NULL")),
                    "uint" => UnsignedIntegerTransformer.ToValue(bytes, GameHookInstance.PlatformOptions.EndianType),
                    _ => throw new Exception($"Unknown type defined for {Path}, {Type}")
                };

                if (string.IsNullOrEmpty(MapperVariables.PostprocessorReader) == false)
                {
                    var postprocessorExpression = new Expression(MapperVariables.PostprocessorReader);

                    postprocessorExpression.Parameters["x"] = value;

                    postprocessorExpression.EvaluateFunction += delegate (string name, FunctionArgs args)
                    {
                        if (name == "BitRange")
                            args.Result = NCalcFunctions.ReadBitRange((int)args.Parameters[0].Evaluate(), (int)args.Parameters[1].Evaluate(), (int)args.Parameters[2].Evaluate());
                    };

                    // TODO: We probably shouldn't hardcode int32 here -- probably should be dependent on the platform?
                    value = Convert.ToInt32(postprocessorExpression.Evaluate());
                }

                if ((Type == "bit" || Type == "bool" || Type == "int" || Type == "uint") && string.IsNullOrEmpty(MapperVariables.Reference) == false)
                {
                    value = ReferenceTransformer.ToValue(Convert.ToUInt64(value), Glossary ?? throw new Exception("ReferenceList returned NULL"));
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

        public async Task<byte[]> WriteValue(string? value, bool? freeze)
        {
            if (IsReadOnly)
            {
                throw new Exception($"Property '{Path}' is read-only and cannot be modified.");
            }

            if (GameHookInstance == null || GameHookInstance.PlatformOptions == null)
            {
                throw new Exception("GameHookInstance is not initalized properly.");
            }

            byte[]? bytes = null;

            if (string.IsNullOrEmpty(Reference) == false)
            {
                // We want to translate the reference found in the directory and then apply that.
                bytes = ReferenceTransformer.FromValue(value, Glossary ?? throw new Exception("ReferenceList returned NULL"));
            }
            else
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                var endianType = GameHookInstance.PlatformOptions.EndianType;

                bytes = Type switch
                {
                    "binaryCodedDecimal" => BinaryCodedDecimalTransformer.FromValue(int.Parse(value)),
                    "bitArray" => BitFieldTransformer.FromValue(value.Split(' ').Select(bool.Parse).ToArray()),
                    "bit" => BitTransformer.FromValue(Bytes ?? throw new Exception("Bytes is NULL."), MapperVariables.Position ?? throw new Exception("Position is NULL."), bool.Parse(value)),
                    "bool" => BooleanTransformer.FromValue(bool.Parse(value)),
                    "nibble" => NibbleTransformer.FromValue(int.Parse(value), Bytes ?? throw new Exception("Bytes is NULL."), (NibblePosition)(MapperVariables.Position ?? throw new Exception("Missing property variable: Position"))),
                    "int" => IntegerTransformer.FromValue(int.Parse(value), Length, endianType),
                    "string" => StringTransformer.FromValue(value, Length, Glossary ?? throw new Exception("ReferenceList returned NULL")),
                    "uint" => UnsignedIntegerTransformer.FromValue(uint.Parse(value), Length, endianType),
                    _ => throw new Exception($"Unknown type defined for {Path}, {Type}")
                };
            }

            if (string.IsNullOrEmpty(MapperVariables.PostprocessorWriter) == false)
            {
                var postprocessorExpression = new Expression(MapperVariables.PostprocessorWriter);

                postprocessorExpression.Parameters["x"] = bytes;
                postprocessorExpression.Parameters["y"] = Bytes;

                postprocessorExpression.EvaluateFunction += delegate (string name, FunctionArgs args)
                {
                    if (name == "BitRange")
                        args.Result = NCalcFunctions.WriteBitRange((byte[])args.Parameters[0].Evaluate(), (byte[])args.Parameters[1].Evaluate(), (int)args.Parameters[2].Evaluate(), (int)args.Parameters[3].Evaluate());
                };

                // TODO: We probably shouldn't hardcode int32 here -- probably should be dependent on the platform?
                bytes = (byte[])postprocessorExpression.Evaluate();
            }

            if (GameHookInstance.Driver == null) { throw new Exception("Driver is not defined."); }
            if (Address == null) { throw new Exception("Address is not defined."); }
            if (bytes == null) { throw new Exception("Bytes is not defined."); }

            await GameHookInstance.Driver.WriteBytes((uint)Address, bytes.Take(Length).ToArray());

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
            if (IsReadOnly)
            {
                throw new Exception($"Property '{Path}' is read-only and cannot be modified.");
            }

            if (GameHookInstance == null || GameHookInstance.PlatformOptions == null)
            {
                throw new Exception("GameHookInstance is not initalized properly.");
            }

            if (Address == null)
            {
                throw new Exception($"Property '{Path}' address is NULL.");
            }

            if (MapperVariables.Preprocessor != null && MapperVariables.Preprocessor.Contains("data_block_a245dcac"))
            {
                var baseAddress = MapperVariables.Address ?? throw new Exception($"Property {Path} does not have a base address.");
                var dataBlock = GameHookInstance.PreprocessorCache?.data_block_a245dcac[baseAddress] ?? throw new Exception($"Unable to retrieve data_block_a245dcac for property {Path} and address {Address?.ToHexdecimalString()}.");

                var writeResults = Preprocessor_a245dcac.Write((uint)Address, bytes, dataBlock);
                foreach (var result in writeResults)
                {
                    await GameHookInstance.GetDriver().WriteBytes(result.Address, result.Bytes);
                }
            }
            else if (MapperVariables.Preprocessor != null && MapperVariables.Preprocessor.Contains("data_block_fa7545e6"))
            {
                throw new NotImplementedException();
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
                await notifier.SendPropertyChanged(this, new string[] { "frozen" });
            }
        }

        public async Task UnfreezeProperty()
        {
            BytesFrozen = null;

            foreach (var notifier in GameHookInstance.ClientNotifiers)
            {
                await notifier.SendPropertyChanged(this, new string[] { "frozen" });
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
