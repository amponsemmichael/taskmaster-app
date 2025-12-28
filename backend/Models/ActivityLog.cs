namespace TaskMaster.Models;

public class ActivityLog
{
    public Guid Id { get; set; }

    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid TaskItemId { get; set; }
    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
