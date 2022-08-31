using System.Reflection;

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

            throw new Exception("Cannot determine application AssemblyVersion.");
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

            throw new Exception("Cannot determine application AssemblyProductVersion.");
        }
    }

    public static string BinaryDirectory =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Could not determine the binary directory.");

    public static string ConfigurationDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GameHook");

    public static string MapperUserSettingsDirectory => Path.Combine(ConfigurationDirectory, "MapperUserSettings");

    public static string AppsettingsFilePath => Path.Combine(ConfigurationDirectory, "GameHook.json");
    public static string AppsettingsFilePath2 => Path.Combine(BinaryDirectory, "GameHook.json");
    public static string DebugAppsettingsFilePath => Path.Combine(ConfigurationDirectory, "GameHook.debug.json");

#if DEBUG
    public static bool IsDebug = true;
    public static bool IsTestingBuild => true;
    public static bool IsPublicBuild => false;
#else
    public static bool IsDebug = false;
    public static bool IsTestingBuild => AssemblyVersion == "0.0.0.0";
    public static bool IsPublicBuild => AssemblyVersion != "0.0.0.0";
#endif
}