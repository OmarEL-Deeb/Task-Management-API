using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.DTOs;
using TaskManagementSystem.Application.Interfaces.Repositories;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Infrastructure.Repositories;

public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<TaskItem> Tasks, int TotalCount)> GetPagedAndFilteredTasksAsync(TaskParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Status) && Enum.TryParse(typeof(Domain.Enums.TaskStatus), parameters.Status, true, out var status))
        {
            query = query.Where(t => t.Status == (Domain.Enums.TaskStatus)status);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Priority) && Enum.TryParse(typeof(Domain.Enums.TaskPriority), parameters.Priority, true, out var priority))
        {
            query = query.Where(t => t.Priority == (Domain.Enums.TaskPriority)priority);
        }

        if (parameters.AssignedToId.HasValue)
        {
            query = query.Where(t => t.AssignedToId == parameters.AssignedToId.Value);
        }

        var totalCount = await query.CountAsync();

        var tasks = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return (tasks, totalCount);
    }
}