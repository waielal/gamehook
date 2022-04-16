using GameHook.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameHook.Domain.GameHookProperties
{
    public static class ReferenceArrayHelper
    {
        public static T? GetFromGlossary<T>(ILogger logger, MemoryAddress? memoryAddress, string glossaryPageName, GameHookGlossaryPage glossaryPage, byte[] bytes) where T : class
        {
            // Translate the byte array into references using
            // the dictionary passed in.
            byte[] value = new byte[8];
            Array.Copy(bytes.Reverse().ToArray(), value, bytes.Length);

            var convertedValue = BitConverter.ToUInt32(value, 0);

            if (glossaryPage.ContainsKey(convertedValue) == false)
            {
                if (memoryAddress.HasValue)
                {
                    logger.LogWarning($"Could not translate byte {bytes.ToHexdecimalString()} at {memoryAddress.Value.ToHexdecimalString()}, no matching reference found in glossary {glossaryPageName}.");
                }
                else
                {
                    logger.LogWarning($"Could not translate byte {bytes.ToHexdecimalString()}, no matching reference found in glossary {glossaryPageName}.");
                }

                return null;
            }

            return glossaryPage[convertedValue];
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
            GlossaryPage = mapper.Glossary[GlossaryPageName] ?? new Dictionary<uint, dynamic>();
        }

        protected override byte[] FromValue(object? value)
        {
            throw new NotImplementedException();
        }

        protected override object? ToValue(byte[] bytes)
        {
            return ReferenceArrayHelper.GetFromGlossary<object>(Logger, Address, GlossaryPageName, GlossaryPage, bytes);
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
            return bytes.Select(x => ReferenceArrayHelper.GetFromGlossary<object>(Logger, Address, GlossaryPageName, GlossaryPage, new byte[] { x }));
        }
    }
}