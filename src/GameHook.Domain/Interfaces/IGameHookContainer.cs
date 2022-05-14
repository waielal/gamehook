using Microsoft.Extensions.Logging;

namespace GameHook.Domain.Interfaces
{
    public record GameHookMapperMeta(int SchemaVersion, Guid Id, string GameName, string GamePlatform);

    public interface IGameHookContainer
    {
        ILogger Logger { get; }
        IGameHookDriver Driver { get; }
        IEnumerable<IClientNotifier> ClientNotifiers { get; }
        IPlatformOptions PlatformOptions { get; }
        GameHookMapperMeta Meta { get; }
        GameHookGlossary Glossary { get; }
        GameHookMacros Macros { get; }
        IDictionary<string, IGameHookProperty> Properties { get; }

        void AddHookProperty(string path, IGameHookProperty property);
        IGameHookProperty? GetPropertyByPath(string path);
        IGameHookProperty? GetRequiredPropertyByPath(string path);

        Task Initialize();
    }
}
