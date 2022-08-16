using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace GameHook.IntegrationTests
{
    [TestClass]
    public class PropertyTests : BaseTest
    {
        [TestMethod]
        public async Task Property_OK_BinaryCodedDecimal()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();
            var bcdValue_1 = mapper.Properties.Single(x => x.Path == "player.money");
            var bcdValue_2 = (await GameHookClient.GetPropertiesAsync()).Single(x => x.Path == "player.money");
            var bcdValue_3 = await GameHookClient.GetPropertyAsync("player.money");

            Assert.AreEqual(bcdValue_1.Address, 0xD346);
            Assert.AreBytesEqual(bcdValue_1.Bytes, new List<int>() { 0x03, 0x42, 0x07 });
            Assert.IsNull(bcdValue_1.Description);
            Assert.AreEqual(1, bcdValue_1.Position);
            Assert.AreEqual(bcdValue_1.Path, "player.money");
            Assert.IsNull(bcdValue_1.Reference);
            Assert.AreEqual(bcdValue_1.Size, 3);
            Assert.AreEqual(bcdValue_1.Type, "binaryCodedDecimal");
            Assert.AreValuesEqual(34207, bcdValue_1.Value);

            Assert.ArePropertiesEqual(bcdValue_1, bcdValue_2);
            Assert.ArePropertiesEqual(bcdValue_1, bcdValue_3);
        }

        [TestMethod]
        public async Task Property_OK_BitFieldProperty()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();
            var bcdValue_1 = mapper.Properties.Single(x => x.Path == "player.pokedexSeen");
            var bcdValue_2 = (await GameHookClient.GetPropertiesAsync()).Single(x => x.Path == "player.pokedexSeen");
            var bcdValue_3 = await GameHookClient.GetPropertyAsync("player.pokedexSeen");

            Assert.AreEqual(bcdValue_1.Address, 0xD309);
            Assert.AreBytesEqual(bcdValue_1.Bytes, new List<int>() { 0x4B, 0xB6, 0xFC, 0xDE, 0x33, 0xAF, 0x9F, 0xC6, 0x3F, 0xF6, 0xC8, 0xFE, 0xA9, 0xF7, 0xA2, 0x89, 0x56, 0x61, 0x18 });
            Assert.IsNull(bcdValue_1.Description);
            Assert.AreEqual(1, bcdValue_1.Position);
            Assert.AreEqual(bcdValue_1.Path, "player.pokedexSeen");
            Assert.IsNull(bcdValue_1.Reference);
            Assert.AreEqual(bcdValue_1.Size, 19);
            Assert.AreEqual(bcdValue_1.Type, "bitArray");
            Assert.AreValueArraysEqual(new List<bool>() { true, true, false, true, false, false, true, false, false, true, true, false, true, true, false, true, false, false, true, true, true, true, true, true, false, true, true, true, true, false, true, true, true, true, false, false, true, true, false, false, true, true, true, true, false, true, false, true, true, true, true, true, true, false, false, true, false, true, true, false, false, false, true, true, true, true, true, true, true, true, false, false, false, true, true, false, true, true, true, true, false, false, false, true, false, false, true, true, false, true, true, true, true, true, true, true, true, false, false, true, false, true, false, true, true, true, true, false, true, true, true, true, false, true, false, false, false, true, false, true, true, false, false, true, false, false, false, true, false, true, true, false, true, false, true, false, true, false, false, false, false, true, true, false, false, false, false, true, true, false, false, false }, (JArray) bcdValue_1.Value);
            Assert.ArePropertiesEqual(bcdValue_1, bcdValue_2);
            Assert.ArePropertiesEqual(bcdValue_1, bcdValue_3);
        }

        [TestMethod]
        public async Task Property_OK_BitProperty()
        {
            await Load_GB_PokemonYellow();

            var mapper = await GameHookClient.GetMapperAsync();
            var bitValue_1 = mapper.Properties.Single(x => x.Path == "settings.battleStyle");
            var bitValue_2 = (await GameHookClient.GetPropertiesAsync()).Single(x => x.Path == "settings.battleStyle");
            var bitValue_3 = await GameHookClient.GetPropertyAsync("settings.battleStyle");

            Assert.AreEqual(bitValue_1.Address, 0xD354);
            Assert.AreBytesEqual(bitValue_1.Bytes, new List<int>() { 0x41 });
            Assert.IsNull(bitValue_1.Description);
            Assert.AreEqual(6, bitValue_1.Position);
            Assert.AreEqual(bitValue_1.Path, "settings.battleStyle");
            Assert.IsNull(bitValue_1.Reference);
            Assert.AreEqual(bitValue_1.Size, 1);
            Assert.AreEqual(bitValue_1.Type, "bit");
            Assert.AreValuesEqual(true, bitValue_1.Value);
            Assert.ArePropertiesEqual(bitValue_1, bitValue_2);
            Assert.ArePropertiesEqual(bitValue_1, bitValue_3);
        }

        [TestMethod]
        public void Property_OK_BooleanProperty()
        {

        }

        [TestMethod]
        public void Property_OK_Integer()
        {

        }

        [TestMethod]
        public void Property_OK_Reference()
        {

        }

        [TestMethod]
        public void Property_OK_String()
        {

        }

        [TestMethod]
        public void Property_OK_UnsignedInteger()
        {

        }

        [TestMethod]
        public async Task Preprocessor_OK_dma_967d10cc()
        {
            await Load_GBA_PokemonFireRed();

            var gameTimeHours = await GameHookClient.GetPropertyAsync("gameTimeHours");
            var gameTimeMinutes = await GameHookClient.GetPropertyAsync("gameTimeMinutes");

            Assert.AreEqual(gameTimeHours.Address, 0x0202460F);
            Assert.AreBytesEqual(gameTimeHours.Bytes, new List<int>() { 0x00 });
            Assert.IsNull(gameTimeHours.Description);
            Assert.IsNull(gameTimeHours.Position);
            Assert.AreEqual(gameTimeHours.Path, "gameTimeHours");
            Assert.IsNull(gameTimeHours.Reference);
            Assert.AreEqual(gameTimeHours.Size, 1);
            Assert.AreEqual(gameTimeHours.Type, "int");
            Assert.AreValuesEqual(0, gameTimeHours.Value);

            Assert.AreEqual(gameTimeMinutes.Address, 0x02024610);
            Assert.AreBytesEqual(gameTimeMinutes.Bytes, new List<int>() { 0x0C });
            Assert.IsNull(gameTimeMinutes.Description);
            Assert.IsNull(gameTimeMinutes.Position);
            Assert.AreEqual(gameTimeMinutes.Path, "gameTimeMinutes");
            Assert.IsNull(gameTimeMinutes.Reference);
            Assert.AreEqual(gameTimeMinutes.Size, 1);
            Assert.AreEqual(gameTimeMinutes.Type, "int");
            Assert.AreValuesEqual(12, gameTimeMinutes.Value);
        }
    }
}