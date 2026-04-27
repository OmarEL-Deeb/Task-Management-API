using TaskManagementSystem.Application.Interfaces.Repositories;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IUserRepository Users { get; private set; }
    public ITaskRepository Tasks { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Tasks = new TaskRepository(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}