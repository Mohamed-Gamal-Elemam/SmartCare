using AutoMapper;
using SmartCare.Core.Entities;
using SmartCare.Services.Abstraction.DTOs.Specialty;

namespace SmartCare.Services.Mapping;

public class SpecialtyProfile : Profile
{
    public SpecialtyProfile()
    {
        CreateMap<Specialty, SpecialtyResponseDto>();

        CreateMap<CreateSpecialtyDto, Specialty>();

        CreateMap<UpdateSpecialtyDto, Specialty>();
    }
}