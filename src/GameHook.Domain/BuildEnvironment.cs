global using GameHookGlossaryPage = System.Collections.Generic.IDictionary<byte, dynamic>;
global using GameHookGlossary = System.Collections.Generic.IDictionary<string, System.Collections.Generic.IDictionary<byte, dynamic>>;
global using GameHookMacros = System.Collections.Generic.IDictionary<string, System.Collections.Generic.IDictionary<object, object>>;
global using MemoryAddress = System.Int32;
using System.Reflection;

public static class BuildEnvironment
{
    public static string AssemblyVersion
    {
        get
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var attributes = entryAssembly?.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return ((AssemblyVersionAttribute)attributes[0]).Version;
            }

            return "0.0.0";
        }
    }

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

            return "0.0.0+unknown";
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