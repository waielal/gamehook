using Microsoft.Extensions.Configuration;
using System.Data;

namespace GameHook.Domain
{
    public static class Extensions
    {
        public static string ToHexdecimalString(this MemoryAddress value) => $"0x{value:X2}";
        public static string ToHexdecimalString(this byte value) => ((uint)value).ToHexdecimalString();

        public static IEnumerable<int> ToIntegerArray(this byte[] bytes) => bytes.Select(x => (int)x).ToArray();

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
    }
}
