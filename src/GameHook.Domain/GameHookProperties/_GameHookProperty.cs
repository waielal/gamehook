using GameHook.Domain.Interfaces;

namespace GameHook.Domain.GameHookProperties
{
    public abstract partial class GameHookProperty : IGameHookProperty
    {
        public GameHookProperty(IGameHookInstance instance, PropertyAttributes attributes)
        {
            Instance = instance;

            Path = attributes.Path;
            Type = attributes.Type;
            MemoryContainer = attributes.MemoryContainer;
            Address = attributes.Address;
            Length = attributes.Length;
            Size = attributes.Size;
            Bit = attributes.Bit;
            Nibble = attributes.Nibble;
            Reference = attributes.Reference;
            Description = attributes.Description;

            StaticValue = attributes.StaticValue;

            ReadFunction = attributes.ReadFunction;
            WriteFunction = attributes.WriteFunction;

            AfterReadValueExpression = attributes.AfterReadValueExpression;
        }

        protected IGameHookInstance Instance { get; }
        public string Path { get; }
        public string Type { get; }

        public string? StaticValue { get; }

        private MemoryAddress? ComputedAddress { get; set; }
        public ReferenceItems? ComputedReference
        {
            get
            {
                if (Instance.Mapper == null) throw new Exception("Instance.Mapper is NULL.");
                if (Reference == null) return null;

                return Instance.Mapper.References[Reference];
            }
        }

        private bool IsMemoryAddressSolved { get; set; }

        private bool ShouldRunReferenceTransformer
        {
            get { return (Type == "bit" || Type == "bool" || Type == "int" || Type == "uint") && Reference != null; }
        }

        public bool IsFrozen => BytesFrozen != null;
        public bool IsReadOnly => Address == null;

        protected abstract object? ToValue(byte[] bytes);
        protected abstract byte[] FromValue(string value);

        uint? IGameHookProperty.Address => ComputedAddress;

        public string? ReadFunction { get; }
        public string? WriteFunction { get; }
        public string? AfterReadValueExpression { get; }

        public HashSet<string> FieldsChanged { get; } = [];

        public void ProcessLoop(IMemoryManager memoryManager)
        {
            if (Instance == null) { throw new Exception("Instance is NULL."); }
            if (Instance.Mapper == null) { throw new Exception("Instance.Mapper is NULL."); }
            if (Instance.Driver == null) { throw new Exception("Instance.Driver is NULL."); }

            if (string.IsNullOrEmpty(ReadFunction) == false)
            {
                // They want to do it themselves entirely in Javascript.
                Instance.Evalulate(ReadFunction, this, null);

                return;
            }

            if (StaticValue != null)
            {
                Value = StaticValue;

                return;
            }

            MemoryAddress? address = ComputedAddress;

            if (string.IsNullOrEmpty(this._address) == false && IsMemoryAddressSolved == false)
            {
                if (AddressMath.TrySolve(this._address, Instance.Variables, out var solvedAddress))
                {
                    address = solvedAddress;
                }
                else
                {
                    // TODO: Write a log entry here.
                }
            }

            if (address == null)
            {
                // There is nothing to do for this property, as it does not have an address or bytes.
                // Hopefully a postprocessor will pick it up and set it's value!

                return;
            }

            byte[]? previousBytes = Bytes?.ToArray();
            byte[]? bytes = null;
            object? value;

            if (bytes == null)
            {
                if (address == null) { throw new Exception("address is NULL."); }
                if (Length == null) { throw new Exception("Length is NULL."); }

                bytes = memoryManager.Get(MemoryContainer, address ?? 0x00, Length ?? 0).Data;
            }

            if (previousBytes != null && bytes != null && previousBytes.SequenceEqual(bytes))
            {
                // Fast path - if the bytes match, then we can assume the property has not been
                // updated since last poll.

                // Do nothing, we don't need to calculate the new value as
                // the bytes are the same.

                return;
            }

            ComputedAddress = address;
            Bytes = bytes?.ToArray();

            if (bytes == null)
            {
                throw new Exception(
                    $"Unable to retrieve bytes for property '{Path}' at address {ComputedAddress?.ToHexdecimalString()}. Is the address within the drivers' memory address block ranges?");
            }

            if (bytes.Length == 0)
            {
                throw new Exception(
                  $"Unable to retrieve bytes for property '{Path}' at address {ComputedAddress?.ToHexdecimalString()}. A byte array length of zero was returned?");
            }

            if (string.IsNullOrEmpty(Nibble) == false)
            {
                if (bytes.Length != 1)
                {
                    throw new MapperException($"For property '{Path}', bytes returned a length of {bytes.Length}. Cannot perform a nibble operation on a length not equal to 1.");
                }

                if (Nibble == "high")
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = (byte)(bytes[i] >> 4);
                    }
                }
                else if (Nibble == "low")
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = (byte)(bytes[i] & 0x0F);
                    }
                }
                else
                {
                    throw new Exception($"Invalid nibble option provided: {Nibble}.");
                }
            }

            if (Bit != null)
            {
                if (bytes == null || bytes.Length == 0 || Bit < 0 || Bit >= bytes.Length * 8)
                {
                    throw new MapperException($"Bit {Bit} was outside of the bytes array length of {bytes?.Length ?? 0}.");
                }

                var byteIndex = Bit / 8 ?? 0;
                var bitIndex = Bit % 8 ?? 0;

                byte bit = (byte)((bytes[byteIndex] >> bitIndex) & 1);
                bytes = [bit];
            }

            if (address != null && BytesFrozen != null && bytes.SequenceEqual(BytesFrozen) == false)
            {
                // Bytes have changed, but property is frozen, so force the bytes back to the original value.
                // Pretend nothing has changed. :)

                _ = Instance.Driver.WriteBytes((MemoryAddress)address, BytesFrozen);

                return;
            }

            value = ToValue(bytes);

            if (string.IsNullOrEmpty(AfterReadValueExpression) == false)
            {
                value = Instance.Evalulate(AfterReadValueExpression, value, null);
            }

            // Reference lookup
            if (ShouldRunReferenceTransformer)
            {
                if (ComputedReference == null) throw new Exception("ReferenceObject is NULL.");

                value = ComputedReference.GetSingleOrDefaultByKey(Convert.ToUInt64(value))?.Value;
            }

            Value = value;
        }

        public async Task WriteValue(string value, bool? freeze)
        {
            if (string.IsNullOrEmpty(WriteFunction) == false)
            {
                // They want to do it themselves entirely in Javascript.
                Instance.Evalulate(WriteFunction, this, null);

                return;
            }

            if (Bytes == null)
            {
                throw new Exception("Bytes is NULL.");
            }

            byte[] bytes;

            if (ShouldRunReferenceTransformer)
            {
                if (ComputedReference == null) throw new Exception("Glossary is NULL.");

                bytes = BitConverter.GetBytes(ComputedReference.GetSingleByValue(value).Key);
            }
            else
            {
                bytes = FromValue(value);
            }

            if (Nibble != null)
            {
                if (Bytes == null)
                {
                    throw new Exception($"{Path}'s bytes are NULL, so we can't write a nibble if we don't know the other half.");
                }

                if (bytes.Length != 1)
                {
                    throw new MapperException($"For property '{Path}', attempted to write bytes with a length of {bytes.Length}. Cannot perform a nibble operation on a byte array length greater than 1.");
                }

                var workingBytes = Bytes.ToArray();

                for (int i = 0; i < workingBytes.Length && i < bytes.Length; i++)
                {
                    if (Nibble == "high")
                    {
                        workingBytes[i] = (byte)((workingBytes[i] & 0x0F) | (bytes[i] << 4));
                    }
                    else if (Nibble == "low")
                    {
                        workingBytes[i] = (byte)((workingBytes[i] & 0xF0) | (bytes[i] & 0x0F));
                    }
                }

                bytes = workingBytes;
            }

            if (Bit != null)
            {
                if (Bytes == null)
                {
                    throw new Exception($"{Path}'s bytes are NULL, so we can't write a nibble if we don't know the other half.");
                }

                var workingBytes = Bytes.ToArray();

                int byteIndex = (Bit ?? 0) / 8;
                int bitIndex = (Bit ?? 0) % 8;

                byte mask = (byte)(1 << bitIndex);
                workingBytes[byteIndex] &= (byte)~mask;
                workingBytes[byteIndex] |= (byte)((bytes[0] & 1) << bitIndex);

                bytes = workingBytes;
            }

            await WriteBytes(bytes, freeze);
        }

        public async Task WriteBytes(byte[] bytesToWrite, bool? freeze)
        {
            if (Instance == null) throw new Exception("Instance is NULL.");
            if (Instance.Driver == null) throw new Exception("Driver is NULL.");

            if (ComputedAddress == null) throw new Exception($"{Path} does not have an address. Cannot write data to an empty address.");
            if (Length == null) throw new Exception($"{Path}'s length is NULL, so we can't write bytes.");

            var bytes = new byte[Length ?? 1];

            // Overlay the bytes onto the buffer.
            // This ensures that we can't overflow the property.
            // It also ensures it can't underflow the property, it copies the remaining from Bytes.
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i < bytesToWrite.Length) bytes[i] = bytesToWrite[i];
                else if (Bytes != null) bytes[i] = Bytes[i];
            }

            if (freeze == true)
            {
                // The property is frozen, but we want to write bytes anyway.
                // So this should replace the existing frozen bytes.

                BytesFrozen = bytes;
            }

            if (bytes.Length != Length)
            {
                throw new Exception($"Something went wrong with attempting to write bytes for {Path}. The bytes to write and the length of the property do not match. Will not proceed.");
            }

            await Instance.Driver.WriteBytes((MemoryAddress)ComputedAddress, bytes);

            if (freeze == true) await FreezeProperty(bytes);
            else if (freeze == false) await UnfreezeProperty();
        }

        public async Task FreezeProperty(byte[] bytesFrozen)
        {
            BytesFrozen = bytesFrozen;

            var propertyArray = new IGameHookProperty[] { this };
            foreach (var notifier in Instance.ClientNotifiers)
            {
                await notifier.SendPropertiesChanged(propertyArray);
            }
        }

        public async Task UnfreezeProperty()
        {
            BytesFrozen = null;

            var propertyArray = new IGameHookProperty[] { this };
            foreach (var notifier in Instance.ClientNotifiers)
            {
                await notifier.SendPropertiesChanged(propertyArray);
            }
        }
    }
}