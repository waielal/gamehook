using GameHook.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameHook.Domain.GameHookProperties
{
    public static class ReferenceArrayHelper
    {
        public static GlossaryItem? SingleOrDefaultByKey(ILogger logger, MemoryAddress? memoryAddress, string glossaryName, IEnumerable<GlossaryItem> glossaryItems, byte[] bytes)
        {
            // Translate the byte array into references using
            // the dictionary passed in.
            byte[] value = new byte[8];
            Array.Copy(bytes.Reverse().ToArray(), value, bytes.Length);

            var convertedValue = BitConverter.ToUInt32(value, 0);

            var item = glossaryItems.SingleOrDefault(x => x.Key == convertedValue);

            if (item == null)
            {
                if (memoryAddress.HasValue)
                {
                    logger.LogWarning($"Could not get value for {bytes.ToHexdecimalString()} at {memoryAddress.Value.ToHexdecimalString()}, no matching key found in glossary {glossaryName}.");
                }
                else
                {
                    logger.LogWarning($"Could not get value for {bytes.ToHexdecimalString()}, no matching key found in glossary {glossaryName}.");
                }

                return null;
            }

            return item;
        }

        public static GlossaryItem? SingleOrDefaultByValue(ILogger logger, string glossaryName, IEnumerable<GlossaryItem> glossaryItems, string? value)
        {
            var item = glossaryItems.SingleOrDefault(x => x.Value?.ToString() == value);

            if (item == null)
            {
                logger.LogWarning($"Could not get key for {value}, no matching value found in glossary {glossaryName}.");

                return null;
            }

            return item;
        }

        public static GlossaryItem? FirstOrDefaultByValue(ILogger logger, string glossaryName, IEnumerable<GlossaryItem> glossaryItems, string? value)
        {
            var item = glossaryItems.FirstOrDefault(x => x.Value?.ToString() == value);

            if (item == null)
            {
                logger.LogWarning($"Could not get key for {value}, no matching value found in glossary {glossaryName}.");

                return null;
            }

            return item;
        }
    }

    public class ReferenceProperty : GameHookProperty<object>
    {
        private string GlossaryName { get; }
        private IEnumerable<GlossaryItem> GlossaryItems { get; }

        public ReferenceProperty(IGameHookContainer mapper, string identifier, PropertyFields fields)
            : base(mapper, identifier, fields)
        {
            GlossaryName = fields.Reference ?? throw new NullReferenceException(nameof(fields.Reference));
            GlossaryItems = mapper.Glossary[GlossaryName] ?? throw new Exception($"Could not load glossary items for {GlossaryName}.");
        }

        protected override byte[] FromValue(object? value)
        {
            throw new NotImplementedException();
        }

        protected override object? ToValue(byte[] bytes)
        {
            return ReferenceArrayHelper.SingleOrDefaultByKey(Logger, Address, GlossaryName, GlossaryItems, bytes)?.Value;
        }
    }

    public class ReferenceArrayProperty : GameHookProperty<IEnumerable<object?>>
    {
        private string GlossaryName { get; }
        private IEnumerable<GlossaryItem> GlossaryItems { get; }

        public ReferenceArrayProperty(IGameHookContainer mapper, string identifier, PropertyFields fields)
            : base(mapper, identifier, fields)
        {
            GlossaryName = fields.Reference ?? throw new NullReferenceException(nameof(fields.Reference));
            GlossaryItems = mapper.Glossary[GlossaryName] ?? throw new Exception($"Could not load glossary items for {GlossaryName}.");
        }

        protected override byte[] FromValue(IEnumerable<object?>? value)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<object?> ToValue(byte[] bytes)
        {
            return bytes.Select(x => ReferenceArrayHelper.SingleOrDefaultByKey(Logger, Address, GlossaryName, GlossaryItems, new byte[] { x })?.Value);
        }
    }
}