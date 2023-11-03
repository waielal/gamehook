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
        public async Task GB_PokemonRed() => await LoadState("gb_pokemon_red_blue", "gb_pokemon_red_blue");

        [TestMethod]
        public async Task GB_PokemonBlue() => await LoadState("gb_pokemon_red_blue", "gb_pokemon_red_blue");

        [TestMethod]
        public async Task GB_PokemonYellow() => await LoadState("gb_pokemon_red_blue", "gb_pokemon_yellow");

        [TestMethod]
        public async Task GBA_MetroidFusion() => await LoadState("gba_metroid_fusion");

        [TestMethod]
        public async Task GBA_PokemonEmerald() => await LoadState("gba_metroid_fusion");

        [TestMethod]
        public async Task GBA_PokemonFireRed() => await LoadState("gba_metroid_fusion");

        [TestMethod]
        public async Task GBA_PokemonRuby() => await LoadState("gba_metroid_fusion");

        [TestMethod]
        public async Task GBA_PokemonSapphire() => await LoadState("gba_metroid_fusion");

        [TestMethod]
        public async Task GBC_PokemonGold() => await LoadState("gba_metroid_fusion");

        [TestMethod]
        public async Task GBC_PokemonSilver() => await LoadState("gba_metroid_fusion");

        [TestMethod]
        public async Task GBC_PokemonCrystal() => await LoadState("gba_metroid_fusion");

        [TestMethod]
        public async Task GBC_LinksAwakening_YAML() => await LoadState("gba_metroid_fusion", "");

        [TestMethod]
        public async Task N64_MarioKart64() => await LoadState("gba_metroid_fusion", "");

        [TestMethod]
        public async Task NDS_PokemonPlatinum() => await LoadState("gba_metroid_fusion", "");

        [TestMethod]
        public async Task NES_DragonQuest1_YAML() => await LoadState("gba_metroid_fusion", "");

        [TestMethod]
        public async Task PSX_ResidentEvil3Nemesis() => await LoadState("gba_metroid_fusion", "");
    }
}
