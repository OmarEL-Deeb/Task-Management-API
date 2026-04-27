namespace TaskManagementSystem.Application.DTOs;

public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // ToDo, InProgress, Done
    public string Priority { get; set; } = string.Empty; // Low, Medium, High
}