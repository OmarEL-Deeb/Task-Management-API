using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskManagementSystem.Domain.Enums.TaskStatus Status { get; set; } = TaskManagementSystem.Domain.Enums.TaskStatus.ToDo;
    public TaskPriority Priority { get; set; }
    public DateTime DueDate { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
}