using GameHook.Application;
using GameHook.Domain;
using GameHook.Domain.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace GameHook.WebAPI.Controllers
{
    static class MapperHelper
    {
        public static PropertyModel MapToPropertyModel(this IGameHookProperty x, string path) =>
            new PropertyModel
            {
                Path = path,
                Type = x.Type,
                Address = x.Address,
                Size = x.Size,
                Position = x.MapperVariables.Position,
                Reference = x.MapperVariables.Reference,
                CharacterMap = x.MapperVariables.CharacterMap,
                Value = x.Value,
                Frozen = x.Frozen,
                Bytes = x.Bytes?.ToIntegerArray(),
                Description = x.MapperVariables.Description
            };
    }

    public record MapperModel
    {
        public MapperMetaModel Meta { get; init; } = null!;
        public IEnumerable<PropertyModel> Properties { get; init; } = null!;
        public Dictionary<string, IEnumerable<GlossaryItemModel>> Glossary { get; init; } = null!;
    }

    public record MapperMetaModel
    {
        public int SchemaVersion { get; init; }
        public Guid Id { get; init; }
        public string GameName { get; init; } = string.Empty;
        public string GamePlatform { get; init; } = string.Empty;
    }

    public class GlossaryItemModel
    {
        public uint Key { get; init; }
        public object? Value { get; init; }
    }

    public record MapperReplaceModel(string? Id);

    public class PropertyModel
    {
        public string Path { get; init; } = string.Empty;

        public string Type { get; init; } = string.Empty;

        public int Size { get; init; }

        public uint? Address { get; init; }

        public int? Position { get; init; }

        public string? Reference { get; init; }

        public string? CharacterMap { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public object? Value { get; init; }

        public IEnumerable<int>? Bytes { get; init; } = Enumerable.Empty<int>();

        public bool? Frozen { get; init; }

        public string? Description { get; init; }
    }

    public class UpdatePropertyValueModel
    {
        public string Path { get; init; } = string.Empty;
        public object? Value { get; init; }
        public bool? Freeze { get; init; }
    }

    public class UpdatePropertyBytesModel
    {
        public string Path { get; init; } = string.Empty;
        public int[] Bytes { get; init; } = Array.Empty<int>();
        public bool? Freeze { get; init; }
    }

    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("mapper")]
    public class MapperController : ControllerBase
    {
        public GameHookInstance Instance { get; }
        public IGameHookDriver Driver { get; }

        public MapperController(GameHookInstance gameHookInstance, IGameHookDriver driver)
        {
            Instance = gameHookInstance;
            Driver = driver;
        }

        [HttpGet]
        [SwaggerOperation("Returns the mapper that was loaded, with all properties (populated with data).")]
        public ActionResult<MapperModel> GetMapper()
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            var model = Instance.Mapper.Adapt<MapperModel>();
            return Ok(model);
        }

        [HttpPut]
        [SwaggerOperation("Changes the active mapper.")]
        public async Task<ActionResult> ChangeMapper(MapperReplaceModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Id))
                {
                    // Reload the existing mapper from the filesystem.
                    await Instance.Load(Driver, Instance.Mapper?.FilesystemId ?? string.Empty);
                }
                else
                {
                    await Instance.Load(Driver, model.Id);
                }

                return Ok();
            }
            catch (PropertyProcessException ex)
            {
                return StatusCode(500, new ProblemDetails() { Status = 500, Title = "An error occured when loading the mapper.", Detail = ex.Message });
            }
            catch
            {
                return StatusCode(500, new ProblemDetails() { Status = 500, Title = "An error occured when loading the mapper." });
            }
        }

        [HttpGet("meta")]
        [SwaggerOperation("Returns the meta section of the mapper file.")]
        public ActionResult<MapperMetaModel> GetMeta()
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            var meta = Instance.Mapper.Metadata;
            var model = new MapperMetaModel
            {
                SchemaVersion = meta.SchemaVersion,
                Id = meta.Id,
                GameName = meta.GameName,
                GamePlatform = meta.GamePlatform
            };

            return Ok(model);
        }

        [HttpGet("values/{**path}/")]
        [Produces("text/plain")]
        [SwaggerOperation("Returns a specific property's value by it's path.")]
        public ActionResult GetValueAsync(string path)
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            path = path.StripEndingRoute().FromRouteToPath();

            var prop = Instance.Mapper.GetPropertyByPath(path);

            if (prop == null)
                return NotFound();

            if (prop.Value is object)
            {
                return BadRequest($"{prop.Path} is an object and cannot be converted to text.");
            }

            return Ok(prop.Value?.ToString() ?? string.Empty);
        }

        [HttpGet("properties")]
        [SwaggerOperation("Returns all properties loaded from the mapper.")]
        public ActionResult<IEnumerable<PropertyModel>> GetProperties()
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            return Ok(Instance.Mapper.Properties.Select(x => x.MapToPropertyModel(x.Path)));
        }

        [HttpGet("properties/{**path}/")]
        [SwaggerOperation("Returns a specific property by it's path.")]
        public ActionResult<PropertyModel?> GetProperty(string path)
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            path = path.StripEndingRoute().FromRouteToPath();

            var prop = Instance.Mapper.GetPropertyByPath(path);

            if (prop == null)
                return NotFound();

            return Ok(prop.MapToPropertyModel(path));
        }

        [HttpPost("set-property-value")]
        [SwaggerOperation("Updates a property's value.")]
        public async Task<ActionResult> UpdatePropertyValueAsync(UpdatePropertyValueModel model)
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            var path = model.Path.StripEndingRoute().FromRouteToPath();

            var prop = Instance.Mapper.GetPropertyByPath(path);

            if (prop == null) return NotFound();

            await prop.WriteValue(model.Value?.ToString(), model.Freeze);

            return Ok();
        }

        [HttpPost("set-property-bytes")]
        [SwaggerOperation("Updates a property's bytes.")]
        public async Task<ActionResult> UpdatePropertyBytesAsync(UpdatePropertyBytesModel model)
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            var path = model.Path.StripEndingRoute().FromRouteToPath();
            var actualBytes = model.Bytes.Select(x => (byte)x).ToArray();

            var prop = Instance.Mapper.GetPropertyByPath(path);

            if (prop == null) return NotFound();

            await prop.WriteBytes(actualBytes, model.Freeze);

            return Ok();
        }

        [HttpPost("set-property-frozen")]
        [SwaggerOperation("Updates a property's frozen status.")]
        public async Task<ActionResult> FreezePropertyAsync(string path, bool frozen = true)
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            path = path.StripEndingRoute().FromRouteToPath();

            var prop = Instance.Mapper.GetPropertyByPath(path);

            if (prop == null) return NotFound();

            if (frozen)
            {
                await prop.FreezeProperty(prop.Bytes ?? Array.Empty<byte>());
            }
            else
            {
                await prop.UnfreezeProperty();
            }

            return Ok();
        }

        [HttpGet("glossary")]
        [SwaggerOperation("Returns the glossary section of the mapper file.")]
        public ActionResult<Dictionary<string, Dictionary<byte, dynamic>>> GetGlossary()
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            return Ok(Instance.Mapper.Glossary);
        }

        [HttpGet("glossary/{key}")]
        [SwaggerOperation("Returns a specific glossary by it's key.")]
        public ActionResult<Dictionary<byte, dynamic>> GetGlossaryPage(string key)
        {
            if (Instance.Initalized == false || Instance.Mapper == null)
                return ApiHelper.MapperNotLoaded();

            key = key.StripEndingRoute();

            if (Instance.Mapper.Glossary.ContainsKey(key) == false)
                return NotFound();
            else
                return Ok(Instance.Mapper.Glossary[key]);
        }
    }
}