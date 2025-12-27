public class TaskCommentRepository : ITaskCommentRepository
{
    private readonly AppDbContext _context;

    public TaskCommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskComment comment)
    {
        _context.TaskComments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskComment>> GetByTaskIdAsync(Guid taskId)
    {
        return await _context.TaskComments
            .Where(c => c.TaskItemId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }
}
