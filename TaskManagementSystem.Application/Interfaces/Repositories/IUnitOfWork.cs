namespace TaskManagementSystem.Application.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    ITaskRepository Tasks { get; }
    IUserRepository Users { get; }
  

    Task<int> CompleteAsync();
}