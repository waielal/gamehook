global using GameHookGlossaryPage = System.Collections.Generic.IDictionary<byte, dynamic>;
global using GameHookGlossary = System.Collections.Generic.IDictionary<string, System.Collections.Generic.IDictionary<byte, dynamic>>;
global using GameHookMacros = System.Collections.Generic.IDictionary<string, System.Collections.Generic.IDictionary<object, object>>;
global using MemoryAddress = System.UInt32;
using System.Reflection;

public static class BuildEnvironment
{
    // 0.0.0
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

    // 0.0.0+dev
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