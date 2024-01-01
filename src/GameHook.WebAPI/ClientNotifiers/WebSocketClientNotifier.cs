using GameHook.Domain.Interfaces;
using GameHook.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GameHook.WebAPI.ClientNotifiers
{
    public class WebSocketClientNotifier(IHubContext<UpdateHub> hubContext) : IClientNotifier
    {
        private readonly IHubContext<UpdateHub> _hubContext = hubContext;

        public Task SendInstanceReset() =>
            _hubContext.Clients.All.SendAsync("InstanceReset");

        public Task SendMapperLoaded(IGameHookMapper mapper) =>
            _hubContext.Clients.All.SendAsync("MapperLoaded");

        public Task SendError(IProblemDetails problemDetails) =>
            _hubContext.Clients.All.SendAsync("Error", problemDetails);

        public Task SendPropertiesChanged(IEnumerable<IGameHookProperty> properties) =>
            _hubContext.Clients.All.SendAsync("PropertiesChanged", properties.Select(x => new
            {
                path = x.Path,
                address = x.Address,
                frozen = x.BytesFrozen != null,
                value = x.Value,
                bytes = x.Bytes?.Select(x => (int)x).ToArray(),
                fieldsChanged = x.FieldsChanged
            }).ToArray());
    }
}
