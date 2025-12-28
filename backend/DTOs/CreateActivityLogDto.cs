namespace TaskMaster.DTOs;

public class CreateActivityLogDto
{
    public Guid TaskItemId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
