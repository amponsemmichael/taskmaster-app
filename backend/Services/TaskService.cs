using TaskMaster.DTOs;
using TaskMaster.DTOs.Tasks;
using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ActivityLogService _activityLogService;

    public TaskService(
        ITaskRepository taskRepository,
        ActivityLogService activityLogService)
    {
        _taskRepository = taskRepository;
        _activityLogService = activityLogService;
    }

    public async Task<TaskItem> CreateAsync(TaskItem task, Guid userId)
    {
        task.CreatedByUserId = userId;
        var createdTask = await _taskRepository.CreateAsync(task);

        await _activityLogService.LogAsync(new CreateActivityLogDto
        {
            TaskItemId = createdTask.Id,
            UserId = userId,
            Action = "CREATED",
            Description = $"Task '{createdTask.Title}' was created"
        });

        return createdTask;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _taskRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async Task<TaskItem> UpdateAsync(Guid id, UpdateTaskDto dto, Guid userId)
    {
        var task = await _taskRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Task with ID {id} not found");

        var changes = new List<string>();

        if (!string.IsNullOrWhiteSpace(dto.Title) && task.Title != dto.Title)
        {
            changes.Add($"Title changed from '{task.Title}' to '{dto.Title}'");
            task.Title = dto.Title;
        }

        if (dto.Description != null && task.Description != dto.Description)
        {
            changes.Add("Description updated");
            task.Description = dto.Description;
        }

        if (dto.DueDate.HasValue && task.DueDate != dto.DueDate)
        {
            changes.Add($"Due date changed to {dto.DueDate.Value:yyyy-MM-dd}");
            task.DueDate = dto.DueDate;
        }

        if (!string.IsNullOrWhiteSpace(dto.Priority) && task.Priority != dto.Priority)
        {
            if (!IsValidPriority(dto.Priority))
                throw new ArgumentException("Invalid priority. Must be LOW, MEDIUM, or HIGH");

            changes.Add($"Priority changed from {task.Priority} to {dto.Priority}");
            task.Priority = dto.Priority;
        }

        if (!string.IsNullOrWhiteSpace(dto.Status) && task.Status != dto.Status)
        {
            if (!IsValidStatus(dto.Status))
                throw new ArgumentException("Invalid status. Must be PENDING, IN_PROGRESS, or COMPLETED");

            changes.Add($"Status changed from {task.Status} to {dto.Status}");
            task.Status = dto.Status;
        }

        if (changes.Any())
        {
            await _taskRepository.UpdateAsync(task);

            await _activityLogService.LogAsync(new CreateActivityLogDto
            {
                TaskItemId = task.Id,
                UserId = userId,
                Action = "UPDATED",
                Description = string.Join(", ", changes)
            });
        }

        return task;
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var task = await _taskRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Task with ID {id} not found");

        await _activityLogService.LogAsync(new CreateActivityLogDto
        {
            TaskItemId = task.Id,
            UserId = userId,
            Action = "DELETED",
            Description = $"Task '{task.Title}' was deleted"
        });

        await _taskRepository.DeleteAsync(task);
    }

    public async Task AssignAsync(Guid taskId, Guid userId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId)
            ?? throw new KeyNotFoundException($"Task with ID {taskId} not found");

        var previousAssignee = task.AssignedToUserId;
        task.AssignedToUserId = userId;
        await _taskRepository.UpdateAsync(task);

        await _activityLogService.LogAsync(new CreateActivityLogDto
        {
            TaskItemId = taskId,
            UserId = userId,
            Action = "ASSIGNED",
            Description = previousAssignee.HasValue
                ? $"Task reassigned to user {userId}"
                : $"Task assigned to user {userId}"
        });
    }

    public async Task<IEnumerable<TaskItem>> FilterAsync(
        string? status,
        string? priority,
        DateTime? dueBefore,
        int page,
        int pageSize)
    {
        return await _taskRepository.FilterAsync(
            status, priority, dueBefore, page, pageSize);
    }

    public async Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, Guid? userId = null)
    {
        return await _taskRepository.SearchAsync(searchTerm, userId);
    }

    private static bool IsValidPriority(string priority)
    {
        return priority.ToUpper() is "LOW" or "MEDIUM" or "HIGH";
    }

    private static bool IsValidStatus(string status)
    {
        return status.ToUpper() is "PENDING" or "IN_PROGRESS" or "COMPLETED";
    }
}