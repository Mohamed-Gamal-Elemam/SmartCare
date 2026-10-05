using AutoMapper;
using SmartCare.Core.Entities.Appointments;
using SmartCare.Services.Abstraction.DTOs.Appointments;

namespace SmartCare.Services.MappingProfiles;

public class AppointmentProfile : Profile
{
    public AppointmentProfile()
    {
        CreateMap<Appointment, AppointmentDto>();
        CreateMap<Appointment, AppointmentDetailsDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.User.FirstName} {src.Patient.User.LastName}"))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => $"{src.Doctor.User.FirstName} {src.Doctor.User.LastName}"))
            .ForMember(dest => dest.SpecialtyName, opt => opt.MapFrom(src => src.Doctor.Specialty.Name))
            .ForMember(dest => dest.ClinicId, opt => opt.MapFrom(src => src.Doctor.ClinicId))
            .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Doctor.Clinic != null ? src.Doctor.Clinic.Name : null));
    }
}