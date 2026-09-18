using AutoMapper;
using SmartCare.Core.Entities.DoctorAvailabilitys;
using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;

namespace SmartCare.Services.Mapping
{
    public class DoctorAvailabilitys :Profile
    {
        public DoctorAvailabilitys()
        {
            // Availability mappings
            CreateMap<DoctorAvailability, DoctorAvailabilityDto>();
            CreateMap<CreateDoctorAvailabilityDto, DoctorAvailability>()
                .ForMember(dest => dest.Breaks, opt => opt.MapFrom(src => src.Breaks));

            // Breaks mappings
            CreateMap<DoctorAvailabilityBreak, DoctorAvailabilityBreakDto>();
            CreateMap<CreateDoctorAvailabilityBreakDto, DoctorAvailabilityBreak>();

            // Exceptions mappings
            CreateMap<DoctorAvailabilityException, DoctorAvailabilityExceptionDto>();
            CreateMap<CreateDoctorAvailabilityExceptionDto, DoctorAvailabilityException>();
        }
    }
}
