using TaskMaster.Models;

namespace TaskMaster.Services.Interfaces;

public interface ITaskService
{
    Task<TaskItem> CreateAsync(TaskItem task, Guid userId);
    Task<IEnumerable<TaskItem>> GetAllAsync();
}
