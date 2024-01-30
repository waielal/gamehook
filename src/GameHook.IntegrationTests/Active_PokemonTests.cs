using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace GameHook.IntegrationTests
{
    [TestClass]
    public class Active_Pokemon : BaseTest
    {
        [TestMethod]
        public async Task Yellow_Active_Pokemon()
        {
            await Load_GB_PokemonYellow(9);

            var mapper = await GameHookClient.GetMapperAsync();

            mapper.AssertAreEqual("player.active_pokemon.species", 0xD16A, [84], "Pikachu");
            mapper.AssertAreEqual("player.active_pokemon.dex_number", 0xD16A, [84], 25);
            //mapper.AssertAreEqual("player.active_pokemon.nickname", 0xD2B4, [129], "BBB");
            mapper.AssertAreEqual("player.active_pokemon.level", 0xD18B, [20], 20);
            //mapper.AssertAreEqual("player.active_pokemon.exp", 0xD178, [0], 8303);
            mapper.AssertAreEqual("player.active_pokemon.type_1", 0xD16F, [23], "Electric");
            mapper.AssertAreEqual("player.active_pokemon.type_2", 0xD170, [23], "Electric");
            //mapper.AssertAreEqual("player.active_pokemon.ot_id", 0xD176, [139], 35662);
            mapper.AssertAreEqual("player.active_pokemon.catch_rate", 0xD171, [163], 163);
            mapper.AssertAreEqual("player.active_pokemon.status_condition", 0xD16E, [0], null);
            mapper.AssertAreEqual("player.active_pokemon.moves.0.move", 0xD172, [84], "ThunderShock");
            mapper.AssertAreEqual("player.active_pokemon.moves.0.pp", 0xD187, [23], 23);
            mapper.AssertAreEqual("player.active_pokemon.moves.0.pp_up", 0xD187, [23], 0);
            mapper.AssertAreEqual("player.active_pokemon.moves.1.move", 0xD173, [104], "Double Team");
            mapper.AssertAreEqual("player.active_pokemon.moves.1.pp", 0xD188, [15], 15);
            mapper.AssertAreEqual("player.active_pokemon.moves.1.pp_up", 0xD188, [15], 0);
            mapper.AssertAreEqual("player.active_pokemon.moves.2.move", 0xD174, [21], "Slam");
            mapper.AssertAreEqual("player.active_pokemon.moves.2.pp", 0xD189, [20], 20);
            mapper.AssertAreEqual("player.active_pokemon.moves.2.pp_up", 0xD189, [20], 0);
            mapper.AssertAreEqual("player.active_pokemon.moves.3.move", 0xD175, [98], "Quick Attack");
            mapper.AssertAreEqual("player.active_pokemon.moves.3.pp", 0xD18A, [30], 30);
            mapper.AssertAreEqual("player.active_pokemon.moves.3.pp_up", 0xD18A, [30], 0);
            // mapper.AssertAreEqual("player.active_pokemon.stats.hp", 0xD16B, [0], 48);
            // mapper.AssertAreEqual("player.active_pokemon.stats.hp_max", 0xD18C, [0], 51);
            // mapper.AssertAreEqual("player.active_pokemon.stats.attack", 0xD18E, [0], 34);
            // mapper.AssertAreEqual("player.active_pokemon.stats.defense", 0xD190, [0], 25);
            // mapper.AssertAreEqual("player.active_pokemon.stats.speed", 0xD192, [0], 50);
            mapper.AssertAreEqual("player.active_pokemon.stats.special", 0xD194, [0], 28);
            mapper.AssertAreEqual("player.active_pokemon.ivs.hp", 10);
            mapper.AssertAreEqual("player.active_pokemon.ivs.attack", 0xD185, [190], 11);
            mapper.AssertAreEqual("player.active_pokemon.ivs.defense", 0xD185, [190], 14);
            mapper.AssertAreEqual("player.active_pokemon.ivs.speed", 0xD186, [242], 15);
            mapper.AssertAreEqual("player.active_pokemon.ivs.special", 0xD186, [242], 2);
            mapper.AssertAreEqual("player.active_pokemon.evs.hp", 0xD17B, [16], 4171);
            mapper.AssertAreEqual("player.active_pokemon.evs.attack", 0xD17D, [13], 3568);
            mapper.AssertAreEqual("player.active_pokemon.evs.defense", 0xD17F, [16], 4278);
            mapper.AssertAreEqual("player.active_pokemon.evs.speed", 0xD181, [17], 4483);
            mapper.AssertAreEqual("player.active_pokemon.evs.special", 0xD183, [10], 2815);
            mapper.AssertAreEqual("player.active_pokemon.modifiers.attack", 0);
            mapper.AssertAreEqual("player.active_pokemon.modifiers.defense", 0);
            mapper.AssertAreEqual("player.active_pokemon.modifiers.speed", 0);
            mapper.AssertAreEqual("player.active_pokemon.modifiers.special", 0);
            mapper.AssertAreEqual("player.active_pokemon.modifiers.accuracy", 0);
            mapper.AssertAreEqual("player.active_pokemon.modifiers.evasion", 0);
            mapper.AssertAreEqual("player.active_pokemon.volatile_status_conditions.confusion", false);
            mapper.AssertAreEqual("player.active_pokemon.volatile_status_conditions.toxic", false);
            mapper.AssertAreEqual("player.active_pokemon.volatile_status_conditions.leech_seed", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.bide", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.thrash", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.multi_hit", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.flinch", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.charging", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.multi_turn", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.invulnerable", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.bypass_accuracy", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.mist", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.focus_energy", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.substitute", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.recharge", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.rage", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.lightscreen", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.reflect", false);
            mapper.AssertAreEqual("player.active_pokemon.effects.transformed", false);
            mapper.AssertAreEqual("player.active_pokemon.counters.multi_hit", 0);
            mapper.AssertAreEqual("player.active_pokemon.counters.confusion", 0);
            mapper.AssertAreEqual("player.active_pokemon.counters.toxic", 0);
            mapper.AssertAreEqual("player.active_pokemon.counters.disable", 0);
            mapper.AssertAreEqual("player.active_pokemon.last_move.move", null);
            mapper.AssertAreEqual("player.active_pokemon.last_move.effect", 0);
            mapper.AssertAreEqual("player.active_pokemon.last_move.power", 0);
            mapper.AssertAreEqual("player.active_pokemon.last_move.type", null);
            mapper.AssertAreEqual("player.active_pokemon.last_move.accuracy", 0);
            mapper.AssertAreEqual("player.active_pokemon.last_move.pp_max", 0);
        }
    }
}
