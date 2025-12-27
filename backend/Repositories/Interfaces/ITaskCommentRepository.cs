public interface ITaskCommentRepository
{
    Task AddAsync(TaskComment comment);
    Task<IEnumerable<TaskComment>> GetByTaskIdAsync(Guid taskId);
}
