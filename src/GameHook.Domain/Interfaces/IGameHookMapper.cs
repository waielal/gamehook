using GameHook.Domain.DTOs;

namespace GameHook.Domain.Interfaces
{
    public class MapperMetadata
    {
        public int SchemaVersion { get; init; } = 0;
        public Guid Id { get; init; } = Guid.Empty;
        public string GameName { get; init; } = string.Empty;
        public string GamePlatform { get; init; } = string.Empty;
    }

    public interface IGameHookMapper
    {
        public Guid Id { get; }

        public string FilesystemId { get; init; }

        public MapperMetadata Metadata { get; init; }

        public IEnumerable<IGameHookProperty> Properties { get; init; }

        public IDictionary<string, IEnumerable<GlossaryItem>> Glossary { get; init; }

        public MapperUserSettingsDTO? UserSettings { get; init; }

        IGameHookProperty GetPropertyByPath(string path);
    }
}
