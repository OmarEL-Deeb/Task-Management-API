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

        // 1. تطبيق الفلترة
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

        // 2. حساب العدد الكلي قبل الـ Pagination
        var totalCount = await query.CountAsync();

        // 3. تطبيق التقسيم (Pagination)
        var tasks = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return (tasks, totalCount);
    }
}