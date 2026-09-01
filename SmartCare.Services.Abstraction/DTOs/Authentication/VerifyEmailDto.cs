using System.ComponentModel.DataAnnotations;

namespace SmartCare.Services.Abstraction.DTOs.Authentication;

public class VerifyEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}