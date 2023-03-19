using GameHook.Application;
using GameHook.Domain;
using GameHook.Domain.Interfaces;

namespace GameHook.GenerateBindings
{
    internal class FakeGameHookInstance : IGameHookInstance
    {
        public List<IClientNotifier> ClientNotifiers => throw new NotImplementedException();

        public bool Initalized => throw new NotImplementedException();

        public IGameHookDriver? Driver => throw new NotImplementedException();

        public IGameHookMapper? Mapper => throw new NotImplementedException();

        public PreprocessorCache? PreprocessorCache => throw new NotImplementedException();

        public IEnumerable<MemoryAddressBlock>? BlocksToRead => throw new NotImplementedException();

        public IPlatformOptions? PlatformOptions => throw new NotImplementedException();

        public IGameHookDriver GetDriver()
        {
            throw new NotImplementedException();
        }

        public IGameHookMapper GetMapper()
        {
            throw new NotImplementedException();
        }

        public IPlatformOptions GetPlatformOptions()
        {
            throw new NotImplementedException();
        }

        public Task Load(IGameHookDriver driver, string mapperId)
        {
            throw new NotImplementedException();
        }

        public Task Read()
        {
            throw new NotImplementedException();
        }

        public Task ReadLoop()
        {
            throw new NotImplementedException();
        }

        public Task ResetState()
        {
            throw new NotImplementedException();
        }
    }
}
