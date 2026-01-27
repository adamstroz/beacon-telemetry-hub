using BeaconTelemetryHub.DataContracts.DataStore;
using Microsoft.Extensions.DependencyInjection;

namespace BeaconTelemetryHub.DataContracts
{
    public static class DataContractsServiceCollectionExtensions
    {
        public static void RegisterDataContractsServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IBeaconDataStore, BeaconDataStore>();
        }
    }
}
