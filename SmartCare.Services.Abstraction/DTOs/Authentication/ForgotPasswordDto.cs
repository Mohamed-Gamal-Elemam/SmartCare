using System.ComponentModel.DataAnnotations;

namespace SmartCare.Services.Abstraction.DTOs.Authentication;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}