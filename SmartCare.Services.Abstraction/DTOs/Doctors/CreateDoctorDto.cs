using System;

namespace SmartCare.Services.Abstraction.DTOs.Doctors;

public class CreateDoctorDto
{
    public Guid UserId { get; set; }

    public Guid SpecialtyId { get; set; }

    public Guid ClinicId { get; set; }

    public string Biography { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public decimal ConsultationFee { get; set; }
}