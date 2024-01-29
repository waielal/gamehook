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
            Bits = attributes.Bits;
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

            if (!string.IsNullOrEmpty(Bits))
            {
                if (Bits.Contains('-'))
                {
                    var parts = Bits.Split('-');

                    int start = int.Parse(parts[0]);
                    int end = int.Parse(parts[1]);

                    int bitCount = (end - start) + 1;

                    byte[] newBytes = new byte[bytes.Length];

                    for (int i = 0; i < bitCount; i++)
                    {
                        int byteIndex = (start + i) / 8; // Calculate the byte index.
                        int bitOffset = (start + i) % 8; // Calculate the bit offset within the byte.

                        // Set the appropriate bit in the newBytes array.
                        newBytes[byteIndex] |= (byte)(((bytes[byteIndex] >> bitOffset) & 1) << (i % 8));
                    }

                    bytes = newBytes;
                }
                else if (Bits.Contains(','))
                {
                    var indices = Bits.Split(',').Select(int.Parse);

                    foreach (int index in indices)
                    {
                        if (index >= 0 && index < bytes.Length * 8)
                        {
                            int byteIndex = index / 8;
                            int bitIndex = index % 8;

                            // Set the specified bit to 1
                            bytes[byteIndex] |= (byte)(1 << bitIndex);
                        }
                    }
                }
                else
                {
                    // Handling a single number
                    int index = int.Parse(Bits);

                    if (bytes == null || bytes.Length == 0 || index < 0 || index >= bytes.Length * 8)
                    {
                        throw new MapperException($"Bit {index} was outside of the bytes array length of {bytes?.Length ?? 0}.");
                    }

                    var byteIndex = index / 8;
                    var bitIndex = index % 8;

                    byte bit = (byte)((bytes[byteIndex] >> bitIndex) & 1);
                    bytes = [bit];
                }
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