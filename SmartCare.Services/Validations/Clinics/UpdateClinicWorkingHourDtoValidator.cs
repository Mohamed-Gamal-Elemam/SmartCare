using FluentValidation;
using SmartCare.Services.Abstraction.DTOs.Clinics;

namespace SmartCare.Services.Validations.Clinics;

public class UpdateClinicWorkingHourDtoValidator : AbstractValidator<UpdateClinicWorkingHourDto>
{
    public UpdateClinicWorkingHourDtoValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("Invalid day of week.");

        When(x => x.IsClosed, () =>
        {
            RuleFor(x => x.OpenTime)
                .Null().WithMessage("Open time must be null when clinic is closed.");

            RuleFor(x => x.CloseTime)
                .Null().WithMessage("Close time must be null when clinic is closed.");
        });

        When(x => !x.IsClosed, () =>
        {
            RuleFor(x => x.OpenTime)
                .NotNull().WithMessage("Open time is required when clinic is open.");

            RuleFor(x => x.CloseTime)
                .NotNull().WithMessage("Close time is required when clinic is open.");

            RuleFor(x => x)
                .Must(x => x.OpenTime < x.CloseTime)
                .When(x => x.OpenTime.HasValue && x.CloseTime.HasValue)
                .WithMessage("Open time must be earlier than close time.");
        });
    }
}