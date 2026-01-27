using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using BeaconTelemetryHub.Receiver.Settings;
using BeaconTelemetryHub.Receiver;
using System.Reflection;
using System.Runtime.InteropServices;
using AutoMapper;
using Microsoft.Extensions.Logging;
using BeaconTelemetryHub.Database;
using BeaconTelemetryHub.DataContracts;
using System.Diagnostics;
using System;
namespace BeaconTelemetryHub.Host
{
    /// <summary>
    /// Defines the exit codes for the application to indicate the final execution status to system.
    /// </summary>
    public enum ExitCode
    {
        /// <summary>
        /// The application executed successfully without any issues.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The application encountered an error and had to terminate.
        /// </summary>
        Error = 1
    }
    internal static class Program
    {
        private static void LogApplicationInformation(this IHost host)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "Unknown";
            var env = host.Services.GetRequiredService<IHostEnvironment>();

            Log.Information("======================================================");
            Log.Information(" Application Info");
            Log.Information(" Name: {AppName}", env.ApplicationName);
            Log.Information(" Version: {AppVersion}", version);
            Log.Information(" Environment: {Environment}", env.EnvironmentName);
            Log.Information(" OS: {OSDescription}", RuntimeInformation.OSDescription);
            Log.Information(" Machine Name: {MachineName}", Environment.MachineName);
            Log.Information(" .NET Version: {DotNetVersion}", RuntimeInformation.FrameworkDescription);
            Log.Information("======================================================");
        }
        static async Task<int> Main(string[] args)
        {
            _ = args;
            // Set ExitCode to default value (success)
            Environment.ExitCode = (int)ExitCode.Success;
            // Create default logger and write to console (at this moment we don't have settings configuration)
            Log.Logger = new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger();
            try
            {
                Log.Information("Starting the Beacon Telemetry Host builder...");
                IHostBuilder hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);

                hostBuilder.ConfigureServices((context, services) =>
                {
                    services.RegisterBeaconTelemetryReceiverServices(context.Configuration);
                    services.RegisterDataContractsServices();
                    services.RegisterDatabaseServices(context.Configuration);
                    services.AddAutoMapper(cfg =>
                    {
                        cfg.AddProfile<MappingProfile>();
                    });
                    services.AddHostedService<BeaconBackgroundService>();
                    services.Configure<BeaconTelemetryHubSettings>(
                                        context.Configuration.GetSection(nameof(BeaconTelemetryHubSettings)));
                    services.Configure<HostOptions>(hostOptions =>
                    {
                        hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
                    });
                });
                using IHost host = hostBuilder.Build();
                // Recreate a logger with settings from configuration
                Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(host.Services.GetRequiredService<IConfiguration>())
                            .CreateLogger();
                host.LogApplicationInformation();
                Log.Information("Host built successfully. Running services...");
                await host.RunAsync();
                return Environment.ExitCode;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The application terminated unexpectedly");
                Environment.ExitCode = (int)ExitCode.Error;
                return Environment.ExitCode;
            }
            finally
            {
                Log.Information("Shutting down the host...");
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
