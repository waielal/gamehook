namespace GameHook.Domain.ValueTransformers
{
    public static class StringTransformer
    {
        public static byte[] FromValue(string value, int size, IEnumerable<GlossaryItem> glossaryItems)
        {
            var uints = value.ToCharArray()
                .Select(x => glossaryItems.FirstOrDefaultByValue(x.ToString()))
                .Select(x => (x ?? glossaryItems.First()).Key)
                .ToList();

            if (uints.Count + 1 > size)
            {
                uints = uints.Take(size - 1).ToList();
            }

            var nullTerminationKey = glossaryItems.FirstOrDefaultByValue(null)?.Key;
            if (nullTerminationKey != null)
            {
                uints.Add(nullTerminationKey ?? throw new Exception("NullTerminationKey is NULL."));
            }

            return uints.Select(x => (byte)x).ToArray();
        }

        public static string ToValue(byte[] data, IEnumerable<GlossaryItem> glossaryItems)
        {
            var results = data.Select(b =>
            {
                var glossaryItem = glossaryItems.SingleOrDefault(x => x.Key == b);
                return glossaryItem?.Value?.ToString() ?? null;
            });

            // Return the completed string buffer.
            return string.Join(string.Empty, results.TakeWhile(s => s != null));
        }
    }
}
