namespace GameHook.Domain.ValueTransformers
{
    public static class ReferenceHelper
    {
        public static GlossaryListItem? SingleOrDefaultByKey(this GlossaryList glossaryItems, ulong data)
        {
            return glossaryItems.Values.SingleOrDefault(x => x.Key == data);
        }

        public static GlossaryListItem? FirstOrDefaultByValue(this GlossaryList glossaryItems, string? value)
        {
            if (value == null)
            {
                return glossaryItems.Values.FirstOrDefault(x => x.Value == null);
            }

            return glossaryItems.Values.FirstOrDefault(x => x.Value?.ToString() == value);
        }
    }

    public static class ReferenceTransformer
    {
        public static byte[]? FromValue(string? value, GlossaryList referenceList)
        {
            var key = referenceList.FirstOrDefaultByValue(value)?.Key;

            if (key == null) return null;
            else return BitConverter.GetBytes(key ?? throw new Exception($"BitConverter cannot convert {key} to a byte array."));
        }

        public static object? ToValue(ulong data, GlossaryList referenceList)
        {
            return referenceList.SingleOrDefaultByKey(data)?.Value;
        }
    }
}
