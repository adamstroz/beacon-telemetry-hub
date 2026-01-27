using AutoMapper;
using BeaconTelemetryHub.DataContracts.Models;
using BeaconTelemetryHub.Receiver.Beacon.Models;

namespace BeaconTelemetryHub.Host
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BeaconTelemetryB, TemperatureDto>()
                     .ForCtorParam(nameof(TemperatureDto.TemperatureCelsius), o => o.MapFrom(s => s.Temperature.DegreesCelsius))
                     .ForCtorParam(nameof(TemperatureDto.DeviceIdentifier), o => o.MapFrom(s => s.DeviceIdentifier))
                     .ForCtorParam(nameof(TemperatureDto.DeviceBleAddress), o => o.MapFrom(s => s.DeviceBleAddress.Address))
                     .ForCtorParam(nameof(TemperatureDto.FoundTimestamp), o => o.MapFrom(s => s.FoundTimestamp));
            CreateMap<BeaconTelemetryB, BatteryDto>()
                     .ForCtorParam(nameof(BatteryDto.BatteryLevelPercent), o => o.MapFrom(s => s.BatteryLevel.HasValue ? s.BatteryLevel.Value.Percent : -1))
                     .ForCtorParam(nameof(BatteryDto.BatteryVoltageMillivolts), o => o.MapFrom(s => s.BatteryVoltage.HasValue ? s.BatteryVoltage.Value.Millivolts: -1))
                     .ForCtorParam(nameof(BatteryDto.DeviceIdentifier), o => o.MapFrom(s => s.DeviceIdentifier))
                     .ForCtorParam(nameof(BatteryDto.DeviceBleAddress), o => o.MapFrom(s => s.DeviceBleAddress.Address))
                     .ForCtorParam(nameof(BatteryDto.FoundTimestamp), o => o.MapFrom(s => s.FoundTimestamp));
            // BeaconTelemetryBase contains RSSI, and all telemetry models inherit from it,
            // so mapping only the base telemetry is enough to get the RssiDto model.
            CreateMap<BeaconTelemetryBase, RssiDto>()
                     .ForCtorParam(nameof(BeaconTelemetryBase.DeviceIdentifier), o => o.MapFrom(s => s.DeviceIdentifier))
                     .ForCtorParam(nameof(BeaconTelemetryBase.DeviceBleAddress), o => o.MapFrom(s => s.DeviceBleAddress.Address))
                     .ForCtorParam(nameof(BeaconTelemetryBase.FoundTimestamp), o => o.MapFrom(s => s.FoundTimestamp))
                     .ForCtorParam(nameof(BeaconTelemetryBase.Rssi), o => o.MapFrom(s => s.Rssi));
        }

    }
}
