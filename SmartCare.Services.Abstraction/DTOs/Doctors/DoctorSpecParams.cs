public class DoctorSpecParams
{
    private const int MaxPageSize = 50;
    public const int DefaultPageSize = 12;

    private int _pageSize = DefaultPageSize;
    private int _pageIndex = 1;

    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value < 1 ? 1 : value; 
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize
                        : (value <= 0 ? DefaultPageSize : value);
    }

    public Guid? SpecialtyId { get; set; }
    public Guid? ClinicId { get; set; }
    public decimal? MinFee { get; set; }
    public decimal? MaxFee { get; set; }
    public bool? IsActive { get; set; }
    public string? Sort { get; set; }

    private string? _search;
    public string? Search
    {
        get => _search;
        set => _search = value?.Trim();
    }
}