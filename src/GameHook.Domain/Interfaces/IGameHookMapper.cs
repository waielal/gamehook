using GameHook.Domain.DTOs;

namespace GameHook.Domain.Interfaces
{
    public interface IGameHookMapper
    {
        public Guid Id { get; }

        public IEnumerable<IGameHookProperty> Properties { get; init; }

        public IDictionary<string, IEnumerable<GlossaryItem>> Glossary { get; init; }

        public MapperUserSettingsDTO? UserSettings { get; init; }
    }
}
