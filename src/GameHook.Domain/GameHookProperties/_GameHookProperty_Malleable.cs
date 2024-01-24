using GameHook.Domain.Interfaces;

namespace GameHook.Domain.GameHookProperties
{
    public abstract partial class GameHookProperty : IGameHookProperty
    {
        private string? _memoryContainer { get; set; }
        private string? _address { get; set; }
        private int? _length { get; set; }
        private int? _size { get; set; }
        private int? _bit { get; set; }
        private string? _nibble { get; set; }
        private string? _reference { get; set; }
        private string? _description { get; set; }
        private object? _value { get; set; }
        private byte[]? _bytes { get; set; }
        private byte[]? _bytesFrozen { get; set; }

        public string? MemoryContainer
        {
            get { return _memoryContainer; }
            set
            {
                if (value == _memoryContainer) { return; }

                FieldsChanged.Add("memoryContainer");
                _memoryContainer = value;
            }
        }

        public string? Address
        {
            get { return _address; }
            set
            {
                if (value == _address) { return; }

                _address = value;

                ComputedAddress = null;

                IsMemoryAddressSolved = AddressMath.TrySolve(value, new Dictionary<string, object?>(), out var solvedAddress);

                if (IsMemoryAddressSolved == false)
                {
                    ComputedAddress = null;
                }
                else
                {
                    ComputedAddress = solvedAddress;
                }

                FieldsChanged.Add("address");
            }
        }

        public int? Length
        {
            get => _length;
            set
            {
                if (_length == value) return;

                FieldsChanged.Add("length");
                _length = value;
            }
        }

        public int? Size
        {
            get => _size;
            set
            {
                if (_size == value) return;

                FieldsChanged.Add("size");
                _size = value;
            }
        }

        public string? Nibble
        {
            get => _nibble;
            set
            {
                if (_nibble == value) return;

                FieldsChanged.Add("nibble");
                _nibble = value;
            }
        }

        public int? Bit
        {
            get => _bit;
            set
            {
                if (_bit == value) return;

                FieldsChanged.Add("bit");
                _bit = value;
            }
        }

        public string? Reference
        {
            get => _reference;
            set
            {
                if (_reference == value) return;

                FieldsChanged.Add("reference");
                _reference = value;
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                if (_description == value) return;

                FieldsChanged.Add("description");
                _description = value;
            }
        }

        public byte[]? Bytes
        {
            get => _bytes;
            private set
            {
                if (_bytes != null && value != null && _bytes.SequenceEqual(value)) return;

                FieldsChanged.Add("bytes");
                _bytes = value;
            }
        }

        public byte[]? BytesFrozen
        {
            get => _bytesFrozen;
            private set
            {
                if (_bytesFrozen != null && value != null && _bytesFrozen.SequenceEqual(value)) return;

                FieldsChanged.Add("frozen");
                _bytesFrozen = value;
            }
        }

        public object? Value
        {
            get => _value;
            set
            {
                if (_value != null && _value.Equals(value)) return;

                FieldsChanged.Add("value");
                _value = value;
            }
        }
    }
}
