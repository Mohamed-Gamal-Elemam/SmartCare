using System;

namespace SmartCare.Services.Abstraction.DTOs.Clinics;

public class ClinicWorkingHourDto
{
    //public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public bool IsClosed { get; set; }
}