using GameHook.Domain;
using GameHook.Domain.Interfaces;

namespace GameHook.Application
{
    public class GameHookMapper : IGameHookMapper
    {
        public GameHookMapper(
            MetadataSection metadata,
            MemorySection memory,
            IEnumerable<IGameHookProperty> properties,
            IEnumerable<ReferenceItems> references,
            string? globalScript,
            bool hasGlobalPreprocessor,
            bool hasGlobalPostprocessor)
        {
            Metadata = metadata;
            Memory = memory;
            Properties = properties.ToDictionary(x => x.Path, x => x);
            References = references.ToDictionary(x => x.Name, x => x);

            GlobalScript = globalScript;
            HasGlobalPreprocessor = hasGlobalPreprocessor;
            HasGlobalPostprocessor = hasGlobalPostprocessor;
        }

        public MetadataSection Metadata { get; }
        public MemorySection Memory { get; }
        public Dictionary<string, IGameHookProperty> Properties { get; }
        public Dictionary<string, ReferenceItems> References { get; }

        public string? GlobalScript { get; }
        public bool HasGlobalPreprocessor { get; }
        public bool HasGlobalPostprocessor { get; }

        public IGameHookProperty[] GetAllProperties() => Properties.Values.ToArray();
    }
}