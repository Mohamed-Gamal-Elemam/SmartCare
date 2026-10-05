using System;
using System.Collections.Generic;

namespace SmartCare.Services.Abstraction.DTOs.DoctorAvailability;

public class UpdateDoctorAvailabilityDto
{
    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int SlotDurationInMinutes { get; set; }

    public List<CreateDoctorAvailabilityBreakDto> Breaks { get; set; } = [];
}