using TaskManagementSystem.Application.DTOs;

namespace TaskManagementSystem.Application.Interfaces.Services;

public interface ITaskService
{
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto taskDto, int currentUserId);
    Task<TaskResponseDto?> GetTaskByIdAsync(int id);
    Task<(IEnumerable<TaskResponseDto> Tasks, int TotalCount)> GetTasksAsync(TaskParameters parameters);
    Task UpdateTaskAsync(int id, UpdateTaskDto taskDto, int currentUserId, string currentUserRole);
    Task DeleteTaskAsync(int id, int currentUserId, string currentUserRole);
}