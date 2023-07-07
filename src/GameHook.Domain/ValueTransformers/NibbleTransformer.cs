using System.ComponentModel.DataAnnotations;

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

        public static byte[] FromValue(int value, byte[] nibble, NibblePosition position)
        {
            if (value < 0)
            {
                throw new ValidationException("Nibble cannot be less than 0.");
            }

            if (value > 15)
            {
                throw new ValidationException("Nibble cannot be greater than 15.");
            }

            var result = position switch
            {
                NibblePosition.MostSignificant => (byte)((value & 0x0F) | (nibble[0] << 4)),
                NibblePosition.LeastSignificant => (byte)((value & 0xF0) | (nibble[0] & 0x0F)),
                _ => throw new Exception($"Unknown Nibble position supplied: {position}.")
            };

            return new byte[] { result };
        }

        public static int ToValue(byte[] value, NibblePosition position)
        {
            return position switch
            {
                NibblePosition.MostSignificant => (byte)((value[0] & 0xF0) >> 4),
                NibblePosition.LeastSignificant => (byte)(value[0] & 0x0F),
                _ => throw new Exception($"Unknown Nibble position supplied: {position}.")
            };
        }
    }
}