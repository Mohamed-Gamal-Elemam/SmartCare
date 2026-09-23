using SmartCare.Core.Entities.Doctors;
using SmartCare.Core.Entities.Patients;

namespace SmartCare.Core.Entities.Appointments;

public class Appointment : BaseEntity<Guid>
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }

    public DateOnly AppointmentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public string? Reason { get; set; }
    public string? CancelReason { get; set; }

    // Total cost/fee snapshot at time of booking
    public decimal ConsultationFee { get; set; }

    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Doctor Doctor { get; set; } = null!;
}