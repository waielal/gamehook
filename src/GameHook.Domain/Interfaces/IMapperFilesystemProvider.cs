using GameHook.Domain.DTOs;

namespace GameHook.Domain.Interfaces
{
    public interface IMapperFilesystemProvider
    {
        string OfficialMapperFolder { get; }

        string? CustomMapperFolder { get; }

        IEnumerable<MapperFilesystemDTO> MapperFiles { get; }

        void RefreshMapperFiles();
    }
}
