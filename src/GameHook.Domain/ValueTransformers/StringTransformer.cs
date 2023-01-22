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

            return uints.Select(x => x.ToHexdecimalString().FromHexdecimalStringToByte()).ToArray();
        }

        public static string ToValue(byte[] data, IEnumerable<GlossaryItem> glossaryItems)
        {
            var results = new List<string?>();

            var values = data.Select(b => glossaryItems.SingleOrDefaultByKey(new byte[] { b })?.Value as string);

            // If the returned values array is empty,
            // then there's nothing to do, return an empty string.
            if (values.Any() == false)
            {
                return string.Empty;
            }

            foreach (var charItem in values)
            {
                if (charItem == null)
                {
                    // This is likely a non-displayable character.

                    break;
                }

                // Add the character to the string buffer.
                results.Add(charItem);
            }

            // Return the completed string buffer.
            return string.Join(string.Empty, results);
        }
    }
}
