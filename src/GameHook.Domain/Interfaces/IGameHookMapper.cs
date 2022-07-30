namespace GameHook.Domain.Interfaces
{
    public interface IGameHookMapper
    {
        public IEnumerable<IGameHookProperty> Properties { get; init; }

        public IDictionary<string, IEnumerable<GlossaryItem>> Glossary { get; init; }
    }
}
