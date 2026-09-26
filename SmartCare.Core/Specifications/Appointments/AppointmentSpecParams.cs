using SmartCare.Core.Entities.Appointments;


namespace SmartCare.Core.Specifications.Appointments
{
    public class AppointmentSpecParams
    {
        public Guid? PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public DateOnly? Date { get; set; }
        public AppointmentStatus? Status { get; set; }

        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1;

        private int _pageSize = 12;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
