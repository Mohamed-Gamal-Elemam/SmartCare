using FluentValidation;
using SmartCare.Services.Abstraction.DTOs.Doctors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Validations.Doctor
{
    public class CreateDoctorDtoValidator : AbstractValidator<CreateDoctorDto>
    {
        public CreateDoctorDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.SpecialtyId)
                .NotEmpty().WithMessage("Specialty ID is required.");

            RuleFor(x => x.ClinicId)
                .NotEmpty().WithMessage("Clinic ID is required.");

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.")
                .LessThanOrEqualTo(60).WithMessage("Years of experience must be realistic (less than or equal to 60).");

            RuleFor(x => x.ConsultationFee)
                .GreaterThan(0).WithMessage("Consultation fee must be greater than zero.");

            RuleFor(x => x.Biography)
                .MaximumLength(1000).WithMessage("Biography cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Biography));
        }
    }
}
