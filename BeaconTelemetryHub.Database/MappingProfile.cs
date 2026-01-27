using AutoMapper;
using BeaconTelemetryHub.Database.Models;
using BeaconTelemetryHub.DataContracts.Models;

namespace BeaconTelemetryHub.Database
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TemperatureDto, Temperature>();
            CreateMap<BatteryDto, Battery>();
            CreateMap<TemperatureDto, Beacon>();
            CreateMap<BatteryDto, Beacon>();
            CreateMap<RssiDto, Rssi>().ForMember(x => x.SignalStrengthdBm, opt => opt.MapFrom(s => s.Rssi));
        }
    }
}
