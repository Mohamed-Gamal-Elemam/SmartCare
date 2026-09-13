using System;
using System.Collections.Generic;

namespace SmartCare.Services.Abstraction.DTOs.Clinics;

public class ClinicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<ClinicWorkingHourDto> WorkingHours { get; set; } = [];
}