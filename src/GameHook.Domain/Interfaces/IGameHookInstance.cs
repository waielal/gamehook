namespace GameHook.Domain.Interfaces
{
    public interface IGameHookInstance
    {
        bool Initalized { get; }
        Dictionary<string, object?> State { get; }
        Dictionary<string, object?> Variables { get; }

        List<IClientNotifier> ClientNotifiers { get; }
        IGameHookDriver? Driver { get; }
        IGameHookMapper? Mapper { get; }
        IPlatformOptions? PlatformOptions { get; }

        Task Load(IGameHookDriver driver, string mapperId);

        object? EvalulateExpression_Type1(string function, object? value);
        bool? ExecuteFunction_Type1(string? function, IGameHookProperty property);
        byte[] ExecuteFunction_Type2(string function, byte[] bytes, IGameHookProperty property);
    }
}
