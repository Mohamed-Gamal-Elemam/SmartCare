using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.DTOs.Authentication
{
    public class RegisterResponseDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool RequiresEmailVerification { get; set; }
    }
}
