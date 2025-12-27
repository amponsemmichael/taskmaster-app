using Microsoft.EntityFrameworkCore;
using TaskMaster.Data;
using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;

namespace TaskMaster.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskItem>> FilterAsync(
    string? status,
    string? priority,
    DateTime? dueBefore,
    int page,
    int pageSize)
{
    var query = _context.Tasks.AsQueryable();

    if (!string.IsNullOrEmpty(status))
        query = query.Where(t => t.Status == status);

    if (!string.IsNullOrEmpty(priority))
        query = query.Where(t => t.Priority == priority);

    if (dueBefore.HasValue)
        query = query.Where(t => t.DueDate <= dueBefore);

    return await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
}