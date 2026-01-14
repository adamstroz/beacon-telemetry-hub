using BeaconTelemetryHub.Receiver.Beacon.BeaconFinder;
using BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers;
using BeaconTelemetryHub.Receiver.Beacon.DataParser;
using BeaconTelemetryHub.Receiver.Beacon.Models;
using BeaconTelemetryHub.Receiver.Settings;
using Microsoft.Extensions.DependencyInjection;
using BeaconTelemetryHub.Receiver.Bluetooth.LinuxBle;
using BeaconTelemetryHub.Receiver.Bluetooth;

namespace BeaconTelemetryHub.Receiver
{
    public static class ReceiverServiceCollectionExtensions
    {
        public static void RegisterBeaconTelemetryReceiverServices(this IServiceCollection services)
        {
            try
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
                    throw new NotImplementedException("Windows BLE adapter is not supported");
                }
                else 
                {
                    throw new NotSupportedException($"Not supported Operating system: {Environment.OSVersion}");
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
            }
            catch (NotImplementedException)
            {
                throw;
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Cannot register telemetry receiver services to the host builder", ex);
            }
        }
    }
}
