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

    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid? accessibleToUserId = null)
    {
        var query = _context.Tasks.AsQueryable();
        if (accessibleToUserId.HasValue)
        {
            var userId = accessibleToUserId.Value;
            query = query.Where(t =>
                t.CreatedByUserId == userId || t.AssignedToUserId == userId);
        }

        return await query.Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(Guid? accessibleToUserId = null)
    {
        var query = _context.Tasks.AsQueryable();
        if (accessibleToUserId.HasValue)
        {
            var userId = accessibleToUserId.Value;
            query = query.Where(t =>
                t.CreatedByUserId == userId || t.AssignedToUserId == userId);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
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
        int pageSize,
        Guid? accessibleToUserId = null)
    {
        var query = _context.Tasks.AsQueryable();

        if (accessibleToUserId.HasValue)
        {
            var userId = accessibleToUserId.Value;
            query = query.Where(t =>
                t.CreatedByUserId == userId || t.AssignedToUserId == userId);
        }

        if (!string.IsNullOrEmpty(status))
            query = query.Where(t => t.Status == status.ToUpper());

        if (!string.IsNullOrEmpty(priority))
            query = query.Where(t => t.Priority == priority.ToUpper());

        if (dueBefore.HasValue)
            query = query.Where(t => t.DueDate <= dueBefore);

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<bool> UserCanAccessTaskAsync(Guid taskId, Guid userId)
    {
        return _context.Tasks.AnyAsync(task =>
            task.Id == taskId &&
            (task.CreatedByUserId == userId || task.AssignedToUserId == userId));
    }

    public Task<List<TaskItem>> GetTasksDueSoonAsync(DateTime now, DateTime tomorrow)
    {
        return _context.Tasks
            .Where(task => task.DueDate.HasValue &&
                           task.DueDate.Value > now &&
                           task.DueDate.Value <= tomorrow &&
                           task.Status != "COMPLETED" &&
                           task.AssignedToUserId != null)
            .ToListAsync();
    }

    public Task<List<TaskItem>> GetOverdueTasksAsync(DateTime now)
    {
        return _context.Tasks
            .Where(task => task.DueDate.HasValue &&
                           task.DueDate.Value < now &&
                           task.Status != "COMPLETED" &&
                           task.AssignedToUserId != null)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, Guid? userId = null)
    {
        var query = _context.Tasks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerTerm = searchTerm.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(lowerTerm) ||
                t.Description.ToLower().Contains(lowerTerm));
        }

        if (userId.HasValue)
        {
            query = query.Where(t =>
                t.CreatedByUserId == userId ||
                t.AssignedToUserId == userId);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}