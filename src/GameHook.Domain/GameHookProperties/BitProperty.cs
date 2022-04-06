using GameHook.Domain.Interfaces;
using System.Collections;

namespace GameHook.Domain.GameHookProperties
{
    public class BitProperty : GameHookProperty<bool>
    {
        // Position is 1-index-based where the actual Get() requires 0-based-index.
        public int Position => Fields.Position ?? throw new Exception($"Property did not define field {nameof(Fields.Position)}");

        public BitProperty(IGameHookContainer mapper, string identifier, PropertyFields fields)
            : base(mapper, identifier, fields)
        {
        }

        protected override byte[] FromValue(bool value)
        {
            return value == true ? new byte[] { 0x01 } : new byte[] { 0x00 };
        }

        protected override bool ToValue(byte[] bytes)
        {
            return new BitArray(bytes).Get(Position - 1);
        }
    }
}