using System.Reflection;
using System.Text.Json.Serialization;
using GameHook.Application;
using GameHook.Domain;
using GameHook.Domain.Drivers;
using GameHook.Domain.Infrastructure;
using GameHook.Domain.Interfaces;
using GameHook.WebAPI.ClientNotifiers;
using GameHook.WebAPI.Hubs;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameHook.WebAPI
{
    public class Startup
    {
        private GameHookConfiguration Configuration { get; }
        private IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = new GameHookConfiguration(configuration);
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddCors();

            // Add Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(x =>
            {
                x.DocumentFilter<DefaultSwashbuckleFilter>();

                x.EnableAnnotations();

                // Use method name as operationId
                x.CustomOperationIds(apiDesc =>
                {
                    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                });

                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "GameHook API",
                    Contact = new OpenApiContact
                    {
                        Name = "GameHook Team",
                        Url = new Uri("https://github.com/gamehook-io/gamehook")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "GNU Affero General Public License v3.0",
                        Url = new Uri("https://github.com/gamehook-io/gamehook/blob/main/LICENSE.txt")
                    }
                });
            });

            // Add Web API
            services
                .AddControllers()
                .AddApplicationPart(typeof(Program).Assembly)
                .AddControllersAsServices()
                .AddProblemDetailsConventions()
                .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

            services.AddSignalR();

            services.AddProblemDetails((options) =>
            {
                options.ShouldLogUnhandledException = (ctx, e, d) => true;

                options.IncludeExceptionDetails = (ctx, ex) =>
                {
                    var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    return env.IsDevelopment();
                };
            });

            // Register application classes.
            services.AddSingleton<DriverOptions>();
            services.AddSingleton<IMapperFilesystemProvider, MapperFilesystemProvider>();
            services.AddSingleton<IMapperUpdateManager, MapperUpdateManager>();
            services.AddSingleton<IGameHookDriver, RetroArchUdpPollingDriver>();
            services.AddSingleton<GameHookInstance>();
            services.AddSingleton<IClientNotifier, WebSocketClientNotifier>();
            services.AddSingleton<IClientNotifier, FilesystemClientNotifier>();
            services.AddSingleton<GameHookConfiguration>();
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger, GameHookConfiguration configuration, IMapperUpdateManager updateManager)
        {
            if (BuildEnvironment.IsTestingBuild)
            {
                logger.LogWarning("WARNING: This is a debug build for testing!");
                logger.LogWarning("Please upgrade to the latest stable release.");
            }
            else
            {
                logger.LogInformation($"Starting GameHook version {BuildEnvironment.AssemblyProductVersion}.");
            }

            Directory.CreateDirectory(BuildEnvironment.ConfigurationDirectory);
            Directory.CreateDirectory(BuildEnvironment.ConfigurationDirectoryUiBuilderScreenDirectory);
            
            updateManager.CheckForUpdates().GetAwaiter().GetResult();

            app.UseCors(x =>
            {
                x.SetIsOriginAllowed(x => true);
                x.AllowAnyMethod();
                x.AllowAnyHeader();
                x.AllowCredentials();
            });

            // Use Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseSerilogRequestLogging();
            app.UseProblemDetails();

            app.UseRouting();

            if (BuildEnvironment.IsDebug)
            {
                app.UseStaticFiles();
            }

            app.UseEndpoints(x =>
            {
                if (BuildEnvironment.IsDebug)
                {
                    x.MapGet("/", () => Results.Redirect("index.html", false));
                }
                else
                {
                    x.MapGet("/", () => Results.File(EmbededResources.index_html, contentType: "text/html"));
                    x.MapGet("/favicon.ico", () => Results.File(EmbededResources.favicon_ico, contentType: "image/x-icon"));
                    x.MapGet("/site.css", () => Results.File(EmbededResources.site_css, contentType: "text/css"));
                    x.MapGet("/dist/gameHookMapperClient.js", () => Results.File(EmbededResources.dist_gameHookMapperClient_js, contentType: "application/javascript"));
                }

                x.MapControllers();

                x.MapHub<UpdateHub>("/updates");
            });

            logger.LogInformation("GameHook is now online.");
            logger.LogInformation($"UI accessible via {string.Join(", ", configuration.Urls)}");

            if (configuration.OutputAllPropertiesToFilesystem)
            {
                logger.LogInformation("Outputting all properties to filesystem.");
            }
        }
    }
}