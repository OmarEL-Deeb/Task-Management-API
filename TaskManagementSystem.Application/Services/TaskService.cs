using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces.Repositories;
using TaskManagementSystem.Application.Interfaces.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto taskDto, int currentUserId)
    {
        if (taskDto.DueDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentException("Due date cannot be in the past.");

        if (!Enum.TryParse(taskDto.Priority, true, out TaskPriority priority))
            throw new ArgumentException("Invalid priority value.");

        var taskItem = new TaskItem
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            Priority = priority,
            DueDate = taskDto.DueDate,
            AssignedToId = taskDto.AssignedToId,
            CreatedById = currentUserId,
            Status = TaskManagementSystem.Domain.Enums.TaskStatus.ToDo
        };

        await _unitOfWork.Tasks.AddAsync(taskItem);
        await _unitOfWork.CompleteAsync();

        return MapToDto(taskItem);
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(int id)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id);
        return task == null ? null : MapToDto(task);
    }

    public async Task<(IEnumerable<TaskResponseDto> Tasks, int TotalCount)> GetTasksAsync(TaskParameters parameters)
    {
        var result = await _unitOfWork.Tasks.GetPagedAndFilteredTasksAsync(parameters);
        var dtos = result.Tasks.Select(MapToDto).ToList();
        return (dtos, result.TotalCount);
    }

    public async Task UpdateTaskAsync(int id, UpdateTaskDto taskDto, int currentUserId, string currentUserRole)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id);
        if (task == null) throw new KeyNotFoundException("Task not found.");

        // Business Rule: A user can only update tasks assigned to them (unless Admin/Manager)
        if (currentUserRole == UserRole.User.ToString() && task.AssignedToId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only update tasks assigned to you.");
        }

        if (!Enum.TryParse(taskDto.Status, true, out TaskManagementSystem.Domain.Enums.TaskStatus newStatus))
            throw new ArgumentException("Invalid status value.");

        // Business Rule: Task cannot be marked as "Done" before being "InProgress"
        if (newStatus == TaskManagementSystem.Domain.Enums.TaskStatus.Done && task.Status != TaskManagementSystem.Domain.Enums.TaskStatus.InProgress)
        {
            throw new InvalidOperationException("Task cannot be marked as 'Done' before being 'InProgress'.");
        }

        if (!Enum.TryParse(taskDto.Priority, true, out TaskPriority newPriority))
            throw new ArgumentException("Invalid priority value.");

        task.Title = taskDto.Title;
        task.Description = taskDto.Description;
        task.Status = newStatus;
        task.Priority = newPriority;

        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteTaskAsync(int id, int currentUserId, string currentUserRole)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id);
        if (task == null) throw new KeyNotFoundException("Task not found.");

        // Business Rule: Only Admins can delete tasks (أو حسب ما تحدد، خليناها Admin هنا كمثال)
        if (currentUserRole != UserRole.Admin.ToString())
        {
            throw new UnauthorizedAccessException("Only Admins can delete tasks.");
        }

        _unitOfWork.Tasks.Delete(task);
        await _unitOfWork.CompleteAsync();
    }

    // دالة مساعدة (Helper Method) لتقليل تكرار الكود
    private TaskResponseDto MapToDto(TaskItem task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate,
            AssignedToId = task.AssignedToId,
            CreatedById = task.CreatedById
        };
    }
}