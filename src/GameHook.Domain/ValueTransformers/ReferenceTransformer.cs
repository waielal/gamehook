namespace GameHook.Domain.ValueTransformers
{
    public static class ReferenceHelper
    {
        public static GlossaryItem? SingleOrDefaultByKey(this IEnumerable<GlossaryItem> glossaryItems, ulong data)
        {
            return glossaryItems.SingleOrDefault(x => x.Key == data);
        }

        public static GlossaryItem? FirstOrDefaultByValue(this IEnumerable<GlossaryItem> glossaryItems, string? value)
        {
            if (value == null)
            {
                return glossaryItems.FirstOrDefault(x => x.Value == null);
            }

            return glossaryItems.FirstOrDefault(x => x.Value?.ToString() == value);
        }
    }

    public static class ReferenceTransformer
    {
        public static byte[]? FromValue(string? value, IEnumerable<GlossaryItem> glossaryItems)
        {
            var key = glossaryItems.FirstOrDefaultByValue(value)?.Key;

            if (key == null) return null;
            else return BitConverter.GetBytes(key ?? throw new Exception($"BitConverter cannot convert {key} to a byte array."));
        }

        public static object? ToValue(ulong data, IEnumerable<GlossaryItem> glossaryItems)
        {
            return glossaryItems.SingleOrDefaultByKey(data)?.Value;
        }
    }
}
