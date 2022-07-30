using GameHook.Domain;
using GameHook.Domain.Interfaces;

namespace GameHook.Application
{
    public class GameHookMapper : IGameHookMapper
    {
        public GameHookMapper(string filesystemId, MapperMetadata metadata, IEnumerable<GameHookProperty> properties, IDictionary<string, IEnumerable<GlossaryItem>> glossary)
        {
            FilesystemId = filesystemId;
            Metadata = metadata;
            Properties = properties;
            Glossary = glossary;
        }

        public string FilesystemId { get; init; }
        public MapperMetadata Metadata { get; init; }
        public IEnumerable<IGameHookProperty> Properties { get; init; }
        public IDictionary<string, IEnumerable<GlossaryItem>> Glossary { get; init; }

        public IGameHookProperty GetPropertyByPath(string path)
        {
            return Properties.Single(x => x.Path == path);
        }
    }
}
