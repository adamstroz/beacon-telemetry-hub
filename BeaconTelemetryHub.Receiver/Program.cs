using BeaconTelemetryHub.Receiver.Beacon.BeaconFinder;
using BeaconTelemetryHub.Receiver.Beacon.DataParser;
using BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers;
using BeaconTelemetryHub.Receiver.Beacon.Models;
using BeaconTelemetryHub.Receiver.Bluetooth;
using BeaconTelemetryHub.Receiver.Bluetooth.LinuxBle;
using BeaconTelemetryHub.Receiver.Settings;
using Linux.Bluetooth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace BeaconTelemetryHub.Receiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                        .WriteTo.Console()
                        
                        .CreateLogger();
            

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);

            hostBuilder.ConfigureServices((context, services) =>
            {

                if (OperatingSystem.IsLinux())
                {
                    services.AddSingleton<IBleAdapterBuilder, LinuxAdapterBuilder>();
                    // Wrapper is registered in DI to support Disposable pattern when host is disposed
                    services.AddTransient<BleLinuxAdapterWrapper>();
                    services.AddSingleton<GetBleLinuxAdapterWrapperDelegate>(provider => () => provider.GetRequiredService<BleLinuxAdapterWrapper>());
                }
                else if (OperatingSystem.IsWindows())
                {
                    //services.AddTransient<IBluetoothAdapterFinder, WindowsAdapterFinder>();
                }
                else
                {
                    throw new InvalidOperationException("Invalid OS!)");
                }
                services.AddSingleton<TelemetryAParser>();
                services.AddSingleton<IBeaconTelemeteryGeneralParser>(sp => sp.GetRequiredService<TelemetryAParser>());
                services.AddSingleton<IBeaconTelemeteryDetailedParser<BeaconTelemetryA>>(sp => sp.GetRequiredService<TelemetryAParser>());

                services.AddSingleton<TelemetryAExtendedParser>();
                services.AddSingleton<IBeaconTelemeteryGeneralParser>(sp => sp.GetRequiredService<TelemetryAExtendedParser>());
                services.AddSingleton<IBeaconTelemeteryDetailedParser<BeaconTelemetryAExtended>>(sp => sp.GetRequiredService<TelemetryAExtendedParser>());

                services.AddSingleton<TelemetryBParser>();
                services.AddSingleton<IBeaconTelemeteryGeneralParser>(sp => sp.GetRequiredService<TelemetryBParser>());
                services.AddSingleton<IBeaconTelemeteryDetailedParser<BeaconTelemetryB>>(sp => sp.GetRequiredService<TelemetryBParser>());

                services.AddSingleton<TelemetryBExtendedParser>();
                services.AddSingleton<IBeaconTelemeteryGeneralParser>(sp => sp.GetRequiredService<TelemetryBExtendedParser>());
                services.AddSingleton<IBeaconTelemeteryDetailedParser<BeaconTelemetryBExtended>>(sp => sp.GetRequiredService<TelemetryBExtendedParser>());

                services.AddSingleton<IBeaconTelemetryResolver, BeaconTelemetryResolver>();
                services.AddSingleton<IBeaconFinder, BeaconFinder>();
                services.AddHostedService<BeaconBackgroundService>();
                services.Configure<BeaconReceiverSettings>(
                    context.Configuration.GetSection("BeaconReceiverSettings"));
            });
  

            using IHost host = hostBuilder.Build();

            await host.StartAsync();

            await host.WaitForShutdownAsync();

        }
    
         
    }
}