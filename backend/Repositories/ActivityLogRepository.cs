using Microsoft.EntityFrameworkCore;
using TaskMaster.Data;
using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;

namespace TaskMaster.Repositories;

public class ActivityLogRepository : IActivityLogRepository
{
    private readonly AppDbContext _context;

    public ActivityLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ActivityLog log)
    {
        await _context.ActivityLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ActivityLog>> GetByTaskIdAsync(Guid taskId)
    {
        return await _context.ActivityLogs
            .Where(l => l.TaskItemId == taskId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }
}
