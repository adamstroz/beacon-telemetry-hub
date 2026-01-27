
namespace BeaconTelemetryHub.DataContracts.DataStore
{
    /// <summary>
    /// Defines a centralized data store interface for beacon telemetry.
    /// Acts as the primary service for orchestrating data distribution 
    /// across multiple data sinks or persistence layers.
    /// </summary>
    public interface IBeaconDataStore : IBeaconDataWriter;
}
