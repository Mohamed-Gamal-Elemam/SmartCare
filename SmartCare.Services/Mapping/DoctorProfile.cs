using AutoMapper;
using SmartCare.Core.Entities.Doctors;
using SmartCare.Services.Abstraction.DTOs.Doctors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Mapping
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));

            CreateMap<CreateDoctorDto, Doctor>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Specialty, opt => opt.Ignore())
                .ForMember(dest => dest.Clinic, opt => opt.Ignore());

            CreateMap<UpdateDoctorDto, Doctor>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Specialty, opt => opt.Ignore())
                .ForMember(dest => dest.Clinic, opt => opt.Ignore());
        }
    }
}
