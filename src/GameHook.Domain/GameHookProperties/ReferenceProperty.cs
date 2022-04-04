using GameHook.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameHook.Domain.GameHookProperties
{
    public static class ReferenceArrayHelper
    {
        public static IEnumerable<T?> GetFromGlossary<T>(ILogger logger, MemoryAddress? memoryAddress, string glossaryPageName, GameHookGlossaryPage glossaryPage, byte[] bytes, bool returnOnNull = false)
        {
            var translatedArray = new List<T?>();

            // Translate the byte array into references using
            // the dictionary passed in.
            foreach (var x in bytes)
            {
                if (glossaryPage.TryGetValue(x, out dynamic? dictionaryItem) == false)
                {
                    if (memoryAddress.HasValue)
                    {
                        logger.LogWarning($"Could not translate byte {x.ToHexdecimalString()} at {memoryAddress.Value.ToHexdecimalString()}, no matching reference found in glossary {glossaryPageName}.");
                    }
                    else
                    {
                        logger.LogWarning($"Could not translate byte {x.ToHexdecimalString()}, no matching reference found in glossary {glossaryPageName}.");
                    }
                    continue;
                }

                if (dictionaryItem == null && returnOnNull)
                {
                    return translatedArray;
                }
                else if (dictionaryItem == null)
                {
                    continue;
                }

                translatedArray.Add(dictionaryItem);
            }

            return translatedArray;
        }
    }

    public class ReferenceProperty : GameHookProperty<object>
    {
        private string GlossaryPageName { get; }
        private GameHookGlossaryPage GlossaryPage { get; }

        public ReferenceProperty(IGameHookContainer mapper, string identifier, PropertyFields fields)
            : base(mapper, identifier, fields)
        {
            GlossaryPageName = fields.Reference ?? throw new NullReferenceException(nameof(fields.Reference));
            GlossaryPage = mapper.Glossary[GlossaryPageName] ?? new Dictionary<byte, dynamic>();
        }

        protected override byte[] FromValue(object? value)
        {
            throw new NotImplementedException();
        }

        protected override object? ToValue(byte[] bytes)
        {
            var values = ReferenceArrayHelper.GetFromGlossary<object?>(Logger, Address, GlossaryPageName, GlossaryPage, bytes);

            return values.FirstOrDefault();
        }
    }

    public class ReferenceArrayProperty : GameHookProperty<IEnumerable<object?>>
    {
        private string GlossaryPageName { get; }
        private GameHookGlossaryPage GlossaryPage { get; }

        public ReferenceArrayProperty(IGameHookContainer mapper, string identifier, PropertyFields fields)
            : base(mapper, identifier, fields)
        {
            GlossaryPageName = fields.Reference ?? throw new NullReferenceException(nameof(fields.Reference));
            GlossaryPage = mapper.Glossary[GlossaryPageName];
        }

        protected override byte[] FromValue(IEnumerable<object?>? value)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<object?> ToValue(byte[] bytes)
        {
            return bytes.Select(x => ReferenceArrayHelper.GetFromGlossary<object?>(Logger, Address, GlossaryPageName, GlossaryPage, new byte[] { x }));
        }
    }
}