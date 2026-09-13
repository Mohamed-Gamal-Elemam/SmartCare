using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartCare.Services.Abstraction.DTOs.Clinics;

public class UpdateClinicDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public List<UpdateClinicWorkingHourDto> WorkingHours { get; set; } = [];
}