using GameHook.Domain.DTOs;

namespace GameHook.Domain.Interfaces
{
    public interface IContainerForDriver
    {
        IPlatformOptions PlatformOptions { get; }

        Task OnDriverMemoryChanged(MemoryAddress address, int length, byte[] newValue);
        Task OnDriverError(ProblemDetailsForClientDTO problemDetails, Exception ex);
        Task OnDriverMemoryTimeout(DriverTimeoutException ex);
    }
}
