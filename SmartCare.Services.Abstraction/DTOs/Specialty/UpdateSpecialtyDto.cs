using System.ComponentModel.DataAnnotations;

namespace SmartCare.Services.Abstraction.DTOs.Specialty;

public class UpdateSpecialtyDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}