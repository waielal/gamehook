using GameHook.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GameHook.WebAPI.Controllers
{
    public record UpdateMemoryModel(int StartingAddress, byte[] Bytes);

    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("driver")]
    public class DriverController : Controller
    {
        private readonly IGameHookContainerFactory _gameHookContainerFactory;
        private readonly IGameHookDriver _driver;

        public DriverController(IGameHookContainerFactory gameHookContainerFactory, IGameHookDriver gameHookDriver)
        {
            _gameHookContainerFactory = gameHookContainerFactory;
            _driver = gameHookDriver;
        }

        [HttpPut("memory")]
        [SwaggerOperation("Write bytes back to the driver manually.")]
        public async Task<IActionResult> WriteMemory(UpdateMemoryModel model)
        {
            if (_gameHookContainerFactory.LoadedMapper == null)
                return ApiHelper.MapperNotLoaded();

            await _driver.WriteBytes(model.StartingAddress, model.Bytes);

            return Ok();
        }
    }
}
