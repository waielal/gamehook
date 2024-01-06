using GameHook.Domain;
using GameHook.Domain.Interfaces;
using GameHook.WebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenAPI.GameHook;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace GameHook.IntegrationTests
{
    public static class GameHookAssertHelpers
    {
        public static void AssertAreEqual(this MapperModel mapper, string path, int expectedAddress, int[] expectedBytes, object? expectedValue)
        {
            if (mapper == null) { throw new Exception("Mapper cannot be NULL."); }

            var actual = mapper.Properties.SingleOrDefault(x => x.Path == path)
                ?? throw new Exception($"Unable to find property '{path}'.");

            Assert.AreEqual(expectedAddress.ToHexdecimalString(), actual.Address?.ToHexdecimalString());
            Assert.AreEqual(expectedBytes.ToHexdecimalString(", "), actual.Bytes.ToHexdecimalString(", "));
            Assert.AreEqual(expectedValue?.ToString(), actual.Value?.ToString());
        }
        //Properties with only values
        public static void AssertAreEqual(this MapperModel mapper, string path, object? expectedValue)
        {
            if (mapper == null) { throw new Exception("Mapper cannot be NULL."); }

            var actual = mapper.Properties.SingleOrDefault(x => x.Path == path)
                ?? throw new Exception($"Unable to find property '{path}'.");

            Assert.AreEqual(expectedValue?.ToString(), actual.Value?.ToString());
        }
        //DMA properties that have byte values but no address
        public static void AssertAreEqual(this MapperModel mapper, string path, int[] expectedBytes, object? expectedValue)
        {
            if (mapper == null) { throw new Exception("Mapper cannot be NULL."); }

            var actual = mapper.Properties.SingleOrDefault(x => x.Path == path)
                ?? throw new Exception($"Unable to find property '{path}'.");

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
        protected ILogger<BaseTest> Logger { get; }

        public BaseTest()
        {
            var testConfiguration = new ConfigurationBuilder().AddJsonFile("testsettings.json").Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(testConfiguration)
                .CreateLogger();

            Server = new HostBuilder()
                .UseSerilog()
                .ConfigureWebHost(host =>
                {
                    host
                        .UseTestServer()
                        .UseConfiguration(testConfiguration)
                        .UseStartup<Startup>();
                })
                .Start();

            StaticMemoryDriver = Server.Services.GetRequiredService<IStaticMemoryDriver>();
            GameHookClient = new GameHookClient("http://localhost:8085", Server.GetTestClient());
            Logger = Server.Services.GetRequiredService<ILogger<BaseTest>>();
        }

        protected async Task LoadSrm(string srmFilename) => await StaticMemoryDriver.SetMemoryFragment(srmFilename);
        protected async Task LoadMapper(string id)
        {
            await GameHookClient.ChangeMapperAsync(new MapperReplaceModel()
            {
                Id = id,
                Driver = "staticMemory"
            });
        }

        protected async Task Load_GB_PokemonRed(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gb_pokemon_red_blue_{n}.json");
            await LoadMapper($"official_gb_pokemon_red_blue");
        }

        protected async Task Load_GB_PokemonYellow(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gb_pokemon_yellow_{n}.json");
            await LoadMapper($"official_gb_pokemon_yellow");
        }

        protected async Task Load_GBC_PokemonGold(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gbc_pokemon_gold_silver_{n}.json");
            await LoadMapper($"official_gbc_pokemon_gold_silver");
        }

        protected async Task Load_GBC_PokemonCrystal(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gbc_pokemon_crystal_{n}.json");
            await LoadMapper($"official_gbc_pokemon_crystal");
        }

        protected async Task Load_GBA_PokemonRuby(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gba_pokemon_ruby_{n}.json");
            await LoadMapper($"official_gba_pokemon_ruby");
        }

        protected async Task Load_GBA_PokemonEmerald(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gba_pokemon_emerald_{n}.json");
            await LoadMapper($"official_gba_pokemon_emerald");
        }

        protected async Task Load_GBA_PokemonFireRed(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gba_pokemon_firered_{n}.json");
            await LoadMapper($"official_gba_pokemon_firered");
        }

        protected async Task Load_NDS_PokemonPlatinum(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"nds_pokemon_platinum_{n}.json");
            await LoadMapper($"official_nds_pokemon_platinum");
        }

        //Depreacted mappers
        protected async Task Load_GB_PokemonRedDeprecated(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gb_pokemon_red_blue_{n}.json");
            await LoadMapper($"official_deprecated_pokemon_red_blue_deprecated");
        }
        protected async Task Load_GB_PokemonYellowDeprecated(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gb_pokemon_yellow_{n}.json");
            await LoadMapper($"official_deprecated_pokemon_yellow_deprecated");
        }
        protected async Task Load_GB_PokemonGoldDeprecated(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gb_pokemon_gold_{n}.json");
            await LoadMapper($"official_deprecated_pokemon_gold_silver_deprecated");
        }
        protected async Task Load_GB_PokemonRubyDeprecated(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gb_pokemon_yellow_{n}.json");
            await LoadMapper($"official_deprecated_pokemon_ruby_deprecated");
        }
        protected async Task Load_GB_PokemonEmeraldDeprecated(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gb_pokemon_yellow_{n}.json");
            await LoadMapper($"official_deprecated_pokemon_emerald_deprecated");
        }
        protected async Task Load_GB_PokemonFireRedDeprecated(int n = 0)
        {
            await StaticMemoryDriver.SetMemoryFragment($"gb_pokemon_yellow_{n}.json");
            await LoadMapper($"official_deprecated_pokemon_firered_deprecated");
        }
    }
}
