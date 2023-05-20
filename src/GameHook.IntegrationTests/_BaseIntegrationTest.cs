using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameHook.Domain.Interfaces;
using GameHook.IntegrationTests.Fakes;
using GameHook.WebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using OpenAPI.GameHook;
using Serilog;
using A = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GameHook.IntegrationTests
{
    public static class ValueConverters
    {
        public static IEnumerable<T> ValueToTypeArray<T>(this PropertyModel model)
        {
            return ((JArray)model.Value).ToObject<List<T>>() ??
                throw new Exception($"Could not cast {model.Path}'s value to {typeof(T)}");
        }
    }

    public static class Assert
    {
        public static void AreValuesEqual(long expected, object actual)
        {
            A.AreEqual(expected, (long)actual);
        }

        public static void AreValuesEqual(bool expected, object actual)
        {
            A.AreEqual(expected, (bool)actual);
        }

        public static void AreValueArraysEqual(List<bool> expected, JArray actual)
        {
            var actualConverted = actual.ToObject<List<bool>>() ?? throw new Exception("Unable to convert JArray.");

            A.IsTrue(expected.SequenceEqual(actualConverted));
        }

        public static void ArePropertiesEqual(PropertyModel expected, PropertyModel actual)
        {
            A.AreEqual(expected.Address, actual.Address);
            AreBytesEqual(expected.Bytes, actual.Bytes);
            A.AreEqual(expected.Description, actual.Description);
            A.AreEqual(expected.Frozen, actual.Frozen);
            A.AreEqual(expected.Path, actual.Path);
            A.AreEqual(expected.Position, actual.Position);
            A.AreEqual(expected.Reference, actual.Reference);
            A.AreEqual(expected.Length, actual.Length);
            A.AreEqual(expected.Type, actual.Type);
            A.AreEqual(expected.Value, actual.Value);
        }

        public static void AreBytesEqual(ICollection<int> expected, ICollection<int> actual)
        {
            A.IsTrue(expected.SequenceEqual(actual));
        }

        public static void AreEqual(object expected, object? actual)
        {
            A.AreEqual(expected, actual);
        }

        public static void IsTrue(bool o)
        {
            A.IsTrue(o);
        }

        public static void IsFalse(bool o)
        {
            A.IsFalse(o);
        }

        public static void AreNotEqual(object notExpected, object actual)
        {
            A.AreNotEqual(notExpected, actual);
        }

        public static void IsNotNull(object? o)
        {
            A.IsNotNull(o);
        }

        public static void IsNull(object? o)
        {
            A.IsNull(o);
        }
    }

    public class TestPropertyAssertModel
    {
        public uint? Address { get; init; }
        public List<int>? Bytes { get; init; }
        public string? Description { get; init; }
        public int? Position { get; init; }
        public string Path { get; init; }
        public string Reference { get; init; }
        public int? Size { get; init; }
        public string Type { get; init; }
        public object? Value { get; init; }
    }

    public abstract class BaseTest
    {
        private IHost Server { get; }
        protected GameHookClient GameHookClient { get; }
        protected FakeDriver FakeDriver { get; }

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
                        .UseStartup<Startup>()
                        .ConfigureTestServices(x =>
                        {
                            x.Remove(x.Single(x => x.ServiceType == typeof(IGameHookDriver)));
                            x.AddSingleton<IGameHookDriver, FakeDriver>();
                        });
                })
                .Start();

            FakeDriver = (FakeDriver)Server.Services.GetRequiredService<IGameHookDriver>();
            GameHookClient = new GameHookClient("http://localhost:8085", Server.GetTestClient());
        }

        public async Task LoadMapperAndRamState(string mapperId, string mapperReplaceId, int n)
        {
            FakeDriver.LoadFakeMemoryAddressBlockResult($"{mapperId}-{n}.json");

            await GameHookClient.ChangeMapperAsync(new MapperReplaceModel()
            {
                Id = mapperReplaceId
            });
        }

        public async Task Load_GB_PokemonYellow(int n) => await LoadMapperAndRamState("b2b7d2d6-5cf0-4db1-9152-1efc7fe36926", "ff4d0e23c73b21068ef1f5deffb6b6ea", n);
        public async Task Load_GBA_PokemonEmerald(int n) => await LoadMapperAndRamState("005fdd01-8921-468c-aca3-d4fa864d5911", "5708b192924d1503cb0f181c192abe72", n);
    }
}
