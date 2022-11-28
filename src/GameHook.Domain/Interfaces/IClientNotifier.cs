using GameHook.Domain.DTOs;

namespace GameHook.Domain.Interfaces
{
    public interface IClientNotifier
    {
        Task SendGameHookError(ProblemDetailsForClientDTO details);
        Task SendInstanceReset();
        Task SendMapperLoaded(IGameHookMapper mapper);
        Task SendDriverError(ProblemDetailsForClientDTO details);
        Task SendPropertyChanged(IGameHookProperty property, string[] fieldsChanged, MapperUserSettingsDTO? mapperUserConfig);
        Task SendUiBuilderScreenSaved(Guid id);
    }
}
