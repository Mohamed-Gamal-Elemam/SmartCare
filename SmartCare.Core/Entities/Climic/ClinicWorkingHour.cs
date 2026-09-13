namespace SmartCare.Core.Entities.Climic;

public class ClinicWorkingHour : BaseEntity<Guid>
{
    public Guid ClinicId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public bool IsClosed { get; set; }

    public Clinic Clinic { get; set; } = null!;
}