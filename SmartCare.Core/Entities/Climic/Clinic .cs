using System.Numerics;

namespace SmartCare.Core.Entities.Climic;

public class Clinic : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ClinicWorkingHour> WorkingHours { get; set; } = new List<ClinicWorkingHour>();

    //public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}