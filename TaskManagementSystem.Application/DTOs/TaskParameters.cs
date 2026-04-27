namespace TaskManagementSystem.Application.DTOs;

public class TaskParameters
{
    private const int maxPageSize = 50;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
    }

    // خصائص الفلترة
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public int? AssignedToId { get; set; }
}