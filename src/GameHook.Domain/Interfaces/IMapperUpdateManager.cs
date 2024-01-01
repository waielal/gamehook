namespace GameHook.Domain.Interfaces
{
    public interface IMapperUpdateManager
    {
        string MapperVersion { get; }

        Task<bool> CheckForUpdates();
    }
}
