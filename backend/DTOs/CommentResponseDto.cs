public record CommentResponseDto(
    Guid Id,
    string Content,
    Guid UserId,
    DateTime CreatedAt
);
