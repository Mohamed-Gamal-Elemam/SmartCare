using System.ComponentModel.DataAnnotations;

namespace SmartCare.Services.Abstraction.DTOs.Authentication;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}