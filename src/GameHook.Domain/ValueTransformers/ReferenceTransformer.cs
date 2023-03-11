namespace GameHook.Domain.ValueTransformers
{
    public static class ReferenceHelper
    {
        public static GlossaryItem? SingleOrDefaultByKey(this IEnumerable<GlossaryItem> glossaryItems, int data)
        {
            return glossaryItems.SingleOrDefault(x => x.Key == data);
        }

        public static GlossaryItem? SingleOrDefaultByKey(this IEnumerable<GlossaryItem> glossaryItems, byte[] bytes)
        {
            var convertedValue = UnsignedIntegerTransformer.ToValue(bytes);
            return glossaryItems.SingleOrDefault(x => x.Key == convertedValue);
        }

        public static GlossaryItem? FirstOrDefaultByValue(this IEnumerable<GlossaryItem> glossaryItems, string? value)
        {
            var item = glossaryItems.FirstOrDefault(x => x.Value?.ToString() == value);

            if (item == null)
            {
                return null;
            }

            return item;
        }
    }

    public static class ReferenceTransformer
    {
        public static byte[]? FromValue(string value, int length, IEnumerable<GlossaryItem> glossaryItems)
        {
            var key = glossaryItems.FirstOrDefaultByValue(value)?.Key;

            if (key == null) return null;
            else return UnsignedIntegerTransformer.FromValue(key ?? 0, length);
        }

        public static object? ToValue(int data, IEnumerable<GlossaryItem> glossaryItems)
        {
            return glossaryItems.SingleOrDefaultByKey(data)?.Value;
        }
    }
}
