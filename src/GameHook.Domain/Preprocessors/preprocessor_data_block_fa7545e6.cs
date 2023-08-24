// G4 - Encrypted block-shuffling.

namespace GameHook.Domain.Preprocessors
{
    public static class Preprocessor_fa7545e6
    {
        public class ReadResult
        {
            public MemoryAddress Address { get; init; }

            public byte[] EncryptedData { get; init; } = Array.Empty<byte>();
            public byte[] DecryptedData { get; init; } = Array.Empty<byte>();
        }

        public class WriteResult
        {
            public MemoryAddress Address { get; init; }
            public byte[] Bytes { get; init; } = Array.Empty<byte>();
        }

        private static byte[]? DecryptedDataCache { get; set; } = null;

        static uint PrngNext(ref uint prngSeed)
        {
            prngSeed = (0x41C64E6D * prngSeed + 0x6073);
            return (prngSeed >> 16) & 0xFFFF;
        }

        static uint DATA16_LE(byte[] data, int offset)
        {
            return (uint)((data[offset] << 0) | (data[offset + 1] << 8));
        }

        static uint DATA32_LE(byte[] data, int offset)
        {
            return (uint)((data[offset] << 0) | (data[offset + 1] << 8) | (data[offset + 2] << 16) | (data[offset + 3] << 24));
        }

        public static ReadResult Read(MemoryAddress startingAddress, byte[] encryptedData, int offset, int size)
        {
            byte[] decryptedData = new byte[encryptedData.Length];

            // first 8 bytes are not encrypted
            for (int i = 0; i < 8; i++)
            {
                decryptedData[i] = encryptedData[i];
            }

            var pid = DATA32_LE(encryptedData, 0x00);
            var checksum = DATA16_LE(encryptedData, 0x06);

            // decrypt blocks
            uint prngSeed = checksum;
            for (int i = 0x08; i < 0x88; i += 2)
            {
                var key = PrngNext(ref prngSeed);
                var data = DATA16_LE(encryptedData, i) ^ key;
                decryptedData[i + 0] = (byte)(data & 0xFF);
                decryptedData[i + 1] = (byte)(data >> 8);
            }

            // unshuffle blocks
            var shuffleId = ((pid & 0x3E000) >> 0xD) % 24;
            var shuffleOrder = shuffleId switch
            {
                0 => new int[4] { 0, 1, 2, 3 },
                1 => new int[4] { 0, 1, 3, 2 },
                2 => new int[4] { 0, 2, 1, 3 },
                3 => new int[4] { 0, 3, 1, 2 },
                4 => new int[4] { 0, 2, 3, 1 },
                5 => new int[4] { 0, 3, 2, 1 },
                6 => new int[4] { 1, 0, 2, 3 },
                7 => new int[4] { 1, 0, 3, 2 },
                8 => new int[4] { 2, 0, 1, 3 },
                9 => new int[4] { 3, 0, 1, 2 },
                10 => new int[4] { 2, 0, 3, 1 },
                11 => new int[4] { 3, 0, 2, 1 },
                12 => new int[4] { 1, 2, 0, 3 },
                13 => new int[4] { 1, 3, 0, 2 },
                14 => new int[4] { 2, 1, 0, 3 },
                15 => new int[4] { 3, 1, 0, 2 },
                16 => new int[4] { 2, 3, 0, 1 },
                17 => new int[4] { 3, 2, 0, 1 },
                18 => new int[4] { 1, 2, 3, 0 },
                19 => new int[4] { 1, 3, 2, 0 },
                20 => new int[4] { 2, 1, 3, 0 },
                21 => new int[4] { 3, 1, 2, 0 },
                22 => new int[4] { 2, 3, 1, 0 },
                23 => new int[4] { 3, 2, 1, 0 },
                _ => throw new Exception($"data_block_a245dcac returned a unknown substructure order.")
            };
            byte[] dataCopy = new byte[0x80];
            Array.Copy(decryptedData, 0x08, dataCopy, 0, 0x80);
            for (int i = 0; i < 4; i++)
            {
                Array.Copy(dataCopy, shuffleOrder[i] * 0x20, decryptedData, 0x08 + i * 0x20, 0x20);
            }

            // decrypt battle stats
            prngSeed = pid;
            for (int i = 0x88; i < 0x9C; i += 2)
            {
                var key = PrngNext(ref prngSeed);
                var data = (DATA16_LE(encryptedData, i) ^ key);
                decryptedData[i + 0] = (byte)(data & 0xFF);
                decryptedData[i + 1] = (byte)(data >> 16);
            }

            return new ReadResult()
            {
                Address = startingAddress + (uint)offset,
                EncryptedData = encryptedData[offset..(offset + size)],
                DecryptedData = decryptedData[offset..(offset + size)]
            };
        }

        public static void ClearCache()
        {
            DecryptedDataCache = null;
        }

        public static IEnumerable<WriteResult> Write(MemoryAddress startingAddress, int offset, byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}
