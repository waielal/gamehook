using GameHook.Domain;
using GameHook.Domain.Interfaces;
using GameHook.WebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenAPI.GameHook;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameHook.IntegrationTests
{
    public static class GameHookAssertHelpers
    {
        public static void AssertAreEqual(this MapperModel mapper, string path, int expectedAddress, int[] expectedBytes, object? expectedValue)
        {
            if (mapper == null) { throw new Exception("Mapper cannot be NULL."); }

            var actual = mapper.Properties.SingleOrDefault(x => x.Path == path)
                ?? throw new Exception($"Unable to find property '{path}'.");

            Assert.AreEqual(expectedAddress, actual.Address);
            Assert.AreEqual(expectedBytes.ToHexdecimalString(", "), actual.Bytes.ToHexdecimalString(", "));
            Assert.AreEqual(expectedValue?.ToString(), actual.Value?.ToString());
        }
    }

    public static class GameHookAssert
    {
        public static void ArePropertiesEqual(PropertyModel expected, PropertyModel actual)
        {
            Assert.AreEqual(expected.Address, actual.Address);
            Assert.AreEqual(expected.Bytes.ToHexdecimalString(", "), actual.Bytes.ToHexdecimalString(", "));
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Frozen, actual.Frozen);
            Assert.AreEqual(expected.Path, actual.Path);
            Assert.AreEqual(expected.Position, actual.Position);
            Assert.AreEqual(expected.Reference, actual.Reference);
            Assert.AreEqual(expected.Length, actual.Length);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Value, actual.Value);
        }
    }

    public abstract class BaseTest
    {
        private IHost Server { get; }
        protected IStaticMemoryDriver StaticMemoryDriver { get; }
        protected GameHookClient GameHookClient { get; }

        public BaseTest()
        {
            var testConfiguration = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            Server = new HostBuilder()
                .ConfigureWebHost(host =>
                {
                    host
                        .UseTestServer()
                        .UseSerilog((ctx, conf) =>
                        {
                            conf.ReadFrom.Configuration(ctx.Configuration);
                        })
                        .UseConfiguration(testConfiguration)
                        .UseStartup<Startup>();
                })
                .Start();

            StaticMemoryDriver = Server.Services.GetRequiredService<IStaticMemoryDriver>();
            GameHookClient = new GameHookClient("http://localhost:8085", Server.GetTestClient());
        }

        protected async Task LoadMapperAndRamState(string name, string extension, int n)
        {
            await StaticMemoryDriver.SetMemoryFragment($"{name}_{n}.json".ToLower());

            await GameHookClient.ChangeMapperAsync(new MapperReplaceModel()
            {
                Id = $"official_{name}_{extension}".ToLower(),
                Driver = "staticMemory"
            });
        }

        protected async Task Load_GBC_PokemonCrystal(string mapperFileExtension = "xml", int n = 0) => await LoadMapperAndRamState("gb_pokemon_crystal", mapperFileExtension, n);
        protected async Task Load_GBA_PokemonEmerald(string mapperFileExtension = "xml", int n = 0) => await LoadMapperAndRamState("gba_pokemon_emerald", mapperFileExtension, n);
        protected async Task Load_NDS_PokemonPlatinum(string mapperFileExtension = "xml", int n = 0) => await LoadMapperAndRamState("nds_pokemon_platinum", mapperFileExtension, n);
    }
}
