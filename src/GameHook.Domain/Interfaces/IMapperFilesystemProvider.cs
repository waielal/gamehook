using GameHook.Domain.DTOs;

namespace GameHook.Domain.Interfaces
{
    public interface IMapperFilesystemProvider
    {
        IEnumerable<MapperFilesystemDTO> MapperFiles { get; }

        void CacheMapperFiles();
    }
}
