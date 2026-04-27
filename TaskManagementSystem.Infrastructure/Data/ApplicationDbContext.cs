using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // هنا بنحل مشكلة الـ Multiple Cascade Paths في SQL Server

        // 1. علاقة المستخدم بالمهمة كـ (AssignedTo)
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict); // منع الحذف التلقائي

        // 2. علاقة المستخدم بالمهمة كـ (CreatedBy)
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // إعدادات إضافية
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique(); // الإيميل لازم يكون غير مكرر

        modelBuilder.Entity<TaskItem>()
            .Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
    }
}