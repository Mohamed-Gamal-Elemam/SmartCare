using AutoMapper;
using SmartCare.Core.Entities;
using SmartCare.Services.Abstraction.DTOs.Clinic;
using SmartCare.Services.Abstraction.DTOs.Clinics;

namespace SmartCare.Services.Mapping;

public class ClinicMappingProfile : Profile
{
    public ClinicMappingProfile()
    {
        // ==========================================
        // 1. Clinic Working Hour Mappings
        // ==========================================

        // Entity -> DTO
        CreateMap<ClinicWorkingHour, ClinicWorkingHourDto>();

        // Create DTO -> Entity
        CreateMap<CreateClinicWorkingHourDto, ClinicWorkingHour>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ClinicId, opt => opt.Ignore())
            .ForMember(dest => dest.Clinic, opt => opt.Ignore());

        // Update DTO -> Entity
        CreateMap<UpdateClinicWorkingHourDto, ClinicWorkingHour>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ClinicId, opt => opt.Ignore())
            .ForMember(dest => dest.Clinic, opt => opt.Ignore());

        // ==========================================
        // 2. Clinic Main Mappings
        // ==========================================

        // Entity -> DTO
        CreateMap<Clinic, ClinicDto>()
            .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src => src.WorkingHours));

        // Create DTO -> Entity
        CreateMap<CreateClinicDto, Clinic>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src => src.WorkingHours));

        // Update DTO -> Entity
        CreateMap<UpdateClinicDto, Clinic>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src => src.WorkingHours));
    }
}