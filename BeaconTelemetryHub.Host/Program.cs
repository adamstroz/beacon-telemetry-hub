using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using BeaconTelemetryHub.Receiver.Settings;
using BeaconTelemetryHub.Receiver;


namespace BeaconTelemetryHub.Host
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IHostBuilder hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);

            hostBuilder.ConfigureServices((context, services) =>
            {
               services.RegisterBeaconTelemetryReceiverServices();
               services.AddHostedService<BeaconBackgroundService>();
               services.Configure<BeaconReceiverSettings>(
                    context.Configuration.GetSection("BeaconReceiverSettings"));
 
            });
            using IHost host = hostBuilder.Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(host.Services.GetRequiredService<IConfiguration>())
                .CreateLogger();
            await host.StartAsync();
            await host.WaitForShutdownAsync();
        }
    }
}
