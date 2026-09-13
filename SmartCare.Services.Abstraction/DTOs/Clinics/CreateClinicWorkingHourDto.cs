using System;
using System.ComponentModel.DataAnnotations;

namespace SmartCare.Services.Abstraction.DTOs.Clinics;

public class CreateClinicWorkingHourDto
{
    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public bool IsClosed { get; set; }
}