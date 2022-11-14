using GameHook.Domain;
using GameHook.Domain.DTOs;
using GameHook.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace GameHook.WebAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("ui")]
    public class UiController : ControllerBase
    {
        private List<IClientNotifier> ClientNotifiers { get; }

        public UiController(IEnumerable<IClientNotifier> clientNotifiers)
        {
            ClientNotifiers = clientNotifiers.ToList();
        }

        [SwaggerOperation("Gets UI configuration.")]
        [HttpGet("configuration")]
        public ActionResult<UiConfigurationDTO> GetUiConfiguration()
        {
            if (System.IO.File.Exists(BuildEnvironment.ConfigurationDirectoryUiConfigFilePath))
            {
                var uiConfigurationJson = System.IO.File.ReadAllText(BuildEnvironment.ConfigurationDirectoryUiConfigFilePath);
                var data = JsonConvert.DeserializeObject<UiConfigurationDTO>(uiConfigurationJson);
                return data;
            }
            else
            {
                return NoContent();
            }
        }

        [SwaggerOperation("Updates UI configuration.")]
        [HttpPut("configuration")]
        public async Task<ActionResult> SetUiConfiguration(UiConfigurationDTO data)
        {
            System.IO.File.WriteAllText(BuildEnvironment.ConfigurationDirectoryUiConfigFilePath, JsonConvert.SerializeObject(data));

            await ClientNotifiers.ForEachAsync(async x => await x.SendUiConfigurationChanged(data));

            return Ok();
        }
    }
}