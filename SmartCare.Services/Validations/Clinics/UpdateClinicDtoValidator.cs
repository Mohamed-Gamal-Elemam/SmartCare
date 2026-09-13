using FluentValidation;
using SmartCare.Services.Abstraction.DTOs.Clinics;

namespace SmartCare.Services.Validations.Clinics;

public class UpdateClinicDtoValidator : AbstractValidator<UpdateClinicDto>
{
    public UpdateClinicDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Clinic name is required.")
            .MaximumLength(150).WithMessage("Clinic name must not exceed 150 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(300).WithMessage("Address must not exceed 300 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
            .Matches(@"^\+?[0-9\s\-()]+$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.WorkingHours)
            .NotEmpty().WithMessage("Working hours are required.")
            .Must(hours => hours.Select(h => h.DayOfWeek).Distinct().Count() == hours.Count)
            .WithMessage("Duplicate days are not allowed in working hours.");

        RuleForEach(x => x.WorkingHours).SetValidator(new UpdateClinicWorkingHourDtoValidator());
    }
}