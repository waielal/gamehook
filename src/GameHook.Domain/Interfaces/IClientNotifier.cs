using GameHook.Domain.DTOs;

namespace GameHook.Domain.Interfaces
{
    public interface IClientNotifier
    {
        Task SendGameHookError(ProblemDetailsForClientDTO details);
        Task SendInstanceReset();
        Task SendMapperLoaded(IGameHookMapper mapper);
        Task SendMapperLoadError();
        Task SendDriverError(ProblemDetailsForClientDTO details);
        Task SendPropertyChanged(string key, uint? address, object? value, byte[]? bytes, bool frozen, string[] fieldsChanged);
    }
}
