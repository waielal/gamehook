using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using System.Threading.Tasks;

namespace GameHook.IntegrationTests
{
    [TestClass]
    public class PokemonHeartGold : BaseTest
    {
        [TestMethod]
        public async Task Property_OK_Names()
        {
            await Load_NDS_PokemonHeartGold ();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.name", 0x227C2FC, [0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01], "AAAAAAA");
            mapper.AssertAreEqual("player.team.0.nickname", 0x48, [0x37, 0x01, 0x2F, 0x01, 0x31, 0x01, 0x2B, 0x01, 0x38, 0x01, 0x33, 0x01, 0x3F, 0x01, 0x37, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00], "MEGANIUM");
        }

        [TestMethod]
        public async Task Property_OK_Player()
        {
            await Load_NDS_PokemonHeartGold();
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.player_id", 0x227C30C, [0xEB, 0x10], 4331);
            mapper.AssertAreEqual("player.secret_id", 0x227C30E, [0x4C, 0xCD], 52556);
            mapper.AssertAreEqual("player.team_count", 0x227C32C, [0x03], 3);

            mapper.AssertAreEqual("player.badges.0", 0x227C316, [0x03], true);
            mapper.AssertAreEqual("player.badges.1", 0x227C316, [0x03], true);
            mapper.AssertAreEqual("player.badges.2", 0x227C316, [0x03], false);
            mapper.AssertAreEqual("player.badges.3", 0x227C316, [0x03], false);
            mapper.AssertAreEqual("player.badges.4", 0x227C316, [0x03], false);
            mapper.AssertAreEqual("player.badges.5", 0x227C316, [0x03], false);
            mapper.AssertAreEqual("player.badges.6", 0x227C316, [0x03], false);
            mapper.AssertAreEqual("player.badges.7", 0x227C316, [0x03], false);
        }

        [TestMethod]
        public async Task Property_OK_PokemonPartyStructure_Index0()
        {
            await Load_NDS_PokemonHeartGold();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("meta.global_pointer", 0x211186C, [0xA8, 0xF2, 0x26, 0x02], 36106920);

            mapper.AssertAreEqual("player.team.0.species", 0x08, [0x9A, 0x00], "Meganium");
            mapper.AssertAreEqual("player.team.0.dex_number", 0x08, [0x9A, 0x00], 154);
            mapper.AssertAreEqual("player.team.0.nature", [0xAD, 0xCB, 0x7E, 0xE7], "Careful");
            mapper.AssertAreEqual("player.team.0.ability", 0x15, [0x41], "Overgrow");
            mapper.AssertAreEqual("player.team.0.level", 0x8C, [0x23], 35);
            mapper.AssertAreEqual("player.team.0.evs.hp", 0x18, [0x0D], 13);
            mapper.AssertAreEqual("player.team.0.ot_id", 0x0C, [0xEB, 0x10], 4331);
            mapper.AssertAreEqual("player.team.0.exp", 0x10, [0xC7, 0x92, 0x00, 0x00], 37575);
            mapper.AssertAreEqual("player.team.0.held_item", 0x0A, [0xEF, 0x00], "Miracle Seed");
            mapper.AssertAreEqual("player.team.0.friendship", 0x14, [0xC5], 197);
            mapper.AssertAreEqual("player.team.0.pokerus", 0x82, [0x00], 0);
            mapper.AssertAreEqual("player.team.0.status_condition", 0x88, [0x00], null);
            mapper.AssertAreEqual("player.team.0.ot_name", 0x68, [0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01, 0x2B, 0x01, 0xFF, 0xFF], "AAAAAAA");

            // Moves
            mapper.AssertAreEqual("player.team.0.moves.0.move", 0x28, [0x50, 0x00], "Petal Dance");
            mapper.AssertAreEqual("player.team.0.moves.0.pp", 0x30, [0x13], 19);
            mapper.AssertAreEqual("player.team.0.moves.0.pp_up", 0x34, [0x00], 0);
            mapper.AssertAreEqual("player.team.0.moves.1.move", 0x2A, [0xEB, 0x00], "Synthesis");
            mapper.AssertAreEqual("player.team.0.moves.1.pp", 0x31, [0x05], 5);
            mapper.AssertAreEqual("player.team.0.moves.1.pp_up", 0x35, [0x00], 0);
            mapper.AssertAreEqual("player.team.0.moves.2.move", 0x2C, [0x4B, 0x00], "Razor Leaf");
            mapper.AssertAreEqual("player.team.0.moves.2.pp", 0x32, [0x13], 19);
            mapper.AssertAreEqual("player.team.0.moves.2.pp_up", 0x36, [0x00], 0);
            mapper.AssertAreEqual("player.team.0.moves.3.move", 0x2E, [0x73, 0x00], "Reflect");
            mapper.AssertAreEqual("player.team.0.moves.3.pp", 0x33, [0x13], 19);
            mapper.AssertAreEqual("player.team.0.moves.3.pp_up", 0x37, [0x00], 0);

            // Stats
            mapper.AssertAreEqual("player.team.0.stats.hp", 0x8E, [0x35, 0x00], 53);
            mapper.AssertAreEqual("player.team.0.stats.hp_max", 0x90, [0x69, 0x00], 105);
            mapper.AssertAreEqual("player.team.0.stats.attack", 0x92, [0x40, 0x00], 64);
            mapper.AssertAreEqual("player.team.0.stats.defense", 0x94, [0x50, 0x00], 80);
            mapper.AssertAreEqual("player.team.0.stats.speed", 0x96, [0x42, 0x00], 66);
            mapper.AssertAreEqual("player.team.0.stats.special_attack", 0x98, [0x3F, 0x00], 63);
            mapper.AssertAreEqual("player.team.0.stats.special_defense", 0x9A, [0x53, 0x00], 83);

            // IVs: also checks BitRange function.
            mapper.AssertAreEqual("player.team.0.ivs.hp", 0x38, [0x0A, 0xB4, 0x93, 0x09], 10);
            mapper.AssertAreEqual("player.team.0.ivs.attack", 0x38, [0x0A, 0xB4, 0x93, 0x09], 0);
            mapper.AssertAreEqual("player.team.0.ivs.defense", 0x38, [0x0A, 0xB4, 0x93, 0x09], 13);
            mapper.AssertAreEqual("player.team.0.ivs.speed", 0x38, [0x0A, 0xB4, 0x93, 0x09], 7);
            mapper.AssertAreEqual("player.team.0.ivs.special_attack", 0x38, [0x0A, 0xB4, 0x93, 0x09], 25);
            mapper.AssertAreEqual("player.team.0.ivs.special_defense", 0x38, [0x0A, 0xB4, 0x93, 0x09], 4);

            // EVs
            mapper.AssertAreEqual("player.team.0.evs.hp", 0x18, [0x0D], 13);
            mapper.AssertAreEqual("player.team.0.evs.attack", 0x19, [0x1C], 28);
            mapper.AssertAreEqual("player.team.0.evs.defense", 0x1A, [0x15], 21);
            mapper.AssertAreEqual("player.team.0.evs.speed", 0x1B, [0x24], 36);
            mapper.AssertAreEqual("player.team.0.evs.special_attack", 0x1C, [0x03], 3);
            mapper.AssertAreEqual("player.team.0.evs.special_defense", 0x1D, [0x02], 2);

            // Internals
            mapper.AssertAreEqual("player.team.0.internals.personality_value", 0x00, [0xAD, 0xCB, 0x7E, 0xE7], 3883846573);
            mapper.AssertAreEqual("player.team.0.internals.checksum", 0x06, [0x3C, 0x10], 4156);
            mapper.AssertAreEqual("player.team.0.internals.secret_id", [0xAD, 0xCB], 52141);
            mapper.AssertAreEqual("player.team.0.internals.language", [0x02], "English");
            mapper.AssertAreEqual("player.team.0.misc.hgss_ball", 0x86, [0x04], 4);

            // Flags
            mapper.AssertAreEqual("player.team.0.flags.is_egg", 0x38, [0x0A, 0xB4, 0x93, 0x09], false);
            mapper.AssertAreEqual("player.team.0.flags.is_nicknamed", 0x38, [0x0A, 0xB4, 0x93, 0x09], false);
            mapper.AssertAreEqual("player.team.0.flags.skip_checksum_1", 0x04, [0x00], false);
            mapper.AssertAreEqual("player.team.0.flags.skip_checksum_2", 0x04, [0x00], false);
            mapper.AssertAreEqual("player.team.0.flags.is_bad_egg", 0x04, [0x00], false);

            //Other Pokemon
            mapper.AssertAreEqual("player.team.1.species", 0x08, [0xA3, 0x00], "Hoothoot");
            mapper.AssertAreEqual("player.team.1.internals.personality_value", 0x00, [0x0C, 0x8E, 0x30, 0x94], 2486210060);
            mapper.AssertAreEqual("player.team.2.species", 0x08, [0xAF, 0x00], "Togepi");
            mapper.AssertAreEqual("player.team.2.internals.personality_value", 0x00, [0x45, 0xE7, 0xB2, 0xD2], 3534939973);
            mapper.AssertAreEqual("player.team.3.species", 0x08, [0x00, 0x00], null);
            mapper.AssertAreEqual("player.team.4.species", 0x08, [0x00, 0x00], null);
            mapper.AssertAreEqual("player.team.5.species", 0x08, [0x00, 0x00], null);
        }

        [TestMethod]
        public async Task Property_OK_BattleOutcome()
        {
            await Load_NDS_PokemonHeartGold();
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("battle.mode", "Wild");
            mapper.AssertAreEqual("battle.outcome", null);
            mapper.AssertAreEqual("battle.other.outcome_flags", 0x22C647B, [0x00], 0);
        }

        [TestMethod]
        public async Task Property_OK_Bag()
        {
            await Load_NDS_PokemonHeartGold();
            var mapper = await GameHookClient.GetMapperAsync();
            mapper.AssertAreEqual("bag.money", 0x227C310, [0x16, 0x2E, 0x00, 0x00], 11798);

            mapper.AssertAreEqual("bag.items.0.item", 0x227C8DC, [0x4F, 0x00], "Repel");
            mapper.AssertAreEqual("bag.items.0.quantity", 0x227C8DE, [0x0A, 0x00], 10);
            mapper.AssertAreEqual("bag.items.1.item", 0x227C8E0, [0x00, 0x00], null);
            mapper.AssertAreEqual("bag.items.1.quantity", 0x227C8E2, [0x00, 0x00], 0);

            mapper.AssertAreEqual("bag.medicine.0.item", 0x227CDFC, [0x32, 0x00], "Rare Candy");
            mapper.AssertAreEqual("bag.medicine.0.quantity", 0x227CDFE, [0x08, 0x00], 8);
            mapper.AssertAreEqual("bag.medicine.1.item", 0x227CE00, [0x16, 0x00], "Paralyze Heal");
            mapper.AssertAreEqual("bag.medicine.1.quantity", 0x227CE02, [0x01, 0x00], 1);

            mapper.AssertAreEqual("bag.balls.0.item", 0x227CF9C, [0x04, 0x00], "Poké Ball");
            mapper.AssertAreEqual("bag.balls.0.quantity", 0x227CF9E, [0x04, 0x00], 4);
            mapper.AssertAreEqual("bag.balls.1.item", 0x227CFA0, [0xEC, 0x01], "Fast Ball");
            mapper.AssertAreEqual("bag.balls.1.quantity", 0x227CFA2, [0x01, 0x00], 1);

            mapper.AssertAreEqual("bag.tmhm.0.item", 0x227CC38, [0x7A, 0x01], "TM51");
            mapper.AssertAreEqual("bag.tmhm.0.quantity", 0x227CC3A, [0x01, 0x00], 1);
            mapper.AssertAreEqual("bag.tmhm.1.item", 0x227CC3C, [0x8D, 0x01], "TM70");
            mapper.AssertAreEqual("bag.tmhm.1.quantity", 0x227CC3E, [0x01, 0x00], 1);

            mapper.AssertAreEqual("bag.battle_items.0.item", 0x227CFFC, [0x3C, 0x00], "X Accuracy");
            mapper.AssertAreEqual("bag.mail.0.item", 0x227CDCC, [0x8D, 0x00], "Tunnel Mail");
        }

        [TestMethod]
        public async Task Property_OK_Encounters()
        {
            await Load_NDS_PokemonHeartGold();
            var mapper = await GameHookClient.GetMapperAsync();
            mapper.AssertAreEqual("overworld.encounter_rate", 5);

            mapper.AssertAreEqual("overworld.encounter_rates.walking", 0x22A1CDC, [0x05], 5);
            mapper.AssertAreEqual("overworld.encounter_rates.surfing", 0x22A1CDD, [0x0F], 15);
            mapper.AssertAreEqual("overworld.encounter_rates.headbutt", 0x22A1CDE, [0x00], 0);
            mapper.AssertAreEqual("overworld.encounter_rates.old_rod", 0x22A1CDF, [0x19], 25);
            mapper.AssertAreEqual("overworld.encounter_rates.good_rod", 0x22A1CE0, [0x32], 50);
            mapper.AssertAreEqual("overworld.encounter_rates.super_rod", 0x22A1CE1, [0x4B], 75);

            mapper.AssertAreEqual("overworld.encounters.0", 0x22A1CF0, [0x0A], "Caterpie");
            mapper.AssertAreEqual("overworld.encounters.1", 0x22A1CF2, [0x0B], "Metapod");
            mapper.AssertAreEqual("overworld.encounters.2", 0x22A1CF4, [0x0A], "Caterpie");
            mapper.AssertAreEqual("overworld.encounters.3", 0x22A1CF6, [0x0B], "Metapod");
        }

        [TestMethod]
        public async Task Property_OK_Overworld()
        {
            await Load_NDS_PokemonHeartGold();
            var mapper = await GameHookClient.GetMapperAsync();
            mapper.AssertAreEqual("overworld.map_index", 0x227D4CC, [0x75, 0x00], 117);
            mapper.AssertAreEqual("overworld.x", 0x227D4D4, [0x0B, 0x00, 0x00, 0x00], 11);
            mapper.AssertAreEqual("overworld.y", 0x227D4D8, [0x50, 0x00, 0x00, 0x00], 80);
        }

        [TestMethod]
        public async Task Property_OK_Meta()
        {
            await Load_NDS_PokemonHeartGold();
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("meta.generation", 4);
            mapper.AssertAreEqual("meta.game_name", "HeartGold and SoulSilver");
            mapper.AssertAreEqual("meta.game_type", "Remakes");
            mapper.AssertAreEqual("meta.state", "Battle");
            mapper.AssertAreEqual("meta.state_enemy", "Pokemon In Battle");
            mapper.AssertAreEqual("meta.global_pointer", 0x211186C, [0xA8, 0xF2, 0x26, 0x02], 36106920);
            mapper.AssertAreEqual("meta.enemy_pointer", 0x22A6C18, [0x2B, 0x01, 0xFF, 0xFF], -65237);
        }
        [TestMethod]
        public async Task Property_OK_Time()
        {
            await Load_NDS_PokemonHeartGold();
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("game_time.hours", 0x227C31E, [0x37, 0x00], 55);
            mapper.AssertAreEqual("game_time.minutes", 0x227C320, [0x0E], 14);
            mapper.AssertAreEqual("game_time.seconds", 0x227C321, [0x00], 0);
        }

        [TestMethod]
        public async Task Property_OK_BattleStructure()
        {
            await Load_NDS_PokemonHeartGold();
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("battle.outcome", null);
            mapper.AssertAreEqual("battle.mode", "Wild");
            mapper.AssertAreEqual("battle.player.team_count", 0x22CAD1C, [0x03], 3);

            mapper.AssertAreEqual("battle.player.active_pokemon.species", 0x22C609C, [0x9A, 0x00], "Meganium");
            mapper.AssertAreEqual("battle.player.active_pokemon.nickname", 0x22C60D2, [0x37, 0x01, 0x2F, 0x01, 0x31, 0x01, 0x2B, 0x01, 0x38, 0x01, 0x33, 0x01, 0x3F, 0x01, 0x37, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF], "MEGANIUM");
            mapper.AssertAreEqual("battle.player.active_pokemon.level", 0x22C60D0, [0x23], 35);
            mapper.AssertAreEqual("battle.player.active_pokemon.ability", 0x22C60C3, [0x41], "Overgrow");
            mapper.AssertAreEqual("battle.player.active_pokemon.nature", 0x22C6104, [0xAD, 0xCB, 0x7E, 0xE7], "Careful");
            mapper.AssertAreEqual("battle.player.active_pokemon.held_item", 0x22C6114, [0xEF, 0x00], "Miracle Seed");
            mapper.AssertAreEqual("battle.player.active_pokemon.type_1", 0x22C60C0, [0x0C], "Grass");
            mapper.AssertAreEqual("battle.player.active_pokemon.type_2", 0x22C60C1, [0x0C], "Grass");
            mapper.AssertAreEqual("battle.player.active_pokemon.status_condition", 0x22C6108, [0x00], null);

            mapper.AssertAreEqual("battle.player.active_pokemon.moves.0.move", 0x22C60A8, [0x50, 0x00], "Petal Dance");
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.0.pp", 0x22C60C8, [0x13], 19);
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.1.move", 0x22C60AA, [0xEB, 0x00], "Synthesis");
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.2.move", 0x22C60AC, [0x4B, 0x00], "Razor Leaf");
            mapper.AssertAreEqual("battle.player.active_pokemon.moves.3.move", 0x22C60AE, [0x73, 0x00], "Reflect");

            mapper.AssertAreEqual("battle.player.active_pokemon.stats.hp", 0x22C60E8, [0x35, 0x00], 53);
            mapper.AssertAreEqual("battle.player.active_pokemon.stats.attack", 0x22C609E, [0x40, 0x00], 64);
            mapper.AssertAreEqual("battle.player.active_pokemon.stats.defense", 0x22C60A0, [0x50, 0x00], 80);
            mapper.AssertAreEqual("battle.player.active_pokemon.stats.speed", 0x22C60A2, [0x42, 0x00], 66);
            mapper.AssertAreEqual("battle.player.active_pokemon.stats.special_attack", 0x22C60A4, [0x3F, 0x00], 63);
            mapper.AssertAreEqual("battle.player.active_pokemon.stats.special_defense", 0x22C60A6, [0x53, 0x00], 83);

            mapper.AssertAreEqual("battle.player.active_pokemon.ivs.hp", 0x22C60B0, [0x0A, 0xB4, 0x93, 0x09], 10);
            mapper.AssertAreEqual("battle.player.active_pokemon.ivs.attack", 0x22C60B0, [0x0A, 0xB4, 0x93, 0x09], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.ivs.defense", 0x22C60B0, [0x0A, 0xB4, 0x93, 0x09], 13);
            mapper.AssertAreEqual("battle.player.active_pokemon.ivs.speed", 0x22C60B0, [0x0A, 0xB4, 0x93, 0x09], 7);
            mapper.AssertAreEqual("battle.player.active_pokemon.ivs.special_attack", 0x22C60B0, [0x0A, 0xB4, 0x93, 0x09], 25);
            mapper.AssertAreEqual("battle.player.active_pokemon.ivs.special_defense", 0x22C60B0, [0x0A, 0xB4, 0x93, 0x09], 4);

            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.attack", 0x22C60B5, [0x06], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.defense", 0x22C60B6, [0x06], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.speed", 0x22C60B7, [0x06], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.special_attack", 0x22C60B8, [0x06], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.special_defense", 0x22C60B9, [0x06], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.accuracy", 0x22C60BA, [0x06], 0);
            mapper.AssertAreEqual("battle.player.active_pokemon.modifiers.evasion", 0x22C60BB, [0x06], 0);

            mapper.AssertAreEqual("battle.player.active_pokemon.internals.personality_value", 0x22C6104, [0xAD, 0xCB, 0x7E, 0xE7], 3883846573);

            mapper.AssertAreEqual("battle.player.team.0.species", 0x08, [0x9A, 0x00], "Meganium");
            mapper.AssertAreEqual("battle.player.team.1.species", 0x08, [0xA3, 0x00], "Hoothoot");
            mapper.AssertAreEqual("battle.player.team.2.species", 0x08, [0xAF, 0x00], "Togepi");
            mapper.AssertAreEqual("battle.player.team.3.species", 0x08, [0x00, 0x00], null);
            mapper.AssertAreEqual("battle.player.team.4.species", 0x08, [0x00, 0x00], null);
            mapper.AssertAreEqual("battle.player.team.5.species", 0x08, [0x00, 0x00], null);


            mapper.AssertAreEqual("battle.opponent.team_count", 0x22CB2EC, [0x01], 1);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.species", 0x22C615C, [0x29, 0x00], "Zubat");
            mapper.AssertAreEqual("battle.opponent.active_pokemon.level", 0x22C6190, [0x06], 6);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.ability", 0x22C6183, [0x27], "Inner Focus");
            mapper.AssertAreEqual("battle.opponent.active_pokemon.type_1", 0x22C6180, [0x03], "Poison");
            mapper.AssertAreEqual("battle.opponent.active_pokemon.type_2", 0x22C6181, [0x02], "Flying");
            mapper.AssertAreEqual("battle.opponent.active_pokemon.nature", 0x22C61C4, [0x27, 0x0C, 0x9C, 0xA9], "Docile");
            mapper.AssertAreEqual("battle.opponent.active_pokemon.exp", 0x22C61C0, [0xD8, 0x00, 0x00, 0x00], 216);

            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.0.move", 0x22C6168, [0x8D, 0x00], "Leech Life");
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.1.move", 0x22C616A, [0x30, 0x00], "Supersonic");
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.2.move", 0x22C616C, [0x00, 0x00], null);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.3.move", 0x22C616E, [0x00, 0x00], null);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.0.pp", 0x22C6188, [0x0F], 15);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.1.pp", 0x22C6189, [0x14], 20);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.2.pp", 0x22C618A, [0x00], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.moves.3.pp", 0x22C618B, [0x00], 0);

            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.hp", 0x22C61A8, [0x15, 0x00], 21);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.hp_max", 0x22C61AC, [0x15, 0x00], 21);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.attack", 0x22C615E, [0x0B, 0x00], 11);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.defense", 0x22C6160, [0x0A, 0x00], 10);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.speed", 0x22C6162, [0x0C, 0x00], 12);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.special_attack", 0x22C6164, [0x0A, 0x00], 10);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.stats.special_defense", 0x22C6166, [0x0A, 0x00], 10);

            mapper.AssertAreEqual("battle.opponent.active_pokemon.ivs.hp", 0x22C6170, [0x10, 0x46, 0xC7, 0x11], 16);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.ivs.attack", 0x22C6170, [0x10, 0x46, 0xC7, 0x11], 16);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.ivs.defense", 0x22C6170, [0x10, 0x46, 0xC7, 0x11], 17);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.ivs.speed", 0x22C6170, [0x10, 0x46, 0xC7, 0x11], 14);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.ivs.special_attack", 0x22C6170, [0x10, 0x46, 0xC7, 0x11], 28);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.ivs.special_defense", 0x22C6170, [0x10, 0x46, 0xC7, 0x11], 8);

            mapper.AssertAreEqual("battle.opponent.active_pokemon.modifiers.attack", 0x022C6175, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.modifiers.defense", 0x022C6176, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.modifiers.speed", 0x022C6177, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.modifiers.special_attack", 0x022C6178, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.modifiers.special_defense", 0x022C6179, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.modifiers.accuracy", 0x022C617A, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.active_pokemon.modifiers.evasion", 0x022C617B, [0x06], 0);

            mapper.AssertAreEqual("battle.opponent.active_pokemon.internals.personality_value", 0x22C61C4, [0x27, 0x0C, 0x9C, 0xA9], 2845576231);
        }
    }
}
