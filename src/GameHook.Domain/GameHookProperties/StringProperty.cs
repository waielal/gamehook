using GameHook.Domain.Interfaces;

namespace GameHook.Domain.GameHookProperties
{
    public class StringProperty : GameHookProperty<string>
    {
        private string GlossaryPageName { get; }
        private GameHookGlossaryPage GlossaryPage { get; }

        public StringProperty(IGameHookContainer mapper, string identifier, PropertyFields fields)
            : base(mapper, identifier, fields)
        {
            GlossaryPageName = fields.Reference ?? "defaultCharacterMap";
            GlossaryPage = mapper.Glossary[GlossaryPageName] ?? new Dictionary<byte, dynamic>();
        }

        protected override byte[] FromValue(string? value)
        {
            if (value == null) { return Array.Empty<byte>(); }

            throw new NotImplementedException();
        }

        protected override string ToValue(byte[] bytes)
        {
            var results = new List<string?>();

            var values = ReferenceArrayHelper.GetFromGlossary<string?>(Logger, StartingAddress, GlossaryPageName, GlossaryPage, bytes, true);

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