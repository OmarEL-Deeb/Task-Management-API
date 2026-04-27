using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.Interfaces.Repositories;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        // بنستخدم SingleOrDefaultAsync عشان نتأكد إن الإيميل مش متكرر
        return await _dbSet.SingleOrDefaultAsync(u => u.Email == email);
    }
}