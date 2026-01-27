using BeaconTelemetryHub.Receiver.Beacon.BeaconFinder;
using BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers;
using BeaconTelemetryHub.Receiver.Beacon.DataParser;
using BeaconTelemetryHub.Receiver.Beacon.Models;
using Microsoft.Extensions.DependencyInjection;
using BeaconTelemetryHub.Receiver.Bluetooth.LinuxBle;
using BeaconTelemetryHub.Receiver.Bluetooth;
using Microsoft.Extensions.Configuration;

namespace BeaconTelemetryHub.Receiver
{
    public static class ReceiverServiceCollectionExtensions
    {
        private static readonly string _isRunningInContainerFlagKey = "DOTNET_RUNNING_IN_CONTAINER";
        public static void RegisterBeaconTelemetryReceiverServices(this IServiceCollection services, IConfiguration configuration )
        {
            try
            {
                if (OperatingSystem.IsLinux())
                {
                    services.AddSingleton<IBleAdapterBuilder, LinuxAdapterBuilder>();
                }
                else if (OperatingSystem.IsWindows())
                {
                    string? isRunningInContainerFlag = configuration[_isRunningInContainerFlagKey];

                    if (isRunningInContainerFlag != null &&
                        bool.Parse(isRunningInContainerFlag))
                    {
                        throw new NotSupportedException("Windows containers are not supported!");
                    }

                    // TODO: Add support for Windows
                    throw new NotImplementedException("Windows BLE adapter is not implemented");
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
