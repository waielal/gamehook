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
        public static byte[] FromValue(int value, byte[] bytes, NibblePosition position)
        {
            if (value < 0)
            {
                throw new ValidationException("Nibble cannot be less than 0.");
            }

            if (value > 15)
            {
                throw new ValidationException("Nibble cannot be greater than 15.");
            }

            if (bytes == null || bytes.Length == 0)
            {
                throw new Exception("Byte array cannot be null or empty.");
            }

            byte newValue = (byte)(value & 0x0F);
            byte targetNibble = (byte)(bytes[0] & (position == NibblePosition.MostSignificant ? 0x0F : 0xF0));

            if (position == NibblePosition.MostSignificant)
            {
                bytes[0] = (byte)((newValue << 4) | targetNibble);
            }
            else
            {
                bytes[0] = (byte)(targetNibble | newValue);
            }

            return bytes;
        }

        public static int ToValue(byte[] value, NibblePosition position)
        {
            if (position == NibblePosition.LeastSignificant)
            {
                return value[0] & 0x0F;
            }
            else if (position == NibblePosition.MostSignificant)
            {
                return (value[0] >> 4) & 0x0F;
            }
            else
            {
                throw new Exception($"Unknown nibble position supplied: {position}");
            }
        }
    }
}