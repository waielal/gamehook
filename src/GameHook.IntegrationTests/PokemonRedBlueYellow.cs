using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace GameHook.IntegrationTests
{
    [TestClass]
    public class PokemonRedBlueYellow : BaseTest
    {
        [TestMethod]
        public async Task Red_Property_OK_PokemonPartyStructure()
        {
            // TODO: Confirm these are correct.
            await Load_GB_PokemonRed();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.team.0.species", 0xD16B, [0x15], "Mew");
            mapper.AssertAreEqual("player.team.0.level", 0xD18C, [0x64], 100);
            mapper.AssertAreEqual("player.team.0.moves.0.move", 0xD173, [0x9D], "Rock Slide");
            mapper.AssertAreEqual("player.team.0.moves.0.pp", 0xD188, [0xD0], 16);
            mapper.AssertAreEqual("player.team.0.moves.0.pp_up", 0xD188, [0xD0], 3);
        }
        [TestMethod]
        public async Task Yellow_Property_OK_PokemonPartyStructure()
        {
            // TODO: Confirm these are correct.
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.team_count", 0xD162, [0x04], 4);
            mapper.AssertAreEqual("player.team.0.species", 0xD16A, [0x3C], "Tauros");
            mapper.AssertAreEqual("player.team.0.level", 0xD18B, [0x2B], 43);
            mapper.AssertAreEqual("player.team.0.stats.hp", 0xD16B, [0x00, 0x24], 36);
            mapper.AssertAreEqual("player.team.0.moves.0.move", 0xD172, [0x22], "Body Slam");
            mapper.AssertAreEqual("player.team.0.moves.0.pp", 0xD187, [0x45], 5);
            mapper.AssertAreEqual("player.team.0.moves.0.pp_up", 0xD187, [0x45], 1);
            mapper.AssertAreEqual("player.team.0.ivs.hp", 15);
            mapper.AssertAreEqual("player.team.0.ivs.attack", 0xD185, [0xFF], 15);
            mapper.AssertAreEqual("player.team.0.ivs.special", 0xD186, [0xFF], 15);
            mapper.AssertAreEqual("player.team.0.evs.hp", 0xD17B, [0x2A, 0x11], 10769);
            mapper.AssertAreEqual("player.team.0.evs.attack", 0xD17D, [0x61, 0x36], 24886);

            mapper.AssertAreEqual("player.team.1.species", 0xD196, [0x24], "Pidgey");
            mapper.AssertAreEqual("player.team.1.level", 0xD1B7, [0x06], 6);
            mapper.AssertAreEqual("player.team.1.stats.hp", 0xD197, [0x00, 0x15], 21);
            mapper.AssertAreEqual("player.team.1.moves.0.move", 0xD19E, [0x10], "Gust");
            mapper.AssertAreEqual("player.team.1.moves.0.pp", 0xD1B3, [0x23], 35);
            mapper.AssertAreEqual("player.team.1.moves.0.pp_up", 0xD1B3, [0x23], 0);
            mapper.AssertAreEqual("player.team.1.ivs.hp", 8);
            mapper.AssertAreEqual("player.team.1.ivs.attack", 0xD1B1, [0xFE], 15);
            mapper.AssertAreEqual("player.team.1.ivs.special", 0xD1B2, [0x48], 8);
            mapper.AssertAreEqual("player.team.1.evs.hp", 0xD1A7, [0x00, 0x00], 0);
            mapper.AssertAreEqual("player.team.1.evs.attack", 0xD1A9, [0x00, 0x00], 0);
        }
        // The following Yellow tests are using a gamestate one tile in front of the door of Fuchsia City Gym with a Tauros in the first slot of the player's party.
        [TestMethod]
        public async Task Yellow_Property_OK_PlayerBagStructure()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("bag.money", 0xD346, [0x04, 0x00, 0x31], 40031);
            mapper.AssertAreEqual("bag.coins", 0xD5A3, [0x00, 0x00], 0);
            mapper.AssertAreEqual("bag.item_count", 0xD31C, [0x13], 19);
            mapper.AssertAreEqual("bag.items.0.item", 0xD31D, [0x06], "BICYCLE");
            mapper.AssertAreEqual("bag.items.0.quantity", 0xD31E, [0x01], 1);
            mapper.AssertAreEqual("bag.items.2.item", 0xD321, [0x38], "SUPER REPEL");
            mapper.AssertAreEqual("bag.items.2.quantity", 0xD322, [0x06], 6);
        }
        [TestMethod]
        public async Task Yellow_Property_OK_MetaProperties()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("meta.generation", 1);
            mapper.AssertAreEqual("meta.game_name", "Yellow");
            mapper.AssertAreEqual("meta.game_type", "Third Version");
            mapper.AssertAreEqual("meta.state", "Overworld");
        }
        [TestMethod]
        public async Task Yellow_Property_OK_Audio()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("audio.current_sound", 0xC001, [0xC7], 199);
            mapper.AssertAreEqual("audio.overworld_track_current_map", 0xD35A, [0xC7], "Cerulean City, Fuchsia City");
            mapper.AssertAreEqual("audio.volume_channels.0", 0xC0DE, [0xB6], 182);
        }
        [TestMethod]
        public async Task Yellow_Property_OK_GameTime()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("game_time.hours", 0xDA3F, [0x00, 0x07], 7);
            mapper.AssertAreEqual("game_time.minutes", 0xDA41, [0x00, 0x28], 40);
            mapper.AssertAreEqual("game_time.seconds", 0xDA43, [0x19], 25);
            mapper.AssertAreEqual("game_time.frames", 0xDA44, [0x33], 51);
        }
        [TestMethod]
        public async Task Yellow_Property_OK_Options()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("options.text_speed_1", 0xD354, [0xC1], false);
            mapper.AssertAreEqual("options.solo_challenge", 0xD354, [0xC1], "Fast Text, No Animations, Battlestyle Set");
        }
        [TestMethod]
        public async Task Yellow_Property_OK_BattleStructure()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("battle.mode", 0xD056, [0x00], null);
            mapper.AssertAreEqual("battle.other.low_health_alarm", 0xCCF6, [0x01], "Disabled");
            mapper.AssertAreEqual("battle.other.battle_start", 0xCCF5, [0x01], 1);
            mapper.AssertAreEqual("battle.player.active_pokemon.nickname", 0xD008, [0x81, 0x84, 0x84, 0x85, 0x50, 0x81, 0x80, 0x8B, 0x8B, 0x50, 0x00], "BEEF");
            mapper.AssertAreEqual("battle.player.active_pokemon.species", 0xD013, [0x3C], "Tauros");
            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.special", 0xCD1D, [0x06], "-1");
            mapper.AssertAreEqual("battle.player.active_pokemon.stats.special", 0xD02A, [0x00, 0x3B], 59);
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.3.move", 0xD01E, [0x59], "Earthquake");
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.3.pp", 0xD02F, [0x8B], 11);
            mapper.AssertAreEqual("battle.opponent.name", 0xD049, [0x8A, 0x8E, 0x86, 0x80, 0x50, 0x81, 0x8B, 0x80, 0x88, 0x8D, 0x84], "KOGA");
            mapper.AssertAreEqual("battle.opponent.trainer", 0xD030, [0x26], "Koga");

            mapper.AssertAreEqual("battle.opponent.id", 0xD05C, [0x01], 1);
            mapper.AssertAreEqual("battle.opponent.team_count", 0xD89B, [0x04], 4);
            mapper.AssertAreEqual("battle.opponent.party_position", 0xCFE7, [0x03], 3);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.hp", 0xCFE5, [0x00, 0x00], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.0.move", 0xCFEC, [0x8D], "Leech Life");
        }
        [TestMethod]
        public async Task Yellow_Property_OK_Events()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();
            mapper.AssertAreEqual("flags.beat_champion", 0xD866, [0x00], false);
            mapper.AssertAreEqual("events.trash_can_puzzle", 0xD772, [0xDF], true);
            mapper.AssertAreEqual("flags.count_play_time", 0xD731, [0x01], true);
        }
        [TestMethod]
        public async Task Yellow_Property_OK_Overworld()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();
            mapper.AssertAreEqual("overworld.map_name", 0xD35D, [0x07], "Fuchsia City");
            mapper.AssertAreEqual("overworld.y", 0xD360, [0x1C], 28);
            mapper.AssertAreEqual("overworld.x", 0xD361, [0x05], 5);
            mapper.AssertAreEqual("overworld.encounter_table.common.2.level", 0xD88B, [0x16], 22);
            mapper.AssertAreEqual("overworld.encounter_table.common.2.species", 0xD88C, [0x0C], "Exeggcute");
            mapper.AssertAreEqual("overworld.encounter_rate", 0xD886, [0x00], 0);
            mapper.AssertAreEqual("overworld.map_data.palette", 0xD35C, [0x00], 0);
        }
        [TestMethod]
        public async Task Yellow_Property_OK_Screen()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();
            mapper.AssertAreEqual("screen.text.prompt", 0xC4F2, [0x23], "");
            mapper.AssertAreEqual("screen.menu.current_item", 0xCC26, [0x00], 0);
            mapper.AssertAreEqual("screen.column_1.tiles.6", 0x9CCC, [0x1F], 31);
        }
        // The following Red and Blue tests are using a gamestate one tile in front of the champion with a Beedrill in the first slot of the player's party.
        [TestMethod]
        public async Task Red_Property_OK_PlayerBagStructure()
        {
            await Load_GB_PokemonRed(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("bag.money", 0xD347, [0x06, 0x13, 0x35], 61335);
            mapper.AssertAreEqual("bag.coins", 0xD5A4, [0x00, 0x00], 0);
            mapper.AssertAreEqual("bag.item_count", 0xD31D, [0x10], 16);
            mapper.AssertAreEqual("bag.items.0.item", 0xD31E, [0x10], "FULL RESTORE");
            mapper.AssertAreEqual("bag.items.0.quantity", 0xD31F, [0x02], 2);
            mapper.AssertAreEqual("bag.items.2.item", 0xD322, [0x52], "ELIXER");
            mapper.AssertAreEqual("bag.items.2.quantity", 0xD323, [0x01], 1);
        }
        [TestMethod]
        public async Task Red_Property_OK_MetaProperties()
        {
            await Load_GB_PokemonRed(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("meta.generation", 1);
            mapper.AssertAreEqual("meta.game_name", "Red and Blue");
            mapper.AssertAreEqual("meta.game_type", "Originals");
            mapper.AssertAreEqual("meta.state", "Overworld");
        }
        [TestMethod]
        public async Task Red_Property_OK_Audio()
        {
            await Load_GB_PokemonRed(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("audio.current_sound", 0xC002, [0x00], 0);
            mapper.AssertAreEqual("audio.overworld_track_current_map", 0xD359, [0x8E], null);
            mapper.AssertAreEqual("audio.volume_channels.0", 0xC0DF, [0xC7], 199);
        }
        [TestMethod]
        public async Task Red_Property_OK_GameTime()
        {
            await Load_GB_PokemonRed(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("game_time.hours", 0xDA40, [0x00, 0x04], 4);
            mapper.AssertAreEqual("game_time.minutes", 0xDA42, [0x00, 0x2B], 43);
            mapper.AssertAreEqual("game_time.seconds", 0xDA44, [0x20], 32);
            mapper.AssertAreEqual("game_time.frames", 0xDA45, [0x0B], 11);
        }
        [TestMethod]
        public async Task Red_Property_OK_Options()
        {
            await Load_GB_PokemonRed(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("options.text_speed_1", 0xD355, [0x41], false);
            mapper.AssertAreEqual("options.solo_challenge", 0xD355, [0x41], "Champion Fight");
        }
        [TestMethod]
        public async Task Red_Property_OK_BattleStructure()
        {
            await Load_GB_PokemonRed(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("battle.mode", 0xD057, [0x00], null);
            mapper.AssertAreEqual("battle.other.low_health_alarm", 0xCCF6, [0x00], "Enabled");
            mapper.AssertAreEqual("battle.other.battle_start", 0xCCF5, [0x00], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.nickname", 0xD009, [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00], "");
            mapper.AssertAreEqual("battle.player.active_pokemon.species", 0xD014, [0x00], null);
            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.special", 0xCD1D, [0x00], null);
            mapper.AssertAreEqual("battle.player.active_pokemon.stats.special", 0xD029, [0x00, 0x00], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.3.move", 0xD01F, [0x00], null);
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.3.pp", 0xD030, [0x00], 0);
            mapper.AssertAreEqual("battle.opponent.name", 0xD04A, [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00], "");
            mapper.AssertAreEqual("battle.opponent.trainer", 0xD031, [0x00], null);

            mapper.AssertAreEqual("battle.opponent.id", 0xD05D, [0x00], 0);
            mapper.AssertAreEqual("battle.opponent.team_count", 0xD89C, [0x05], 5);
            mapper.AssertAreEqual("battle.opponent.party_position", 0xCFE8, [0x00], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.hp", 0xCFE6, [0x00, 0x00], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.0.move", 0xCFED, [0x00], null);
        }
        [TestMethod]
        public async Task Red_Property_OK_Events()
        {
            await Load_GB_PokemonRed(1);

            var mapper = await GameHookClient.GetMapperAsync();
            mapper.AssertAreEqual("flags.beat_champion", 0xD867, [0x00], false);
            mapper.AssertAreEqual("events.trash_can_puzzle", 0xD773, [0x03], true);
            mapper.AssertAreEqual("flags.count_play_time", 0xD732, [0x01], true);
        }
        [TestMethod]
        public async Task Red_Property_OK_Overworld()
        {
            await Load_GB_PokemonRed(1);

            var mapper = await GameHookClient.GetMapperAsync();
            mapper.AssertAreEqual("overworld.map_name", 0xD35E, [0x78], "Champion's Room");
            mapper.AssertAreEqual("overworld.y", 0xD361, [0x03], 3);
            mapper.AssertAreEqual("overworld.x", 0xD362, [0x04], 4);
            mapper.AssertAreEqual("overworld.encounter_table.common.2.level", 0xD88C, [0x1A], 26);
            mapper.AssertAreEqual("overworld.encounter_table.common.2.species", 0xD88D, [0x05], "Spearow");
            mapper.AssertAreEqual("overworld.encounter_rate", 0xD887, [0x00], 0);
            mapper.AssertAreEqual("overworld.map_data.palette", 0xD35D, [0x00], 0);
        }
        [TestMethod]
        public async Task Red_Property_OK_Screen()
        {
            await Load_GB_PokemonRed(1);

            var mapper = await GameHookClient.GetMapperAsync();
            mapper.AssertAreEqual("screen.text.prompt", 0xC4F2, [0xEE], "▼");
            mapper.AssertAreEqual("screen.menu.current_item", 0xCC26, [0x00], 0);
            mapper.AssertAreEqual("screen.column_1.tiles.6", 0x9CCD, [0x11], 17);
        }
    }
}
