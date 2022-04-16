using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            Assert.IsNull(bcdValue_1.Bit);
            Assert.AreBytesEqual(bcdValue_1.Bytes, new List<int>() { 0x03, 0x42, 0x07 });
            Assert.IsNull(bcdValue_1.Description);
            Assert.AreEqual(1, bcdValue_1.Index);
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
            Assert.IsNull(bcdValue_1.Bit);
            Assert.AreBytesEqual(bcdValue_1.Bytes, new List<int>() { 0x4B, 0xB6, 0xFC, 0xDE, 0x33, 0xAF, 0x9F, 0xC6, 0x3F, 0xF6, 0xC8, 0xFE, 0xA9, 0xF7, 0xA2, 0x89, 0x56, 0x61, 0x18 });
            Assert.IsNull(bcdValue_1.Description);
            Assert.AreEqual(1, bcdValue_1.Index);
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
    }
}