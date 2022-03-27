using GameHook.Domain.DTOs;

namespace GameHook.Domain.Interfaces
{
    public interface IClientNotifier
    {
        Task SendGameHookError(ProblemDetailsForClientDTO details);
        Task SendMapperLoaded();
        Task SendDriverError(ProblemDetailsForClientDTO details);
        Task SendPropertyChanged(string key, object? value, IEnumerable<int> bytes, bool frozen);
        Task SendPropertyFrozen(string key);
        Task SendPropertyUnfrozen(string key);
    }
}
