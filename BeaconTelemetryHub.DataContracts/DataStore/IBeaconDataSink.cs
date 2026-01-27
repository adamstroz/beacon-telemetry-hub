
namespace BeaconTelemetryHub.DataContracts.DataStore
{
    /// <summary>
    /// Defines a data sink for beacon telemetry
    /// Implementations of this interface are responsible for persisting telemetry data 
    /// to specific storage targets (e.g., database, cloud, or external API).
    /// </summary>
    public interface IBeaconDataSink : IBeaconDataWriter;
}
