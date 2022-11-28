using GameHook.Domain;
using GameHook.Domain.DTOs;
using GameHook.Domain.Interfaces;
using GameHook.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GameHook.WebAPI.ClientNotifiers
{
    public class WebSocketClientNotifier : IClientNotifier
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        public WebSocketClientNotifier(IHubContext<UpdateHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendGameHookError(ProblemDetailsForClientDTO problemDetails) =>
            _hubContext.Clients.All.SendAsync("GameHookError", problemDetails);

        public Task SendInstanceReset() =>
            _hubContext.Clients.All.SendAsync("InstanceReset");

        public Task SendMapperLoaded(IGameHookMapper mapper) =>
            _hubContext.Clients.All.SendAsync("MapperLoaded");

        public Task SendDriverError(ProblemDetailsForClientDTO problemDetails) =>
            _hubContext.Clients.All.SendAsync("DriverError", problemDetails);

        public Task SendPropertyChanged(IGameHookProperty property, string[] fieldsChanged, MapperUserSettingsDTO? _) =>
            _hubContext.Clients.All.SendAsync("PropertyChanged", property.Path, property.Address, property.Value, property.Bytes?.ToIntegerArray(), property.Frozen, fieldsChanged);

        public Task SendUiBuilderScreenSaved(Guid id) =>
            _hubContext.Clients.All.SendAsync("UiBuilderScreenSaved", id);
    }
}
