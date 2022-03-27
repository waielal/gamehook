using GameHook.Domain.DTOs;

namespace GameHook.Domain.Interfaces
{
    public interface IGameHookContainerFactory
    {
        IGameHookContainer? LoadedMapper { get; }

        string? LoadedMapperId { get; }

        Task LoadGameMapper(string id);

        Task ReloadGameMapper();
    }
}
