using GameHook.Domain.Interfaces;
using System.Collections;

namespace GameHook.Domain.GameHookProperties
{
    public class BitProperty : GameHookProperty<bool>
    {
        // Position is 0-7 index based,
        public int Position => Fields.Position ?? throw new Exception($"Property did not define field {nameof(Fields.Position)}");

        public BitProperty(IGameHookContainer mapper, string identifier, PropertyFields fields)
            : base(mapper, identifier, fields)
        {
        }

        protected override byte[] FromValue(bool value)
        {
            var bitArray = new BitArray(Bytes);
            bitArray.Set(Position, value);

            byte[] bytes = new byte[1];
            bitArray.CopyTo(bytes, 0);

            return bytes;
        }

        protected override bool ToValue(byte[] bytes)
        {
            return new BitArray(bytes).Get(Position);
        }
    }
}