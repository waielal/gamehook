using GameHook.WebAPI;
using Serilog;

public class Program
{
    public static void Main()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");

        try
        {
            // TODO: 12/27/2023 - Remove this at a future date. Logs are now stored within %APPDATA%\GameHook.
            if (File.Exists("GameHook.log"))
            {
                File.Delete("GameHook.log");
            }

            if (File.Exists(BuildEnvironment.LogFilePath))
            {
                File.WriteAllText(BuildEnvironment.LogFilePath, string.Empty);
            }

            Log.Logger = new LoggerConfiguration()
                                .WriteTo.Console()
                                .WriteTo.File(BuildEnvironment.LogFilePath)
                                .CreateBootstrapLogger();

            Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(x => x.UseStartup<Startup>())
                    .ConfigureAppConfiguration(x =>
                    {
                        // Add a custom appsettings.user.json file if
                        // the user wants to override their settings.

                        x.AddJsonStream(EmbededResources.appsettings_json);
                        x.AddJsonFile(BuildEnvironment.ConfigurationDirectoryAppsettingsFilePath, true, false);
                        x.AddJsonFile(BuildEnvironment.BinaryDirectoryGameHookFilePath, true, false);

                        if (BuildEnvironment.IsTestingBuild)
                        {
                            x.AddJsonFile("appsettings.Development.json", true);
                        }
                    })
                    .UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
                    .Build()
                    .Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "GameHook startup failed!");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}