using System.Text.Json;
using GameHook.Domain;
using GameHook.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GameHook.WebAPI.Controllers
{
    public class UiBuilderScreenMetadataModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Author { get; set; }

        public DateTime LastModified { get; set; }
    }

    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("ui-builder")]
    public class UiBuilderController : ControllerBase
    {
        private List<IClientNotifier> ClientNotifiers { get; }
        private JsonSerializerOptions SerializerOptions { get; }

        public UiBuilderController(IEnumerable<IClientNotifier> clientNotifiers)
        {
            ClientNotifiers = clientNotifiers.ToList();

            SerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [SwaggerOperation("Gets all UI builder screens.")]
        [HttpGet("screens")]
        public IEnumerable<UiBuilderScreenMetadataModel> GetAllUiBuilderScreens()
        {
            var files = Directory.GetFiles(BuildEnvironment.ConfigurationDirectoryUiBuilderScreenDirectory, "*.json", SearchOption.TopDirectoryOnly)
                         .Select(x => new FileInfo(x))
                         .OrderByDescending(x => x.LastWriteTime)
                         .ToArray();

            return files.Select(x =>
            {
                var json = System.IO.File.ReadAllText(x.FullName);

                var model = JsonSerializer.Deserialize<UiBuilderScreenMetadataModel>(json, SerializerOptions)
                                ?? throw new Exception($"Unable to parse screen {x.Name}.");

                model.Id = Guid.Parse(Path.GetFileNameWithoutExtension(x.Name));
                model.Name = string.IsNullOrWhiteSpace(model.Name) ? $"Screen {model.Id}" : model.Name;
                model.LastModified = x.LastWriteTime;

                return model;
            });
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