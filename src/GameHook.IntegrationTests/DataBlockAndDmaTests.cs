using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameHook.IntegrationTests
{
    [TestClass]
    public class DataBlockAndDmaTests : BaseTest
    {
        [TestMethod]
        public async Task Preprocessor_OK_dmaPointers()
        {
            await Load_GBA_PokemonEmerald(1);

            var dma1_1 = await GameHookClient.GetPropertyAsync("dmaPointers.dma1");
            var dma2_1 = await GameHookClient.GetPropertyAsync("dmaPointers.dma2");
            var dma3_1 = await GameHookClient.GetPropertyAsync("dmaPointers.dma3");
            var gameTimeSeconds_1 = await GameHookClient.GetPropertyAsync("gameTime.seconds");
            var gameTimeFrames_1 = await GameHookClient.GetPropertyAsync("gameTime.frames");

            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x03005D8C,
                Value = 33708668
            }, dma1_1);
            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x03005D90,
                Value = 33704656
            }, dma2_1);
            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x03005D94,
                Value = 33724548
            }, dma3_1);

            // Check game time hours and minutes.
            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x02024AE1,
                Bytes = new int[] { 0x00 },
                Path = "gameTime.seconds",
                Length = 1,
                Type = "int",
                Value = 0
            }, gameTimeSeconds_1);

            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x02024AE2,
                Bytes = new int[] { 0x0C },
                Path = "gameTime.frames",
                Length = 1,
                Type = "int",
                Value = 12
            }, gameTimeFrames_1);

            await Load_GBA_PokemonEmerald(2);

            var dma1_2 = await GameHookClient.GetPropertyAsync("dmaPointers.dma1");
            var dma2_2 = await GameHookClient.GetPropertyAsync("dmaPointers.dma2");
            var dma3_2 = await GameHookClient.GetPropertyAsync("dmaPointers.dma3");
            var gameTimeSeconds_2 = await GameHookClient.GetPropertyAsync("gameTime.seconds");
            var gameTimeFrames_2 = await GameHookClient.GetPropertyAsync("gameTime.frames");

            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x2025A68,
                Value = 33708648
            }, dma1_2);
            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x2024ABC,
                Value = 33704636
            }, dma2_2);
            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x2029870,
                Value = 33724528
            }, dma3_2);

            /*
            // Check game time hours and minutes.
            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x0202460F,
                Bytes = new int[] { 0x00 },
                Path = "gameTime.seconds",
                Size = 1,
                Type = "int",
                Value = 0
            }, gameTimeSeconds_1);

            Assert.ArePropertiesEqual(new OpenAPI.GameHook.PropertyModel
            {
                Address = 0x02024610,
                Bytes = new int[] { 0x0C },
                Path = "gameTime.frames",
                Size = 1,
                Type = "int",
                Value = 12
            }, gameTimeFrames_1);
            */
        }
    }
}