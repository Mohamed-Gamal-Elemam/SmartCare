using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Entities.Appointments
{
    public enum AppointmentStatus
    {
        Pending = 1,
        Confirmed,
        Cancelled,
        Completed,
        NoShow
    }
}
