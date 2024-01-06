using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using System.Threading.Tasks;

namespace GameHook.IntegrationTests
{
    [TestClass]
    public class PokemonPlatinum : BaseTest
    {
        [TestMethod]
        public async Task Property_OK_Names()
        {
            await Load_NDS_PokemonPlatinum();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.name", 36168212, [0x2B, 0x01, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00], "A");
            mapper.AssertAreEqual("player.team.0.nickname", 72, [0x3E, 0x01, 0x39, 0x01, 0x3C, 0x01, 0x3E, 0x01, 0x2F, 0x01, 0x3C, 0x01, 0x3C, 0x01, 0x2B, 0x01, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00], "TORTERRA");
        }

        [TestMethod]
        public async Task Property_OK_Player()
        {
            await Load_NDS_PokemonPlatinum();
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.badges.badge1", 0x0227E22E, [0x03], true);
        }

        [TestMethod]
        public async Task Property_OK_PokemonPartyStructure_Index0()
        {
            await Load_NDS_PokemonPlatinum();

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("meta.global_pointer", 0x02101D2C, [0xB8, 0x11, 0x27, 0x02], 36114872);

            mapper.AssertAreEqual("player.team.0.species", 0x08, [0x85, 0x01], "Torterra");
            mapper.AssertAreEqual("player.team.0.nature", 0x00, [0xB4, 0x00, 0xE8, 0xB5], "Adamant");
            mapper.AssertAreEqual("player.team.0.level", 0x8C, [0x25], 37);
            //mapper.AssertAreEqual("player.team.0.moves.0.move", 0x8C, [0x48], "Mega Drain");
            //mapper.AssertAreEqual("player.team.0.evs.hp", 0x18, [0x0E], 14);
            mapper.AssertAreEqual("player.team.0.evs.specialDefense", 0x1D, [0x06], 6);
            mapper.AssertAreEqual("player.team.0.flags.isEgg", 0x38, [0xC4, 0x11, 0xAE, 0x23], false);
            mapper.AssertAreEqual("player.team.0.internals.personalityValue", 0x00, [0xB4, 0x00, 0xE8, 0xB5], 3051880628);
            mapper.AssertAreEqual("player.team.0.internals.checksum", 0x06, [0xE1, 0xAC], 44257);
            mapper.AssertAreEqual("player.team.0.expPoints", 0x10, [0x97, 0xB1, 0x00, 0x00], 45463);
            mapper.AssertAreEqual("player.team.0.heldItem", 0x0A, [0xF9, 0x00], "Charcoal");

            // Check IVs which make use of the BitRange function.
            mapper.AssertAreEqual("player.team.0.ivs.hp", 0x38, [0xC4, 0x11, 0xAE, 0x23], 4);
            //mapper.AssertAreEqual("player.team.0.ivs.attack", 0x38, [0xC4, 0x11, 0xAE, 0x23], 4);
            mapper.AssertAreEqual("player.team.0.ivs.defense", 0x38, [0xC4, 0x11, 0xAE, 0x23], 4);
            mapper.AssertAreEqual("player.team.0.ivs.hp", 0x38, [0xC4, 0x11, 0xAE, 0x23], 4);
            mapper.AssertAreEqual("player.team.0.flags.isEgg", 0x38, [0xC4, 0x11, 0xAE, 0x23], false);
        }

        [TestMethod]
        public async Task Property_OK_BattleOutcome()
        {
            await Load_NDS_PokemonPlatinum();
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("battle.outcome", 0x022C5B2F, [0x45], 69);
            mapper.AssertAreEqual("battle.player.activePokemon.expPoints", 0x022C57B4, [0x9C, 0x57, 0x2C, 0x02], 36460444); //Data makes no contextual sense because the player is not in battle
            // mapper.AssertAreEqual("battle.ally.team.0.nature", 0x00, new int[] { 0x00, 0x00, 0x00, 0x00 }, "Hardy"); //Data makes no contextual sense because the player is not in battle
            //mapper.AssertAreEqual("battle.opponentA.team.0.pokerus", 0x82, [0x6A], 106); //Data makes no contextual sense because the player is not in battle
            //mapper.AssertAreEqual("battle.opponentB.team.0.pokedexNumber", 0x08, [0x58, 0xF3], 62296); //Data makes no contextual sense because the player is not in battle
        }

        [TestMethod]
        public async Task Property_OK_BagStructure()
        {
            await Load_NDS_PokemonPlatinum();
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("bag.items.0.item", 0x0227E7DC, [0xDF, 0x00], "Amulet Coin");
            mapper.AssertAreEqual("bag.medicine.0.item", 0x0227ECF8, [0x12, 0x00], "Antidote");
            mapper.AssertAreEqual("bag.pokeBalls.0.item", 0x0227EE98, [0x04, 0x00], "Poké Ball");
            mapper.AssertAreEqual("bag.tmhm.0.item", 0x0227EB38, [0x48, 0x01], "TM01");
            mapper.AssertAreEqual("bag.berries.0.item", 0x0227ED98, [0x9C, 0x00], "Persim Berry");
        }

        [TestMethod]
        public async Task Property_OK_PokemonPartyStructure_Index0_IsNotEgg()
        {
            await Load_NDS_PokemonPlatinum();
            var mapper = await GameHookClient.GetMapperAsync();

            // The 4th Pokemon in the party is actually an egg.
            mapper.AssertAreEqual("player.team.0.flags.isEgg", 0x38, [0xC4, 0x11, 0xAE, 0x23], false);
        }

        [TestMethod]
        public async Task Property_OK_PokemonPartyStructure_Index3_IsEgg()
        {
            await Load_NDS_PokemonPlatinum();
            var mapper = await GameHookClient.GetMapperAsync();

            // The 4th Pokemon in the party is actually an egg.
            mapper.AssertAreEqual("player.team.3.flags.isEgg", 0x38, [0x9C, 0xFC, 0xF2, 0x51], true);
        }

        //Tests for Platinum
        [TestMethod]
        public async Task Property_OK_Meta()
        {
            await Load_NDS_PokemonPlatinum(1);
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("meta.generation", 4);
            mapper.AssertAreEqual("meta.gameName", "Platinum");
            mapper.AssertAreEqual("meta.gameType", "Third Version");
            mapper.AssertAreEqual("meta.global_pointer", 0x02101D2C, [0xB8, 0x11, 0x27, 0x02], 36114872);
            mapper.AssertAreEqual("meta.enemy_pointer", 0x022A64AC, [0xCC, 0x7E, 0x2A, 0x02], 36339404);
            mapper.AssertAreEqual("meta.state", "Battle");
            mapper.AssertAreEqual("meta.stateEnemy", "Pokemon In Battle");
        }
        [TestMethod]
        public async Task Property_OK_Time()
        {
            await Load_NDS_PokemonPlatinum(1);
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("gameTime.hours", 0x0227E236, [0x09, 0x00], 9);
            mapper.AssertAreEqual("gameTime.minutes", 0x0227E238, [0x04], 4);
            mapper.AssertAreEqual("gameTime.seconds", 0x0227E239, [0x1A], 26);
        }
        [TestMethod]
        public async Task Property_OK_Rival()
        {
            await Load_NDS_PokemonPlatinum(1);
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("rival.name", 0x02280994, [0x2C, 0x01, 0x45, 0x01, 0x56, 0x01, 0x56, 0x01, 0x5D, 0x01, 0xFF, 0xFF, 0x00, 0x00], "Barry");
        }
        [TestMethod]
        public async Task Property_OK_PlayerCharacter()
        {
            await Load_NDS_PokemonPlatinum(1);
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.name", 0x0227E214, [0x3D, 0x01, 0x2D, 0x01, 0x39, 0x01, 0x3E, 0x01, 0x3E, 0x01, 0xFF, 0xFF, 0x00, 0x00], "SCOTT");
            mapper.AssertAreEqual("player.badges.badge1", 0x0227E22E, [0xFF], true);
            mapper.AssertAreEqual("player.badges.badge2", 0x0227E22E, [0xFF], true);
            mapper.AssertAreEqual("player.badges.badge3", 0x0227E22E, [0xFF], true);
            mapper.AssertAreEqual("player.badges.badge4", 0x0227E22E, [0xFF], true);
            mapper.AssertAreEqual("player.badges.badge5", 0x0227E22E, [0xFF], true);
            mapper.AssertAreEqual("player.badges.badge6", 0x0227E22E, [0xFF], true);
            mapper.AssertAreEqual("player.badges.badge7", 0x0227E22E, [0xFF], true);
            mapper.AssertAreEqual("player.badges.badge8", 0x0227E22E, [0xFF], true);
        }
        [TestMethod]
        public async Task Property_OK_Overworld()
        {
            await Load_NDS_PokemonPlatinum(1);
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("overworld.map_name", 0x0227F42C, [0xB9, 0x00], "Pokemon League - Map 14");
            mapper.AssertAreEqual("overworld.map_index", 0x0227F42C, [0xB9, 0x00], 185);
            mapper.AssertAreEqual("overworld.encounterRate.walking", 0x022A157C, [0x00, 0x00], 0);
            mapper.AssertAreEqual("overworld.encounterRate.surfing", 0x022A1648, [0x00, 0x00], 0);
            mapper.AssertAreEqual("overworld.encounterRate.oldRod", 0x022A16A0, [0x00, 0x00], 0);
            mapper.AssertAreEqual("overworld.encounterRate.goodRod", 0x022A16CC, [0x00, 0x00], 0);
            mapper.AssertAreEqual("overworld.encounterRate.superRod", 0x022A16F8, [0x00, 0x00], 0);
        }
        [TestMethod]
        public async Task Property_OK_Bag()
        {
            await Load_NDS_PokemonPlatinum(1);
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("bag.items.0.item", 0x0227E7DC, [0x5E, 0x00], "Honey");
            mapper.AssertAreEqual("bag.items.0.quantity", 0x0227E7DE, [0x0A, 0x00], 10);
            mapper.AssertAreEqual("bag.medicine.0.item", 0x0227ECF8, [0x12, 0x00], "Antidote");
            mapper.AssertAreEqual("bag.medicine.0.quantity", 0x0227ECFA, [0x02, 0x00], 2);
            mapper.AssertAreEqual("bag.pokeBalls.0.item", 0x0227EE98, [0x0E, 0x00], "Heal Ball");
            mapper.AssertAreEqual("bag.pokeBalls.0.quantity", 0x0227EE9A, [0x01, 0x00], 1);
            mapper.AssertAreEqual("bag.tmhm.0.item", 0x0227EB38, [0x4D, 0x01], "TM06");
            mapper.AssertAreEqual("bag.tmhm.0.quantity", 0x0227EB3A, [0x01, 0x00], 1);
            mapper.AssertAreEqual("bag.berries.0.item", 0x0227ED98, [0x95, 0x00], "Cheri Berry");
            mapper.AssertAreEqual("bag.berries.0.quantity", 0x0227ED9A, [0x01, 0x00], 1);
        }
        [TestMethod]
        public async Task Property_OK_PlayerPartyStructure()
        {
            await Load_NDS_PokemonPlatinum(1);
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.team.0.species", 0x08, [0x85, 0x01], "Torterra");
            mapper.AssertAreEqual("player.team.0.nickname", 0x48, [0x3E, 0x01, 0x41, 0x01, 0x33, 0x01, 0x31, 0x01, 0xFF, 0xFF, 0x62, 0x01, 0x58, 0x43, 0xA5, 0x01, 0xCC, 0x9C, 0x79, 0x01, 0x38, 0xB8], "TWIG");
            mapper.AssertAreEqual("player.team.0.level", 0x8C, [0x54], 84);
            mapper.AssertAreEqual("player.team.0.ability", 0x15, [0x41], "Overgrow");
            mapper.AssertAreEqual("player.team.0.nature", [0xC0, 0x7D, 0xEF, 0x45], "Hardy");
            mapper.AssertAreEqual("player.team.0.heldItem", 0x0A, [0xFD, 0x00], "Shell Bell");
            mapper.AssertAreEqual("player.team.0.friendship", 0x14, [0xFF], 255);
            mapper.AssertAreEqual("player.team.0.expPoints", 0x10, [0x20, 0x5D, 0x09, 0x00], 613664);
            mapper.AssertAreEqual("player.team.0.statusCondition", 0x88, [0x00], null);
            mapper.AssertAreEqual("player.team.0.pokerus", 0x82, [0x00], 0);

            mapper.AssertAreEqual("player.team.0.moves.0.move", 0x28, [0x59, 0x00], "Earthquake");
            mapper.AssertAreEqual("player.team.0.moves.0.pp", 0x30, [0x0A], 10);
            mapper.AssertAreEqual("player.team.0.moves.0.ppUp", 0x34, [0x00], 0);

            mapper.AssertAreEqual("player.team.0.stats.hp", 0x8E, [0x25, 0x01], 293);
            mapper.AssertAreEqual("player.team.0.stats.attack", 0x92, [0xED, 0x00], 237);
            mapper.AssertAreEqual("player.team.0.stats.defense", 0x94, [0xD9, 0x00], 217);
            mapper.AssertAreEqual("player.team.0.stats.speed", 0x96, [0xA2, 0x00], 162);
            mapper.AssertAreEqual("player.team.0.stats.specialAttack", 0x98, [0xAF, 0x00], 175);
            mapper.AssertAreEqual("player.team.0.stats.specialDefense", 0x9A, [0xB1, 0x00], 177);

            mapper.AssertAreEqual("player.team.0.ivs.hp", 0x38, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("player.team.0.ivs.attack", 0x38, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("player.team.0.ivs.defense", 0x38, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("player.team.0.ivs.speed", 0x38, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("player.team.0.ivs.specialAttack", 0x38, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("player.team.0.ivs.specialDefense", 0x38, [0xFF, 0xFF, 0xFF, 0xBF], 31);

            mapper.AssertAreEqual("player.team.0.evs.hp", 0x18, [0x41], 65);
            mapper.AssertAreEqual("player.team.0.evs.attack", 0x19, [0x70], 112);
            mapper.AssertAreEqual("player.team.0.evs.defense", 0x1A, [0x32], 50);
            mapper.AssertAreEqual("player.team.0.evs.speed", 0x1B, [0xB1], 177);
            mapper.AssertAreEqual("player.team.0.evs.specialAttack", 0x1C, [0x58], 88);
            mapper.AssertAreEqual("player.team.0.evs.specialDefense", 0x1D, [0x12], 18);

            mapper.AssertAreEqual("player.team.0.flags.isEgg", 0x38, [0xFF, 0xFF, 0xFF, 0xBF], false);
            mapper.AssertAreEqual("player.team.0.flags.isNicknamed", 0x38, [0xFF, 0xFF, 0xFF, 0xBF], true);
            mapper.AssertAreEqual("player.team.0.flags.skipChecksum1", 0x04, [0x00], false);
            mapper.AssertAreEqual("player.team.0.flags.skipChecksum2", 0x04, [0x00], false);
            mapper.AssertAreEqual("player.team.0.flags.isBadEgg", 0x04, [0x00], false);

            mapper.AssertAreEqual("player.team.0.internals.personalityValue", [0xC0, 0x7D, 0xEF, 0x45], 1173323200);
            mapper.AssertAreEqual("player.team.0.internals.checksum", 0x06, [0xF2, 0x71], 29170);
            mapper.AssertAreEqual("player.team.0.internals.secretID", [0xC0, 0x7D], 32192);
            mapper.AssertAreEqual("player.team.0.internals.language", 0x17, [0x02], "English");

            mapper.AssertAreEqual("player.team.1.species", 0x08, [0x8F, 0x01], "Bidoof");
            mapper.AssertAreEqual("player.team.2.species", 0x08, [0x8C, 0x01], "Starly");
            mapper.AssertAreEqual("player.team.3.species", 0x08, [0x36, 0x00], "Psyduck");
            mapper.AssertAreEqual("player.team.4.species", 0x08, [0x4A, 0x00], "Geodude");
        }
        [TestMethod]
        public async Task Property_OK_BattleStructure()
        {
            await Load_NDS_PokemonPlatinum(1);
            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("battle.outcome", 0x022C5B2F, [0x00], 0);
            mapper.AssertAreEqual("battle.mode", "Trainer");
            mapper.AssertAreEqual("battle.player.teamCount", 0x0227E248, [0x06], 6);

            mapper.AssertAreEqual("battle.player.activePokemon.species", 0x022C5750, [0x85, 0x01], "Torterra");
            mapper.AssertAreEqual("battle.player.activePokemon.nickname", 0x022C5786, [0x3E, 0x01, 0x41, 0x01, 0x33, 0x01, 0x31, 0x01, 0xFF, 0xFF, 0x62, 0x01, 0x58, 0x43, 0xA5, 0x01, 0xCC, 0x9C, 0x79, 0x01, 0xFF, 0xFF], "TWIG");
            mapper.AssertAreEqual("battle.player.activePokemon.level", 0x022C5784, [0x54], 84);
            mapper.AssertAreEqual("battle.player.activePokemon.ability", 0x022C5777, [0x41], "Overgrow");
            mapper.AssertAreEqual("battle.player.activePokemon.nature", 0x022C57B8, [0xC0, 0x7D, 0xEF, 0x45], "Hardy");
            mapper.AssertAreEqual("battle.player.activePokemon.heldItem", 0x022C57C8, [0xFD, 0x00], "Shell Bell");
            mapper.AssertAreEqual("battle.player.activePokemon.type_1", 0x022C5774, [0x0C], "Grass");
            mapper.AssertAreEqual("battle.player.activePokemon.type_2", 0x022C5775, [0x04], "Ground");
            mapper.AssertAreEqual("battle.player.activePokemon.statusCondition", 0x022C57BC, [0x00], null);

            mapper.AssertAreEqual("battle.player.activePokemon.moves.0.move", 0x022C575C, [0x59, 0x00], "Earthquake");
            mapper.AssertAreEqual("battle.player.activePokemon.moves.0.pp", 0x022C577C, [0x0A], 10);

            mapper.AssertAreEqual("battle.player.activePokemon.stats.hp", 0x022C579C, [0xF8, 0x00], 248);
            mapper.AssertAreEqual("battle.player.activePokemon.stats.attack", 0x022C5752, [0xED, 0x00], 237);
            mapper.AssertAreEqual("battle.player.activePokemon.stats.defense", 0x022C5754, [0xD9, 0x00], 217);
            mapper.AssertAreEqual("battle.player.activePokemon.stats.speed", 0x022C5756, [0xA2, 0x00], 162);
            mapper.AssertAreEqual("battle.player.activePokemon.stats.specialAttack", 0x022C5758, [0xAF, 0x00], 175);
            mapper.AssertAreEqual("battle.player.activePokemon.stats.specialDefense", 0x022C575A, [0xB1, 0x00], 177);

            mapper.AssertAreEqual("battle.player.activePokemon.ivs.hp", 0x022C5764, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("battle.player.activePokemon.ivs.attack", 0x022C5764, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("battle.player.activePokemon.ivs.defense", 0x022C5764, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("battle.player.activePokemon.ivs.speed", 0x022C5764, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("battle.player.activePokemon.ivs.specialAttack", 0x022C5764, [0xFF, 0xFF, 0xFF, 0xBF], 31);
            mapper.AssertAreEqual("battle.player.activePokemon.ivs.specialDefense", 0x022C5764, [0xFF, 0xFF, 0xFF, 0xBF], 31);

            mapper.AssertAreEqual("battle.player.activePokemon.modifiers.attack", 0x022C5769, [0x08], 2);
            mapper.AssertAreEqual("battle.player.activePokemon.modifiers.defense", 0x022C576A, [0x06], 0);
            mapper.AssertAreEqual("battle.player.activePokemon.modifiers.speed", 0x022C576B, [0x06], 0);
            mapper.AssertAreEqual("battle.player.activePokemon.modifiers.specialAttack", 0x022C576C, [0x06], 0);
            mapper.AssertAreEqual("battle.player.activePokemon.modifiers.specialDefense", 0x022C576D, [0x06], 0);
            mapper.AssertAreEqual("battle.player.activePokemon.modifiers.accuracy", 0x022C576E, [0x06], 0);
            mapper.AssertAreEqual("battle.player.activePokemon.modifiers.evasion", 0x022C576F, [0x06], 0);

            mapper.AssertAreEqual("battle.player.activePokemon.internals.personalityValue", 0x022C57B8, [0xC0, 0x7D, 0xEF, 0x45], 1173323200);

            mapper.AssertAreEqual("battle.player.team.1.species", 0x08, [0x8F, 0x01], "Bidoof");
            mapper.AssertAreEqual("battle.player.team.2.species", 0x08, [0x8C, 0x01], "Starly");
            mapper.AssertAreEqual("battle.player.team.3.species", 0x08, [0x36, 0x00], "Psyduck");
            mapper.AssertAreEqual("battle.player.team.4.species", 0x08, [0x4A, 0x00], "Geodude");

            mapper.AssertAreEqual("battle.opponent.trainer", 0x022BFA36, [0x0B, 0x01], "Champion Cynthia");
            mapper.AssertAreEqual("battle.opponent.partyPos", 0x022C5B42, [0x00], 0);
            mapper.AssertAreEqual("battle.opponent.teamCount", 0x022C9FF0, [0x06], 6);
            mapper.AssertAreEqual("battle.opponent.enemy_bar_synced_hp", 0x022C585C, [0x8F, 0x00], 143);
            mapper.AssertAreEqual("battle.opponent.activePokemon.species", 0x022C5810, [0xBA, 0x01], "Spiritomb");
            mapper.AssertAreEqual("battle.opponent.activePokemon.level", 0x022C5844, [0x3A], 58);
            mapper.AssertAreEqual("battle.opponent.activePokemon.ability", 0x022C5837, [0x2E], "Pressure");
            mapper.AssertAreEqual("battle.opponent.activePokemon.stats.hp", 0x022C585C, [0x8F, 0x00], 143);

            mapper.AssertAreEqual("battle.opponent.activePokemon.ivs.hp", 0x022C5824, [0xDE, 0x7B, 0xEF, 0x3D], 30);
            mapper.AssertAreEqual("battle.opponent.activePokemon.ivs.attack", 0x022C5824, [0xDE, 0x7B, 0xEF, 0x3D], 30);
            mapper.AssertAreEqual("battle.opponent.activePokemon.ivs.defense", 0x022C5824, [0xDE, 0x7B, 0xEF, 0x3D], 30);
            mapper.AssertAreEqual("battle.opponent.activePokemon.ivs.speed", 0x022C5824, [0xDE, 0x7B, 0xEF, 0x3D], 30);
            mapper.AssertAreEqual("battle.opponent.activePokemon.ivs.specialAttack", 0x022C5824, [0xDE, 0x7B, 0xEF, 0x3D], 30);
            mapper.AssertAreEqual("battle.opponent.activePokemon.ivs.specialDefense", 0x022C5824, [0xDE, 0x7B, 0xEF, 0x3D], 30);

            
            mapper.AssertAreEqual("battle.opponent.activePokemon.modifiers.attack", 0x022C5829, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.activePokemon.modifiers.defense", 0x022C582A, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.activePokemon.modifiers.speed", 0x022C582B, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.activePokemon.modifiers.specialAttack", 0x022C582C, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.activePokemon.modifiers.specialDefense", 0x022C582D, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.activePokemon.modifiers.accuracy", 0x022C582E, [0x06], 0);
            mapper.AssertAreEqual("battle.opponent.activePokemon.modifiers.evasion", 0x022C582F, [0x06], 0);

            mapper.AssertAreEqual("battle.opponent.activePokemon.moves.0.move", 0x022C581C, [0x8F, 0x01], "Dark Pulse");
            mapper.AssertAreEqual("battle.opponent.activePokemon.moves.1.move", 0x022C581E, [0x5E, 0x00], "Psychic");
            mapper.AssertAreEqual("battle.opponent.activePokemon.moves.2.move", 0x022C5820, [0x3E, 0x01], "Silver Wind");
            mapper.AssertAreEqual("battle.opponent.activePokemon.moves.3.move", 0x022C5822, [0xF7, 0x00], "Shadow Ball");
            mapper.AssertAreEqual("battle.opponent.activePokemon.moves.0.pp", 0x022C583C, [0x0E], 14);
            mapper.AssertAreEqual("battle.opponent.activePokemon.moves.1.pp", 0x022C583D, [0x0A], 10);
            mapper.AssertAreEqual("battle.opponent.activePokemon.moves.2.pp", 0x022C583E, [0x05], 5);
            mapper.AssertAreEqual("battle.opponent.activePokemon.moves.3.pp", 0x022C583F, [0x0F], 15);

            mapper.AssertAreEqual("battle.opponent.activePokemon.internals.personalityValue", 0x022C5878, [0x78, 0x05, 0x6E, 0x00], 7210360);

            mapper.AssertAreEqual("battle.field.weather", 0x022C2B91, [0x00, 0x00], null);
            mapper.AssertAreEqual("battle.field.weatherCounter", 0x022C2B94, [0x00], 0);
        }
    }
}
