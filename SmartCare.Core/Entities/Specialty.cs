namespace SmartCare.Core.Entities;

public class Specialty : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;


}