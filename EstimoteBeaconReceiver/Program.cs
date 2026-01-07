using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.Bluetooth.LinuxBle;
using EstimoteBeaconReceiver.EstimoteBeacon;
using EstimoteBeaconReceiver.Settings;
using Linux.Bluetooth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace EstimoteBeaconReceiver
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