using TaskMaster.Models;

namespace TaskMaster.Repositories.Interfaces;
public interface ITaskCommentRepository
{
    Task AddAsync(TaskComment comment);
    Task<IEnumerable<TaskComment>> GetByTaskIdAsync(Guid taskId);
}
