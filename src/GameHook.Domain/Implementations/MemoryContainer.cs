using GameHook.Domain.Interfaces;

namespace GameHook.Domain.Implementations
{
    public class MemoryContainer : IMemoryContainer
    {
        private Dictionary<MemoryAddress, byte[]> MemoryLocations { get; }

        public MemoryContainer()
        {
            MemoryLocations = new Dictionary<MemoryAddress, byte[]>();
        }

        public MemoryAddress GetRelativeAddress(MemoryAddress absoluteAddress)
        {
            return 0x00;
        }

        public MemoryAddress GetAbsoluteAddress(MemoryAddress relativeAddress)
        {
            return 0x00;
        }

        public byte[] GetBytes(MemoryAddress address, int length)
        {
            var baseAddress = (uint)0x00;
            return MemoryLocations[baseAddress][(int)(baseAddress - address)..length].ToArray();
        }

        public ulong GET_DATA8_LE(MemoryAddress address)
        {
            throw new NotImplementedException();
        }

        public ulong GET_DATA8_BE(MemoryAddress address)
        {
            throw new NotImplementedException();
        }

        public ulong GET_DATA16_LE(MemoryAddress address)
        {
            throw new NotImplementedException();
        }

        public ulong GET_DATA16_BE(MemoryAddress address)
        {
            throw new NotImplementedException();
        }

        public ulong GET_DATA32_LE(MemoryAddress address)
        {
            throw new NotImplementedException();
        }

        public ulong GET_DATA32_BE(MemoryAddress address)
        {
            throw new NotImplementedException();
        }
    }
}
