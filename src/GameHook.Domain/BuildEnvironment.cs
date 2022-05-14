global using GameHookGlossary = System.Collections.Generic.IDictionary<string, System.Collections.Generic.IEnumerable<GlossaryItem>>;
global using GameHookMacros = System.Collections.Generic.IDictionary<string, System.Collections.Generic.IDictionary<object, object>>;
global using MemoryAddress = System.UInt32;
using System.Reflection;

public class GlossaryItem
{
    public GlossaryItem(uint key, object? value)
    {
        Key = key;
        Value = value;
    }

    public uint Key { get; private set; }
    public object? Value { get; private set; }
}

public static class BuildEnvironment
{
    // 0.0.0.0
    public static string AssemblyVersion
    {
        get
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var attributes = entryAssembly?.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return ((AssemblyFileVersionAttribute)attributes[0]).Version;
            }

            return "";
        }
    }

    // 0.0.0.0+hash
    public static string AssemblyProductVersion
    {
        get
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var attributes = entryAssembly?.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
            }

            return "";
        }
    }

    public static string ConfigurationDirectory
    {
        get
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GameHook");
        }
    }

    public static string OutputPropertiesDirectory => Path.Combine(ConfigurationDirectory, "OutputProperties");

    public static string UserAppsettingsFilePath => Path.Combine(ConfigurationDirectory, "appsettings.user.json");
    public static string DebugAppsettingsFilePath => Path.Combine(ConfigurationDirectory, "appsettings.debug.json");

#if DEBUG
    public const bool IsDebugBuild = true;
    public const bool IsReleaseBuild = false;
    public const string ReleaseMode = "DEBUG";
#else
    public const bool IsDebugBuild = false;
    public const bool IsReleaseBuild = true;
    public const string ReleaseMode = "RELEASE";
#endif
}