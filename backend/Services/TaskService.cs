using TaskMaster.DTOs;
using TaskMaster.DTOs.Tasks;
using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IActivityLogService _activityLogService;
    private readonly IUserService _userService;

    public TaskService(
        ITaskRepository taskRepository,
        IActivityLogService activityLogService,
        IUserService userService)
    {
        _taskRepository = taskRepository;
        _activityLogService = activityLogService;
        _userService = userService;
    }

    public async Task<TaskItem> CreateAsync(CreateTaskDto dto, Guid userId)
    {
        if (!IsValidPriority(dto.Priority))
            throw new ArgumentException("Invalid priority. Must be LOW, MEDIUM, or HIGH");

        if (!IsValidStatus(dto.Status))
            throw new ArgumentException("Invalid status. Must be PENDING, IN_PROGRESS, or COMPLETED");

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Priority = dto.Priority.ToUpper(),
            Status = dto.Status.ToUpper(),
            CreatedByUserId = userId
        };

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

    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid? userId)
    {
        return await _taskRepository.GetByIdAsync(id, userId);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(Guid? userId)
    {
        return await _taskRepository.GetAllAsync(userId);
    }

    public async Task<TaskItem> UpdateAsync(
        Guid id,
        UpdateTaskDto dto,
        Guid userId,
        bool isAdmin)
    {
        var task = await _taskRepository.GetByIdAsync(id, isAdmin ? null : userId)
            ?? throw new KeyNotFoundException($"Task with ID {id} not found");
        EnsureCreatorOrAdmin(task, userId, isAdmin);

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

    public async Task DeleteAsync(Guid id, Guid userId, bool isAdmin)
    {
        var task = await _taskRepository.GetByIdAsync(id, isAdmin ? null : userId)
            ?? throw new KeyNotFoundException($"Task with ID {id} not found");
        EnsureCreatorOrAdmin(task, userId, isAdmin);

        await _activityLogService.LogAsync(new CreateActivityLogDto
        {
            TaskItemId = task.Id,
            UserId = userId,
            Action = "DELETED",
            Description = $"Task '{task.Title}' was deleted"
        });

        await _taskRepository.DeleteAsync(task);
    }

    public async Task AssignAsync(
        Guid taskId,
        Guid assignedUserId,
        Guid assignedByUserId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId)
            ?? throw new KeyNotFoundException($"Task with ID {taskId} not found");

        if (!await _userService.UserExistsAsync(assignedUserId))
            throw new KeyNotFoundException($"User with ID {assignedUserId} not found");

        var previousAssignee = task.AssignedToUserId;
        task.AssignedToUserId = assignedUserId;
        await _taskRepository.UpdateAsync(task);

        await _activityLogService.LogAsync(new CreateActivityLogDto
        {
            TaskItemId = taskId,
            UserId = assignedByUserId,
            Action = "ASSIGNED",
            Description = previousAssignee.HasValue
                ? $"Task reassigned to user {assignedUserId}"
                : $"Task assigned to user {assignedUserId}"
        });
    }

    public async Task<IEnumerable<TaskItem>> FilterAsync(
        string? status,
        string? priority,
        DateTime? dueBefore,
        int page,
        int pageSize,
        Guid? userId)
    {
        return await _taskRepository.FilterAsync(
            status, priority, dueBefore, page, pageSize, userId);
    }

    public async Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, Guid? userId)
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

    private static void EnsureCreatorOrAdmin(TaskItem task, Guid userId, bool isAdmin)
    {
        if (!isAdmin && task.CreatedByUserId != userId)
            throw new KeyNotFoundException($"Task with ID {task.Id} not found");
    }
}