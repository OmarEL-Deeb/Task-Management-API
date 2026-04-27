using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}