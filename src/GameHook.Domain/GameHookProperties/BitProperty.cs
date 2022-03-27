using GameHook.Domain.Interfaces;
using System.Collections;

namespace GameHook.Domain.GameHookProperties
{
    public class BitProperty : GameHookProperty<bool>
    {
        public int Index => Fields.Index ?? throw new Exception("Undefined field: Index");

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
            if (PlatformOptions.EndianType == EndianTypeEnum.LittleEndian)
            {
                Array.Reverse(bytes);
            }

            return new BitArray(bytes).Get(Index);
        }
    }
}