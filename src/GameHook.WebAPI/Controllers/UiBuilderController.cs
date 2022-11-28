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

        [SwaggerOperation("Gets UI builder screen.")]
        [HttpGet("screens/{id}/")]
        public ActionResult<object> GetUiBuilderScreen(Guid id)
        {
            var path = $"{BuildEnvironment.ConfigurationDirectoryUiBuilderScreenDirectory}\\{id}.json";

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

        [SwaggerOperation("Updates UI builder screen.")]
        [HttpPut("screens/{id}/")]
        public async Task<ActionResult> SetUiBuilderScreen(Guid id, [FromBody] dynamic data)
        {
            if (Directory.Exists(BuildEnvironment.ConfigurationDirectoryUiBuilderScreenDirectory) == false)
            {
                Directory.CreateDirectory(BuildEnvironment.ConfigurationDirectoryUiBuilderScreenDirectory);
            }

            var path = $"{BuildEnvironment.ConfigurationDirectoryUiBuilderScreenDirectory}\\{id}.json";
            System.IO.File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(data));

            await ClientNotifiers.ForEachAsync(async x => await x.SendUiBuilderScreenSaved(id));

            return Ok();
        }
    }
}