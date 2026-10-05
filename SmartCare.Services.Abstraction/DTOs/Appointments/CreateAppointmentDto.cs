using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        public Guid DoctorId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? Reason { get; set; }
    }
}
