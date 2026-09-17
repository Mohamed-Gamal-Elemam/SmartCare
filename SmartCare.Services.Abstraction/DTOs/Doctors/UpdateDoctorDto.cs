using System;

namespace SmartCare.Services.Abstraction.DTOs.Doctors;

public class UpdateDoctorDto
{
    public Guid SpecialtyId { get; set; }

    public Guid ClinicId { get; set; }

    public string Biography { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public decimal ConsultationFee { get; set; }
}