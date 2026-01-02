using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.Bluetooth.Linux;
using EstimoteBeaconReceiver.EstimoteBeacon;
using EstimoteBeaconReceiver.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EstimoteBeaconReceiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);

            hostBuilder.ConfigureServices((context, services) =>
            {
           
                if (OperatingSystem.IsLinux())
                {
                    services.AddTransient<IBleAdapterFinder, LinuxAdapterFinder>();
                }
                else if (OperatingSystem.IsWindows())
                {
                    //services.AddTransient<IBluetoothAdapterFinder, WindowsAdapterFinder>();
                }
                else
                {
                    throw new InvalidOperationException("Invalid OS!)");
                }

                services.AddTransient<IBeaconFinder, BeaconFinder>();
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