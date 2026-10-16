using TaskMaster.DTOs;

namespace TaskMaster.Services.Interfaces;

public interface ITaskCommentService
{
    Task AddCommentAsync(Guid taskId, Guid userId, bool isAdmin, CreateCommentDto dto);
    Task<IEnumerable<CommentResponseDto>> GetCommentsAsync(Guid taskId, Guid userId, bool isAdmin);
}
