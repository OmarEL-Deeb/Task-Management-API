using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Interfaces.Repositories;

public interface ITaskRepository : IGenericRepository<TaskItem>
{
    Task<(IEnumerable<TaskItem> Tasks, int TotalCount)> GetPagedAndFilteredTasksAsync(TaskParameters parameters);
}