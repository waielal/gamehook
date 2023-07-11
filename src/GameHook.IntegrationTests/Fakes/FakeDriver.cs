using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using GameHook.Domain;
using GameHook.Domain.Interfaces;

namespace GameHook.IntegrationTests.Fakes
{
    public class FakeDriver : IGameHookDriver
    {
        public string ProperName => "Fake";

        private IEnumerable<MemoryAddressBlockResult> LoadedFake { get; set; } = new List<MemoryAddressBlockResult>();

        public void LoadFakeMemoryAddressBlockResult(string filename)
        {
            var json = File.ReadAllText($"./Data/MemoryAddressBlocks/{filename}");

            LoadedFake = JsonSerializer.Deserialize<IEnumerable<MemoryAddressBlockResult>>(json)
                ?? throw new System.Exception("Could not deserialize fake memory address block result.");
        }

        public async Task<IEnumerable<MemoryAddressBlockResult>> ReadBytes(IPlatformOptions? platformOptions, IEnumerable<MemoryAddressBlock> blocks)
        {
            await Task.FromResult(LoadedFake);
            return LoadedFake;
        }

        public async Task WriteBytes(IPlatformOptions? platformOptions, uint startingMemoryAddress, byte[] values)
        {
            await Task.CompletedTask;
        }
    }
}
