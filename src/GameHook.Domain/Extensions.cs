using Microsoft.Extensions.Configuration;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GameHook.Domain
{
    public static class Extensions
    {
        public static string ToHexdecimalString(this MemoryAddress value) => $"0x{value:X2}";
        public static string ToHexdecimalString(this byte value) => ((uint)value).ToHexdecimalString();
        public static string ToHexdecimalString(this byte[] value) => $"{string.Join(' ', value.Select(x => x.ToHexdecimalString()))}";

        public static IEnumerable<int> ToIntegerArray(this byte[] bytes) => bytes.Select(x => (int)x).ToArray();

        public static string ToPascalCase(this string str)
        {
            string[] words = Regex.Split(str, @"[_\-]");
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (!string.IsNullOrEmpty(word))
                {
                    words[i] = textInfo.ToTitleCase(word);
                }
            }

            return string.Join("", words);
        }

        public static byte[] ReverseBytesIfLE(this byte[] bytes, EndianTypes? endianType)
        {
            if (endianType == null || bytes.Length == 1) { return bytes; }

            if (endianType == EndianTypes.LittleEndian)
            {
                var workingBytes = (byte[])bytes.Clone();

                Array.Reverse(workingBytes);

                return workingBytes;
            }

            return bytes;
        }

        public static string GetRequiredValue(this IConfiguration configuration, string key)
        {
            var value = configuration[key];

            if (value == null) throw new Exception($"Configuration '{key}' is missing from appsettings.json");
            if (string.IsNullOrWhiteSpace(value)) throw new Exception($"Configuration '{key}' is empty.");

            return value;
        }

        public static bool Between(this MemoryAddress value, MemoryAddress min, MemoryAddress max)
        {
            return value >= min && value <= max;
        }

        public static MemoryAddressBlockResult? GetResultWithinRange(this IEnumerable<MemoryAddressBlockResult> blocks, MemoryAddress address)
        {
            return blocks.SingleOrDefault(x => address >= x.StartingAddress && address <= x.EndingAddress);
        }

        public static byte[] GetRelativeAddress(this MemoryAddressBlockResult block, MemoryAddress memoryAddress, int length)
        {
            var startingOffset = (int)(memoryAddress - block.StartingAddress);
            var endingOffset = startingOffset + length;

            return block.Data[startingOffset..endingOffset];
        }

        public static byte[]? GetAddressData(this IEnumerable<MemoryAddressBlockResult> blocks, uint address, int length)
        {
            return GetResultWithinRange(blocks, address)?.GetRelativeAddress(address, length);
        }

        public static MemoryAddress ToMemoryAddress(this string memoryAddress)
        {
            if (MemoryAddress.TryParse(memoryAddress, out var result)) { return result; }
            throw new Exception($"Unable to determine memory address from string {memoryAddress}. It must be in decimal form (not hexdecimal).");
        }

        public static int GetIntParameterFromFunction(this string function, int position)
        {
            return int.Parse(function.Between("(", ")").Split(",")[position]);
        }

        public static MemoryAddress GetMemoryAddressFromFunction(this string function, int position)
        {
            return function.Between("(", ")").Split(",")[position].ToMemoryAddress();
        }

        public static string Between(this string str, string firstString, string lastString)
        {
            int start = str.IndexOf(firstString) + firstString.Length;
            int end = str.IndexOf(lastString);
            return str.Substring(start, end - start);
        }

        public static async Task ForEachAsync<T>(this List<T> list, Func<T, Task> func)
        {
            foreach (var value in list) await func(value);
        }

        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input; // Return input as it is if it's null or empty
            }

            char firstChar = char.ToUpper(input[0]); // Convert the first character to uppercase
            string restOfString = input.Substring(1); // Get the remaining characters of the string

            return firstChar + restOfString;
        }
    }
}
