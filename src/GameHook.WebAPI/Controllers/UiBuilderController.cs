using GameHook.Domain;
using GameHook.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace GameHook.WebAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("ui-builder")]
    public class UiBuilderController : ControllerBase
    {
        private List<IClientNotifier> ClientNotifiers { get; }

        public UiBuilderController(IEnumerable<IClientNotifier> clientNotifiers)
        {
            ClientNotifiers = clientNotifiers.ToList();
        }

        [SwaggerOperation("Gets UI builder configuration.")]
        [HttpGet("configuration/{id}/")]
        public ActionResult<object> GetUiBuilderConfiguration(Guid id)
        {
            var path = $"{BuildEnvironment.ConfigurationDirectoryUiBuilderConfigDirectory}\\{id}.json";

            if (System.IO.File.Exists(path))
            {
                var uiConfigurationJson = System.IO.File.ReadAllText(path);
                return Content(uiConfigurationJson, "application/json");
            }
            else
            {
                return NoContent();
            }
        }

        [SwaggerOperation("Updates UI builder configuration.")]
        [HttpPut("configuration/{id}/")]
        public async Task<ActionResult> SetUiBuilderConfiguration(Guid id, string data)
        {
            if (Directory.Exists(BuildEnvironment.ConfigurationDirectoryUiBuilderConfigDirectory) == false)
            {
                Directory.CreateDirectory(BuildEnvironment.ConfigurationDirectoryUiBuilderConfigDirectory);
            }

            var path = $"{BuildEnvironment.ConfigurationDirectoryUiBuilderConfigDirectory}\\{id}.json";

            var dataObject = JsonConvert.DeserializeObject<object>(data);
            if (dataObject == null) { throw new Exception("Failed to parse data."); }

            System.IO.File.WriteAllText(path, data);

            await ClientNotifiers.ForEachAsync(async x => await x.SendUiBuilderConfigurationChanged(id));

            return Ok();
        }
    }
}