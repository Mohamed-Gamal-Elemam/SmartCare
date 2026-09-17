using Microsoft.AspNetCore.Identity;
using SmartCare.Core.Entities.Doctors;


namespace SmartCare.Core.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual Doctor? Doctor { get; set; }
    }
}
    