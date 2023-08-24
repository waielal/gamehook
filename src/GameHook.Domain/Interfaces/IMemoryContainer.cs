namespace GameHook.Domain.Interfaces
{
    public interface IMemoryContainer
    {
        public byte[] GetBytes(MemoryAddress address, int length);

        public ulong GET_DATA8_LE(MemoryAddress address);
        public ulong GET_DATA8_BE(MemoryAddress address);

        public ulong GET_DATA16_LE(MemoryAddress address);
        public ulong GET_DATA16_BE(MemoryAddress address);

        public ulong GET_DATA32_LE(MemoryAddress address);
        public ulong GET_DATA32_BE(MemoryAddress address);
    }
}
