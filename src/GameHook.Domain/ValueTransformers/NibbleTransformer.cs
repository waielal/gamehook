namespace GameHook.Domain.ValueTransformers
{
    public enum NibblePosition
    {
        MostSignificant,
        LeastSignificant
    }

    public static class NibbleTransformer
    {
        public static NibblePosition ToNibblePosition(this int position)
        {
            return position switch
            {
                0 => NibblePosition.MostSignificant,
                1 => NibblePosition.LeastSignificant,
                _ => throw new Exception("Unable to determine from position.")
            };
        }
        
        public static byte FromValue(byte value, byte nibble, NibblePosition position)
        {
            return position switch
            {
                NibblePosition.MostSignificant => (byte)((value & 0x0F) | (nibble << 4)),
                NibblePosition.LeastSignificant => (byte)((value & 0xF0) | (nibble & 0x0F)),
                _ => throw new Exception($"Unknown NibblePosition supplied: {position}.")
            };
        }

        public static int ToValue(byte value, NibblePosition position)
        {
            return position switch
            {
                NibblePosition.MostSignificant => (byte)((value & 0xF0) >> 4),
                NibblePosition.LeastSignificant => (byte)(value & 0x0F),
                _ => throw new Exception($"Unknown NibblePosition supplied: {position}.")
            };
        }
    }
}