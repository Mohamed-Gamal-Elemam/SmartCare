using SmartCare.Core.Entities.Appointments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.DTOs.Appointments
{
    public class AppointmentDetailsDto
    {
        public Guid Id { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public AppointmentStatus Status { get; set; }

        public string? Reason { get; set; }

        public string? CancelReason { get; set; }

        public decimal ConsultationFee { get; set; }

        public Guid PatientId { get; set; }

        public string PatientName { get; set; } = null!;

        public Guid DoctorId { get; set; }

        public string DoctorName { get; set; } = null!;

        public string SpecialtyName { get; set; } = null!;

        public Guid? ClinicId { get; set; }

        public string? ClinicName { get; set; }
    }
}
