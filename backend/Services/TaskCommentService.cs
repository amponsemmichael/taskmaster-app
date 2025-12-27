
using TaskMaster.DTOs;
using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;

namespace TaskMaster.Services;
public class TaskCommentService
{
    private readonly ITaskCommentRepository _commentRepo;

    public TaskCommentService(ITaskCommentRepository commentRepo)
    {
        _commentRepo = commentRepo;
    }

    public async Task AddCommentAsync(Guid taskId, Guid userId, CreateCommentDto dto)
    {
        var comment = new TaskComment
        {
            Id = Guid.NewGuid(),
            TaskItemId = taskId,
            UserId = userId,
            Content = dto.Content
        };

        await _commentRepo.AddAsync(comment);
    }

    public async Task<IEnumerable<CommentResponseDto>> GetCommentsAsync(Guid taskId)
    {
        var comments = await _commentRepo.GetByTaskIdAsync(taskId);

        return comments.Select(c =>
            new CommentResponseDto(c.Id, c.Content, c.UserId, c.CreatedAt)
        );
    }
}
