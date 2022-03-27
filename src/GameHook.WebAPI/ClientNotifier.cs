using GameHook.Domain.DTOs;
using GameHook.Domain.Interfaces;
using GameHook.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GameHook.WebAPI
{
    public class ClientNotifier : IClientNotifier
    {
        private readonly IHubContext<UpdateHub> _hubContext;
        public ClientNotifier(IHubContext<UpdateHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendGameHookError(ProblemDetailsForClientDTO problemDetails) =>
            _hubContext.Clients.All.SendAsync("GameHookError", problemDetails);

        public Task SendMapperLoaded() =>
            _hubContext.Clients.All.SendAsync("MapperLoaded");

        public Task SendDriverError(ProblemDetailsForClientDTO problemDetails) =>
            _hubContext.Clients.All.SendAsync("DriverError", problemDetails);

        public Task SendPropertyChanged(string key, object? value, IEnumerable<int> bytes, bool frozen) =>
            _hubContext.Clients.All.SendAsync("PropertyChanged", key, value, bytes, frozen);

        public Task SendPropertyFrozen(string key) =>
            _hubContext.Clients.All.SendAsync("PropertyFrozen", key);

        public Task SendPropertyUnfrozen(string key) =>
            _hubContext.Clients.All.SendAsync("PropertyUnfrozen", key);
    }
}
