using System;

namespace SmartCare.Services.Abstraction.DTOs.Doctors;

public class DoctorDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public Guid SpecialtyId { get; set; }
    public string SpecialtyName { get; set; } = string.Empty;

    public Guid ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;

    public string Biography { get; set; } = null!;
    public int YearsOfExperience { get; set; }
    public decimal ConsultationFee { get; set; }
    public bool IsActive { get; set; }
}