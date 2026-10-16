using TaskMaster.DTOs;
using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly ITaskCommentRepository _commentRepo;
    private readonly IActivityLogService _activityLogService;
    private readonly ITaskRepository _taskRepository;

    public TaskCommentService(
        ITaskCommentRepository commentRepo,
        IActivityLogService activityLogService,
        ITaskRepository taskRepository)
    {
        _commentRepo = commentRepo;
        _activityLogService = activityLogService;
        _taskRepository = taskRepository;
    }

    public async Task AddCommentAsync(
        Guid taskId,
        Guid userId,
        bool isAdmin,
        CreateCommentDto dto)
    {
        await EnsureCanAccessTaskAsync(taskId, userId, isAdmin);
        var comment = new TaskComment
        {
            Id = Guid.NewGuid(),
            TaskItemId = taskId,
            UserId = userId,
            Content = dto.Content
        };

        await _commentRepo.AddAsync(comment);

        await _activityLogService.LogAsync(new CreateActivityLogDto
        {
            TaskItemId = taskId,
            UserId = userId,
            Action = "COMMENTED",
            Description = "A comment was added to the task"
        });
    }

    public async Task<IEnumerable<CommentResponseDto>> GetCommentsAsync(
        Guid taskId,
        Guid userId,
        bool isAdmin)
    {
        await EnsureCanAccessTaskAsync(taskId, userId, isAdmin);
        var comments = await _commentRepo.GetByTaskIdAsync(taskId);

        return comments.Select(c =>
            new CommentResponseDto(c.Id, c.Content, c.UserId, c.CreatedAt)
        );
    }

    private async Task EnsureCanAccessTaskAsync(Guid taskId, Guid userId, bool isAdmin)
    {
        if (!isAdmin && !await _taskRepository.UserCanAccessTaskAsync(taskId, userId))
            throw new KeyNotFoundException($"Task with ID {taskId} not found");

        if (isAdmin && await _taskRepository.GetByIdAsync(taskId) is null)
            throw new KeyNotFoundException($"Task with ID {taskId} not found");
    }
}
