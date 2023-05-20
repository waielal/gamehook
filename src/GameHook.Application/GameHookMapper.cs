using GameHook.Domain;
using GameHook.Domain.Interfaces;

namespace GameHook.Application
{
    public class GameHookMapper : IGameHookMapper
    {
        public GameHookMapper(MapperMetadata metadata,
            IEnumerable<IGameHookProperty> properties,
            IDictionary<string, IEnumerable<GlossaryItem>> glossary)
        {
            Metadata = metadata;
            Properties = properties;
            Glossary = glossary;
        }

        public Guid Id => Metadata.Id;
        public MapperMetadata Metadata { get; init; }
        public IEnumerable<IGameHookProperty> Properties { get; init; }
        public IDictionary<string, IEnumerable<GlossaryItem>> Glossary { get; init; }

        public IGameHookProperty GetPropertyByPath(string path)
        {
            return Properties.Single(x => x.Path == path);
        }
    }
}