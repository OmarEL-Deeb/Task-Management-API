using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Application.DTOs;

public class CreateTaskDto
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Priority must be Low, Medium, or High")]
    public string Priority { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    public int? AssignedToId { get; set; }
}