using SmartCare.Core.Entities.Appointments;

namespace SmartCare.Core.Specifications.Appointments;

public class AppointmentSpecParams
{
    public Guid? PatientId { get; set; }

    public Guid? DoctorId { get; set; }

    public DateOnly? Date { get; set; }

    public AppointmentStatus? Status { get; set; }

    private const int MaxPageSize = 50;

    private int _pageIndex = 1;

    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value < 1 ? 1 : value;
    }

    private int _pageSize = 12;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value <= 0)
                _pageSize = 12;
            else
                _pageSize = Math.Min(value, MaxPageSize);
        }
    }
}
