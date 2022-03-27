using GameHook.Domain.GameHookProperties;

namespace GameHook.Domain.Interfaces
{
    public interface IGameHookProperty
    {
        public string Identifier { get; }

        PropertyFields Fields { get; }
        string Type { get; }
        MemoryAddress StartingAddress { get; }
        int Length { get; }

        object? Value { get; }
        byte[] Bytes { get; }
        bool Frozen { get; }
        byte[]? FreezeToBytes { get; }

        Task WriteBytes(byte[] values, bool? freeze);
        void Unfreeze();

        /// <summary>
        /// This should only be called by the driver!
        /// This updates the internal state of the property.
        /// This will not send data to the driver.
        /// Use WriteBytes() for that.
        /// </summary>
        void OnDriverMemoryChanged(byte[] bytes);
    }
}