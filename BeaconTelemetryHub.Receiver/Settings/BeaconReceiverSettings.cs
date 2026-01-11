using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconTelemetryHub.Receiver.Settings
{
    internal record BeaconReceiverSettings()
    {
        public string? AdapterName { get; init; } = null;
        /*
         * Packest from the Estimote family (Telemetry, Connectivity, etc.) are
         * broadcast as Service Data (per "§ 1.11. The Service Data - 16 bit UUID" from
         * the BLE spec), with the Service UUID 'fe9a'.   
        */
        public string EstimoteServiceUUID { get; set; } = "fe9a";
        /* 
         * Estimote Telemetry packets are identified by the Telemetry Packet Type ID,
         * which is located at byte index 9 of the Service Data payload.
         * Frame type, for Telemetry it's always 2 (i.e., 0b0010)
        */
        public byte EstimoteTelemetryPacketTypeId { get;set; } = 0x02;
        public TimeSpan ScanDuration { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan ScanInterval { get; set; } = TimeSpan.FromMicroseconds(10);
    }
}
