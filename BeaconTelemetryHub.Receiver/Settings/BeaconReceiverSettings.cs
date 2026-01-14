namespace BeaconTelemetryHub.Receiver.Settings
{
    public record BeaconReceiverSettings()
    {
        /// <summary>
        /// Name of the Bluetooth adapter used for scanning.
        /// If null, the default system adapter will be used.
        /// </summary>
        public string? AdapterName { get; init; } = null;

        /// <summary>
        /// Service UUID used by Estimote beacons to broadcast packets
        /// (Telemetry, Connectivity, etc.) as BLE Service Data.
        /// According to the BLE specification (§1.11 — Service Data, 16-bit UUID),
        /// Estimote uses the service UUID "fe9a".
        /// </summary>
        public string EstimoteServiceUUID { get; set; } = "fe9a";

        /// <summary>
        /// Telemetry Packet Type identifier for Estimote Telemetry frames.
        /// This value is located at byte index 9 of the Service Data payload.
        /// For Telemetry frames, the type is always 2 (0b0010).
        /// </summary>
        public byte EstimoteTelemetryPacketTypeId { get; set; } = 0x02;

        /// <summary>
        /// Duration of a single scan operation.
        /// Specifies how long the system actively performs scanning.
        /// </summary>
        public TimeSpan ScanDuration { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Time interval between consecutive scan operations.
        /// Specifies how often a new scan is started.
        /// </summary>
        public TimeSpan ScanInterval { get; set; } = TimeSpan.FromMinutes(1);

    }
}
