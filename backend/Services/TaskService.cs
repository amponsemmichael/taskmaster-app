using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskItem> CreateAsync(TaskItem task, Guid userId)
    {
        task.CreatedByUserId = userId;
        return await _taskRepository.CreateAsync(task);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async Task AssignAsync(Guid taskId, Guid userId)
{
    var task = await _taskRepository.GetByIdAsync(taskId)
        ?? throw new Exception("Task not found");

    task.AssignedToUserId = userId;
    await _taskRepository.UpdateAsync(task);
}
}
