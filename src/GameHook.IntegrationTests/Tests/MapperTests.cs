using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenAPI.GameHook;
using System.Threading.Tasks;

namespace GameHook.IntegrationTests.Tests
{
    [TestClass]
    public class MapperTests : BaseTest
    {
        [TestMethod]
        public async Task AllMappersLoad_OK()
        {
            var mappers = await GameHookClient.GetMapperFilesAsync();

            foreach (var mapper in mappers)
            {
                // TODO: dma_967d10cc may be broken?
                if (mapper.Id == "official_gba_pokemon_emerald_yml") { continue; }

                Logger.LogInformation($"Checking {mapper.Id} {mapper.DisplayName}.");

                // Load generic memory for the system.
                if (mapper.Id.StartsWith("official_gb_")) { await LoadRamState("gb_pokemon_red_blue"); }
                else if (mapper.Id.StartsWith("official_gbc_")) { await LoadRamState("gbc_pokemon_crystal"); }
                else if (mapper.Id.StartsWith("official_gba_")) { await LoadRamState("gba_pokemon_emerald"); }
                else if (mapper.Id.StartsWith("official_nds_")) { await LoadRamState("nds_pokemon_platinum"); }

                await GameHookClient.ChangeMapperAsync(new MapperReplaceModel()
                {
                    Id = mapper.Id,
                    Driver = "staticMemory"
                });
            }
        }

    }
}
