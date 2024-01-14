using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace GameHook.IntegrationTests
{
    [TestClass]
    public class PokemonFireRed : BaseTest
    {
        // FireRed and LeafGreen Tests
        [TestMethod]
        public async Task FireRed_Property_OK_DMA_B_GameTime()
        {
            await Load_GBA_PokemonFireRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("game_time.hours", 0x0202460E, [0x00], 0);
            mapper.AssertAreEqual("game_time.minutes", 0x02024610, [0x1B], 27);
            mapper.AssertAreEqual("game_time.seconds", 0x02024611, [0x12], 18);
            mapper.AssertAreEqual("game_time.frames", 0x02024612, [0x34], 52);
        }

        [TestMethod]
        public async Task FireRed_Property_OK_PokemonPartyStructure_Index0()
        {
            await Load_GBA_PokemonFireRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.team.0.species", 0x20, [0x07, 0x00], "Squirtle");
            mapper.AssertAreEqual("player.team.0.internals.personality_value", [0x68, 0x5A, 0x6F, 0xA9], 2842647144);
            mapper.AssertAreEqual("player.team.0.internals.checksum", 0x1C, [0x95, 0x55], 21909);
            mapper.AssertAreEqual("player.team.0.nickname", 0x08, [0xC8, 0xBB, 0xC7, 0xBF, 0xC6, 0xBF, 0xCD, 0xCD, 0xFF, 0xFF], "NAMELESS");
            mapper.AssertAreEqual("player.team.0.exp", 0x24, [0x92, 0x07, 0x00, 0x00], 1938);
            mapper.AssertAreEqual("player.team.0.held_item", 0x22, [0x0F, 0x00], "Burn Heal");
            mapper.AssertAreEqual("player.team.0.friendship", 0x29, [0x74], 116);
            mapper.AssertAreEqual("player.team.0.misc.is_bad_egg", 0x13, [0x02], false);

            mapper.AssertAreEqual("player.team.0.level", 0x54, [0x0E], 14);
            mapper.AssertAreEqual("player.team.0.stats.hp", 0x56, [0x1E, 0x00], 30);

            mapper.AssertAreEqual("player.team.0.moves.0.move", 0x2C, [0x21, 0x00], "Tackle");
            mapper.AssertAreEqual("player.team.0.moves.0.pp", 0x34, [0x22], 34);
            mapper.AssertAreEqual("player.team.0.moves.0.pp_up", 0x5B, [0x00], 0);

            mapper.AssertAreEqual("player.team.0.ivs.hp", 0x48, [0x9A, 0xDA, 0xB0, 0x3B], 26);
            mapper.AssertAreEqual("player.team.0.ivs.attack", 0x48, [0x9A, 0xDA, 0xB0, 0x3B], 20);
            mapper.AssertAreEqual("player.team.0.ivs.defense", 0x48, [0x9A, 0xDA, 0xB0, 0x3B], 22);
            mapper.AssertAreEqual("player.team.0.ivs.speed", 0x48, [0x9A, 0xDA, 0xB0, 0x3B], 1);
            mapper.AssertAreEqual("player.team.0.ivs.special_attack", 0x48, [0x9A, 0xDA, 0xB0, 0x3B], 27);
            mapper.AssertAreEqual("player.team.0.ivs.special_defense", 0x48, [0x9A, 0xDA, 0xB0, 0x3B], 29);

            mapper.AssertAreEqual("player.team.0.evs.hp", 0x38, [0x03], 3);
            mapper.AssertAreEqual("player.team.0.evs.attack", 0x39, [0x00], 0);
            mapper.AssertAreEqual("player.team.0.evs.defense", 0x3A, [0x06], 6);
            mapper.AssertAreEqual("player.team.0.evs.speed", 0x3B, [0x0B], 11);
            mapper.AssertAreEqual("player.team.0.evs.special_attack", 0x3C, [0x01], 1);
            mapper.AssertAreEqual("player.team.0.evs.special_defense", 0x3D, [0x00], 0);

            mapper.AssertAreEqual("player.team.1.species", 0x20, [0x00, 0x00], null);
            mapper.AssertAreEqual("player.team.1.internals.personality_value", [0x00, 0x00, 0x00, 0x00], 0);
        }
        [TestMethod]
        public async Task FireRed_Property_OK_MetaProperties_Index0()
        {
            await Load_GBA_PokemonFireRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("meta.generation", 3);
            mapper.AssertAreEqual("meta.game_name", "FireRed and LeafGreen");
            mapper.AssertAreEqual("meta.game_type", "Remakes");
        }
        [TestMethod]
        public async Task FireRed_Property_OK_GameTime_Index0()
        {
            await Load_GBA_PokemonFireRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("game_time.hours", 0x0202460E, [0x00], 0);
            mapper.AssertAreEqual("game_time.minutes", 0x02024610, [0x1B], 27);
            mapper.AssertAreEqual("game_time.seconds", 0x02024611, [0x12], 18);
            mapper.AssertAreEqual("game_time.frames", 0x02024612, [0x34], 52);
        }
        [TestMethod]
        public async Task FireRed_Property_OK_Options_Index0()
        {
            await Load_GBA_PokemonFireRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("options.battle_animations", 0x020255B9, [0x00], false);
            mapper.AssertAreEqual("options.battle_style", 0x020255B9, [0x00], false);
            mapper.AssertAreEqual("options.sound", 0x020255B9, [0x00], false);
            mapper.AssertAreEqual("options.button_mode", 0x020255B7, [0x00], 0);
        }
        [TestMethod]
        public async Task FireRed_Property_OK_Overworld_Index0()
        {
            await Load_GBA_PokemonFireRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("overworld.map_name", 0x02031DBC, [0x03, 0x15], "ROUTE 3");
        }
        [TestMethod]
        public async Task FireRed_Property_OK_PointersCallbacks_Index0()
        {
            await Load_GBA_PokemonFireRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("pointers.dma_1", 0x03005008, [0xA4, 0x55, 0x02, 0x02], 33707428);
            mapper.AssertAreEqual("pointers.dma_2", 0x0300500C, [0x00, 0x46, 0x02, 0x02], 33703424);
            mapper.AssertAreEqual("pointers.dma_3", 0x03005010, [0x8C, 0x93, 0x02, 0x02], 33723276);
            // mapper.AssertAreEqual("pointers.callback_1", 0x030022C0, [0x05, 0x5E, 0x08, 0x08], "Overworld");
            // mapper.AssertAreEqual("pointers.callback_2", 0x030022C4, [0x5D, 0x5E, 0x08, 0x08], "Overworld");
        }
        [TestMethod]
        public async Task FireRed_Property_OK_Player_Index0()
        {
            await Load_GBA_PokemonFireRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.name", 0x02024600, [0xBB, 0xBB, 0xBC, 0xBC, 0xBD, 0xBD, 0xFF, 0xFF], "AABBCC");
            mapper.AssertAreEqual("player.gender", 0x02024608, [0x00], "Male");
            mapper.AssertAreEqual("player.badges.0", 0x0202651A, [0x01], true);
            mapper.AssertAreEqual("player.badges.1", 0x0202651A, [0x01], false);
        }
        [TestMethod]
        public async Task FireRed_Property_OK_BattleStructure_Index0()
        {
            await Load_GBA_PokemonFireRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("battle.outcome", null);
            mapper.AssertAreEqual("battle.battle_flags.trainer", 0x02022B4C, [0x0C, 0x00, 0x00, 0x00], true);
            mapper.AssertAreEqual("battle.battle_flags.double", 0x02022B4C, [0x0C, 0x00, 0x00, 0x00], false);
            mapper.AssertAreEqual("battle.field.weather", 0x02023F1C, [0x00], null);
            mapper.AssertAreEqual("battle.field.weather_count", 0x02023F48, [0x00], 0);
            // mapper.AssertAreEqual("battle.player.active_pokemon.species", 0x02024084, [0x18, 0x01], "Torchic");
            // mapper.AssertAreEqual("battle.player.active_pokemon.ability", 0x020240A4, [0x42], "Blaze");
            // mapper.AssertAreEqual("battle.player.active_pokemon.type_1", 0x020240A5, [0x0A], "Fire");
            // mapper.AssertAreEqual("battle.player.active_pokemon.stats.hp", 0x020240AC, [0x18, 0x00], 24);
            // mapper.AssertAreEqual("battle.player.active_pokemon.stats.attack", 0x02024086, [0x19, 0x00], 25);
            // mapper.AssertAreEqual("battle.player.active_pokemon.stats.defense", 0x02024088, [0x13, 0x00], 19);
            // mapper.AssertAreEqual("battle.player.active_pokemon.stats.speed", 0x0202408A, [0x14, 0x00], 20);
            // mapper.AssertAreEqual("battle.player.active_pokemon.stats.special_attack", 0x0202408C, [0x16, 0x00], 22);
            // mapper.AssertAreEqual("battle.player.active_pokemon.stats.special_defense", 0x0202408E, [0x15, 0x00], 21);
            // mapper.AssertAreEqual("battle.player.active_pokemon.moves.0.move", 0x02024090, [0x0A, 0x00], "Scratch");
            // mapper.AssertAreEqual("battle.player.active_pokemon.ivs.hp", 0x02024098, [0x3B, 0x4B, 0x3B, 0x1E], 27);
            // mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.attack", 0x0202409D, [0x06], 0);

            mapper.AssertAreEqual("battle.opponent.trainer", 0x020386AE, [0x69, 0x00], "BUG_CATCHER_COLTON");
            mapper.AssertAreEqual("battle.opponent.id", 0x020386AE, [0x69, 0x00], 105);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.species", 0x02023C3C, [0x0D, 0x00], "Weedle");
        }
    }
}
