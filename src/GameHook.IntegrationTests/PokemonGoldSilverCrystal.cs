using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace GameHook.IntegrationTests
{
    [TestClass]
    public class PokemonGoldSilverCrystal : BaseTest
    {
        //Crystal Tests
        [TestMethod]
        public async Task Crystal_Property_OK_PokemonPartyStructure()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.team.0.species", 0xDCDF, [0x9C], "Quilava");
            mapper.AssertAreEqual("player.team.0.level", 0xDCFE, [0x10], 16);
            mapper.AssertAreEqual("player.team.0.moves.0.move", 0xDCE1, [0x34], "Ember");
            mapper.AssertAreEqual("player.team.0.moves.0.pp", 0xDCF6, [0x02], 2);
            mapper.AssertAreEqual("player.team.0.moves.0.pp_up", 0xDCF6, [0x02], 0);
            mapper.AssertAreEqual("player.team.0.hidden_power.type", "Ground");
            mapper.AssertAreEqual("player.team.0.hidden_power.power", 41);
            mapper.AssertAreEqual("player.team.0.ivs.hp", 6);
            mapper.AssertAreEqual("player.team.0.ivs.attack", 0xDCF4, [0x4B], 4);
            mapper.AssertAreEqual("player.team.0.ivs.defense", 0xDCF4, [0x4B], 11);
            mapper.AssertAreEqual("player.team.0.evs.hp", 0xDCEA, [0x08, 0x75], 2165);
            mapper.AssertAreEqual("player.team.0.evs.attack", 0xDCEC, [0x0A, 0x27], 2599);

            mapper.AssertAreEqual("player.team.1.species", 0xDD0F, [0x00], null);
            mapper.AssertAreEqual("player.team.1.level", 0xDD2E, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.stats.hp", 0xDD31, [0x00, 0x00], 0);
            mapper.AssertAreEqual("player.team.1.moves.0.move", 0xDD11, [0x00], null);
            mapper.AssertAreEqual("player.team.1.moves.0.pp", 0xDD26, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.moves.0.pp_up", 0xDD26, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.hidden_power.type", "Fighting");
            mapper.AssertAreEqual("player.team.1.hidden_power.power", 31);
            mapper.AssertAreEqual("player.team.1.ivs.hp", 0);
            mapper.AssertAreEqual("player.team.1.ivs.attack", 0xDD24, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.ivs.special", 0xDD25, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.evs.hp", 0xDD1A, [0x00, 0x00], 0);
            mapper.AssertAreEqual("player.team.1.evs.attack", 0xDD1C, [0x00, 0x00], 0);
        }
        [TestMethod]
        public async Task Crystal_Property_OK_Audio()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("audio.current_sound", 0xC2BF, [0x2A], 42);
            mapper.AssertAreEqual("audio.map_music", 0xC2C0, [0x1B], 27);
        }
        [TestMethod]
        public async Task Crystal_Property_OK_BattleStructure()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("battle.mode", 0xD22D, [0x02], "Trainer");
            mapper.AssertAreEqual("battle.outcome", null);
            mapper.AssertAreEqual("battle.other.battle_start", 0xD264, [0x00], 0);
            mapper.AssertAreEqual("battle.other.text_buffer", 0xD073, [0x84, 0x8C, 0x81, 0x84, 0x91, 0x50, 0x85, 0x8B, 0x80, 0x8C, 0x84], "EMBER");
            mapper.AssertAreEqual("battle.player.active_pokemon.species", 0xC62C, [0x9C], "Quilava");
            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.special_attack", 0xC6CF, [0x07], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.stats.special_attack", 0xC646, [0x00, 0x21], 33);
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.2.move", 0xC630, [0x6C], "Smokescreen"); //check
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.2.pp", 0xC636, [0x14], 20);
            mapper.AssertAreEqual("battle.opponent.trainer", 0xD22F, [0x01], "FALKNER");

            mapper.AssertAreEqual("battle.opponent.id", 0xD231, [0x01], 1);
            mapper.AssertAreEqual("battle.opponent.team_count", 0xD280, [0x02], 2);
            mapper.AssertAreEqual("battle.opponent.party_position", 0xC663, [0x01], 1);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.hp", 0xD216, [0x00, 0x00], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.0.move", 0xD208, [0x21], "Tackle");
        }
        [TestMethod]
        public async Task Crystal_Property_OK_Events()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("flags.beat_champion", 0xD4B5, [0x00], false);
        }
        [TestMethod]
        public async Task Crystal_Property_OK_Time()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("game_time.hours", 0xD4C4, [0x00, 0x00], 0);
            mapper.AssertAreEqual("game_time.minutes", 0xD4C6, [0x33], 51);
            mapper.AssertAreEqual("game_time.seconds", 0xD4C7, [0x0B], 11);
            mapper.AssertAreEqual("game_time.frames", 0xD4C8, [0x15], 21);

            mapper.AssertAreEqual("time.current.day", 0xD4CB, [0x00], "Sunday");
            mapper.AssertAreEqual("time.current.hour", 0xFF94, [0x0A], 10);
            mapper.AssertAreEqual("time.current.minute", 0xFF96, [0x33], 51);
            mapper.AssertAreEqual("time.current.second", 0xFF98, [0x36], 54);
            mapper.AssertAreEqual("time.current.dst", 0xD4C2, [0x80], true);
            mapper.AssertAreEqual("time.current.time_of_day", 0xD269, [0x01], "Day");
        }
        [TestMethod]
        public async Task Crystal_Property_OK_MetaProperties()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("meta.generation", 2);
            mapper.AssertAreEqual("meta.game_name", "Crystal");
            mapper.AssertAreEqual("meta.game_type", "Third Version");
            mapper.AssertAreEqual("meta.state", "Battle");
        }
        [TestMethod]
        public async Task Crystal_Property_OK_Options()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("options.text_speed_2", 0xCFCC, [0xC1], false);
            mapper.AssertAreEqual("options.text_speed_3", 0xCFCC, [0xC1], false);
            mapper.AssertAreEqual("options.battle_style", 0xCFCC, [0xC1], true);
            mapper.AssertAreEqual("options.battle_animations", 0xCFCC, [0xC1], true);
        }
        [TestMethod]
        public async Task Crystal_Property_OK_Overworld()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("overworld.map_name", 0xDCB5, [0x0A, 0x07], "Violet City - Gym");
            mapper.AssertAreEqual("overworld.map_index", 0xDCB6, [0x07], 7);
            mapper.AssertAreEqual("overworld.y", 0xDCB7, [0x02], 2);
            mapper.AssertAreEqual("overworld.x", 0xDCB8, [0x05], 5);
            mapper.AssertAreEqual("overworld.encounter_rates.morning", 0xD25A, [0x00], 0);
            mapper.AssertAreEqual("overworld.encounter_rates.day", 0xD25B, [0x00], 0);
            mapper.AssertAreEqual("overworld.encounter_rates.night", 0xD25C, [0x00], 0);
            mapper.AssertAreEqual("overworld.encounter_rates.water", 0xD25D, [0x00], 0);
        }
        [TestMethod]
        public async Task Crystal_Property_OK_PlayerBagStructure()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("bag.money", 0xD84E, [0x00, 0x12, 0xC8], 4808);
            mapper.AssertAreEqual("bag.coins", 0xD855, [0x00, 0x00], 0);
            mapper.AssertAreEqual("bag.item_count", 0xD892, [0x04], 4);
            mapper.AssertAreEqual("bag.items.0.item", 0xD893, [0xAD], "Berry");
            mapper.AssertAreEqual("bag.items.0.quantity", 0xD894, [0x01], 1);
            mapper.AssertAreEqual("bag.items.2.item", 0xD897, [0x53], "Bitter Berry");
            mapper.AssertAreEqual("bag.items.2.quantity", 0xD898, [0x01], 1);
        }
        [TestMethod]
        public async Task Crystal_Property_OK_Screen()
        {
            await Load_GBC_PokemonCrystal(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("screen.text.prompt", 0xC606, [0xEE], "▼");
            mapper.AssertAreEqual("screen.menu.menu_flags", 0xCF81, [0x41], 65);
            mapper.AssertAreEqual("screen.column_1.tiles.6", 0x98CC, [0x7F], 127);
        }

        //Gold and Silver Tests
        [TestMethod]
        public async Task Gold_Property_OK_PokemonPartyStructure()
        {
            await Load_GBC_PokemonGold(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.team.0.species", 0xDA2A, [0x9B], "Cyndaquil");
            mapper.AssertAreEqual("player.team.0.level", 0xDA49, [0x05], 5);
            mapper.AssertAreEqual("player.team.0.moves.0.move", 0xDA2C, [0x21], "Tackle");
            mapper.AssertAreEqual("player.team.0.moves.0.pp", 0xDA41, [0x22], 34);
            mapper.AssertAreEqual("player.team.0.moves.0.pp_up", 0xDA41, [0x22], 0);
            mapper.AssertAreEqual("player.team.0.hidden_power.type", "Electric");
            mapper.AssertAreEqual("player.team.0.hidden_power.power", 53);
            mapper.AssertAreEqual("player.team.0.ivs.hp", 4);
            mapper.AssertAreEqual("player.team.0.ivs.attack", 0xDA3F, [0xA7], 10);
            mapper.AssertAreEqual("player.team.0.ivs.defense", 0xDA3F, [0xA7], 7);
            mapper.AssertAreEqual("player.team.0.evs.hp", 0xDA35, [0x00, 0x00], 0);
            mapper.AssertAreEqual("player.team.0.evs.attack", 0xDA37, [0x00, 0x00], 0);

            mapper.AssertAreEqual("player.team.1.species", 0xDA5A, [0x00], null);
            mapper.AssertAreEqual("player.team.1.level", 0xDA79, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.stats.hp", 0xDA7C, [0x00, 0x00], 0);
            mapper.AssertAreEqual("player.team.1.moves.0.move", 0xDA5C, [0x00], null);
            mapper.AssertAreEqual("player.team.1.moves.0.pp", 0xDA71, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.moves.0.pp_up", 0xDA71, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.hidden_power.type", "Fighting");
            mapper.AssertAreEqual("player.team.1.hidden_power.power", 31);
            mapper.AssertAreEqual("player.team.1.ivs.hp", 0);
            mapper.AssertAreEqual("player.team.1.ivs.attack", 0xDA6F, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.ivs.special", 0xDA70, [0x00], 0);
            mapper.AssertAreEqual("player.team.1.evs.hp", 0xDA65, [0x00, 0x00], 0);
            mapper.AssertAreEqual("player.team.1.evs.attack", 0xDA67, [0x00, 0x00], 0);
        }
        [TestMethod]
        public async Task Gold_Property_OK_BattleStructure()
        {
            await Load_GBC_PokemonGold(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("battle.mode", 0xD116, [0x01], "Wild");
            mapper.AssertAreEqual("battle.outcome", null);
            mapper.AssertAreEqual("battle.player.active_pokemon.species", 0xCB0C, [0x9B], "Cyndaquil");
        }
        [TestMethod]
        public async Task Gold_Property_OK_Time()
        {
            await Load_GBC_PokemonGold(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("game_time.hours", 0xD1EB, [0x00, 0x00], 0);
            mapper.AssertAreEqual("game_time.minutes", 0xD1ED, [0x02], 2);
            mapper.AssertAreEqual("game_time.seconds", 0xD1EE, [0x31], 49);
            mapper.AssertAreEqual("game_time.frames", 0xD1EF, [0x2D], 45);

            mapper.AssertAreEqual("time.current.day", 0xD1F2, [0x00], "Sunday");
            mapper.AssertAreEqual("time.current.hour", 0xFF96, [0x0A], 10);
            mapper.AssertAreEqual("time.current.minute", 0xFF98, [0x03], 3);
            mapper.AssertAreEqual("time.current.second", 0xFF9A, [0x06], 6);
            mapper.AssertAreEqual("time.current.dst", 0xD1E8, [0x80], true);
            mapper.AssertAreEqual("time.current.time_of_day", 0xD157, [0x01], "Day");
        }
        [TestMethod]
        public async Task Gold_Property_OK_MetaProperties()
        {
            await Load_GBC_PokemonGold(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("meta.generation", 2);
            mapper.AssertAreEqual("meta.game_name", "Gold and Silver");
            mapper.AssertAreEqual("meta.game_type", "Originals");
            mapper.AssertAreEqual("meta.state", "Battle");
        }
        [TestMethod]
        public async Task Gold_Property_OK_Options()
        {
            await Load_GBC_PokemonGold(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("options.text_speed_2", 0xD199, [0xC1], false);
            mapper.AssertAreEqual("options.text_speed_3", 0xD199, [0xC1], false);
            mapper.AssertAreEqual("options.battle_style", 0xD199, [0xC1], true);
            mapper.AssertAreEqual("options.battle_animations", 0xD199, [0xC1], true);
        }
        [TestMethod]
        public async Task Gold_Property_OK_Overworld()
        {
            await Load_GBC_PokemonGold(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("overworld.map_name", 0xDA00, [0x18, 0x03], "Route 29");
            mapper.AssertAreEqual("overworld.map_index", 0xDA01, [0x03], 3);
            mapper.AssertAreEqual("overworld.y", 0xDA02, [0x0D], 13);
            mapper.AssertAreEqual("overworld.x", 0xDA03, [0x2C], 44);
            mapper.AssertAreEqual("overworld.encounter_rates.morning", 0xCDB5, [0x00], 0);
            mapper.AssertAreEqual("overworld.encounter_rates.day", 0xCDB6, [0x00], 0);
            mapper.AssertAreEqual("overworld.encounter_rates.night", 0xCDB7, [0x00], 0);
            mapper.AssertAreEqual("overworld.encounter_rates.water", 0xCDB8, [0x00], 0);
        }
        [TestMethod]
        public async Task Gold_Property_OK_PlayerBagStructure()
        {
            await Load_GBC_PokemonGold(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("bag.money", 0xD573, [0x00, 0x0B, 0xB8], 3000);
            mapper.AssertAreEqual("bag.coins", 0xD57A, [0x00, 0x00], 0);
            mapper.AssertAreEqual("bag.item_count", 0xD5B7, [0x01], 1);
            mapper.AssertAreEqual("bag.items.0.item", 0xD5B8, [0x12], "Potion");
            mapper.AssertAreEqual("bag.items.0.quantity", 0xD5B9, [0x01], 1);
        }
        [TestMethod]
        public async Task Gold_Property_OK_Screen()
        {
            await Load_GBC_PokemonGold(1);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("screen.text.prompt", 0xC606, [0x7A], "");
            mapper.AssertAreEqual("screen.menu.menu_flags", 0xCEB8, [0x40], 64);
            mapper.AssertAreEqual("screen.column_1.tiles.6", 0x98CC, [0x06], 6);
        }
    }
}
