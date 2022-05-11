using GameHook.Domain.Interfaces;

namespace GameHook.Domain.GameHookProperties
{
    public class StringProperty : GameHookProperty<string>
    {
        private string GlossaryName { get; }
        private IEnumerable<GlossaryItem> GlossaryItems { get; }

        public StringProperty(IGameHookContainer mapper, string identifier, PropertyFields fields)
            : base(mapper, identifier, fields)
        {
            GlossaryName = fields.Reference ?? "defaultCharacterMap";
            GlossaryItems = mapper.Glossary[GlossaryName] ?? throw new Exception($"Could not load glossary items for {GlossaryName}.");
        }

        protected override byte[] FromValue(string? value)
        {
            if (value == null) { return Enumerable.Repeat((byte)0x00, Size).ToArray(); }

            var uints = value.ToCharArray()
                .Select(x => ReferenceArrayHelper.FirstOrDefaultByValue(Logger, GlossaryName, GlossaryItems, x.ToString()))
                .Select(x => x.Key);

            return uints.Select(x => x.ToHexdecimalString().FromHexdecimalStringToByte()).ToArray();
        }

        protected override string ToValue(byte[] bytes)
        {
            var results = new List<string?>();

            var values = bytes.Select(b => ReferenceArrayHelper.SingleOrDefaultByKey(Logger, Address, GlossaryName, GlossaryItems, new byte[] { b })?.Value as string);

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

        public override string ToString()
        {
            return Value ?? string.Empty;
        }
    }
}