namespace GameHook.Domain.Interfaces
{
    public class MapperMetadata
    {
        public Guid Id { get; init; } = Guid.Empty;
        public string UniqueIdentifier => GameName.Replace(" ", string.Empty);
        public string GameName { get; init; } = string.Empty;
        public string GamePlatform { get; init; } = string.Empty;
    }

    public interface IGameHookMapper
    {
        public Guid Id { get; }

        public MapperMetadata Metadata { get; init; }

        public IEnumerable<IGameHookProperty> Properties { get; init; }

        public IEnumerable<GlossaryList> Glossary { get; init; }

        IGameHookProperty GetPropertyByPath(string path);
    }
}
