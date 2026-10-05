using SmartCare.Core.Entities.Appointments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.DTOs.Appointments
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Guid DoctorId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public AppointmentStatus Status { get; set; }

        public string? Reason { get; set; }

        public string? CancelReason { get; set; }

        public decimal ConsultationFee { get; set; }
    }
}
