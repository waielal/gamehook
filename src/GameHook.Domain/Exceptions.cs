namespace GameHook.Domain
{
    public class SanityCheckException : Exception
    {
        public SanityCheckException(string message) : base(message) { }
    }

    public abstract class UserPresentableException : Exception
    {
        public UserPresentableException(string title, Exception? innerException) : base(title, innerException)
        {
            Title = title;
        }

        public string Title { get; }
    }

    public class DriverTimeoutException : Exception
    {
        public MemoryAddress MemoryAddress { get; }

        public DriverTimeoutException(MemoryAddress address, string driverName, Exception? innerException)
            : base($"A timeout occurred when reading address {address.ToHexdecimalString()}. Is {driverName} running and accessible?", innerException)
        {
            MemoryAddress = address;
        }
    }

    public class DriverErrorException :Exception
    {
        public DriverErrorException(string message) : base(message) { }
    }

    public class PropertyProcessException : Exception
    {
        public PropertyProcessException(string message, Exception? innerException) : base(message, innerException) { }
    }

    public class GenericUserPresentableException : UserPresentableException
    {
        public GenericUserPresentableException(string message) : base(message, null) { }
    }

    public class GameHookContainerInitializationException : UserPresentableException
    {
        public GameHookContainerInitializationException(string message, Exception? innerException) : base(message, innerException) { }
    }

    public class MapperParsingException : Exception
    {
        public MapperParsingException(string message, Exception? innerException = null) : base(message, innerException) { }
    }

    public class DriverShutdownException : Exception
    {
        public DriverShutdownException(Exception? innerException)
            : base("RetroArch driver is shutting down due to lost connection.", innerException) { }
    }
}
