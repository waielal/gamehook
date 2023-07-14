namespace GameHook.Domain.ValueTransformers
{
    public static class IntegerTransformer
    {
        public static byte[] FromValue(int value, int length, EndianTypes endianType)
        {
            var bytes = BitConverter.GetBytes(value).Take(length).ToArray();
            return bytes.ReverseBytesIfLE(endianType);
        }

        public static int ToValue(byte[] data, EndianTypes endianType)
        {
            // TODO: Integers cannot currently exceed int32
            byte[] value = new byte[8];
            Array.Copy(data.ReverseBytesIfLE(endianType), value, data.Length);
            return BitConverter.ToInt32(value, 0);
        }
    }
}
