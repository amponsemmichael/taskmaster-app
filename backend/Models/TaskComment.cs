using System.Text.Json.Serialization;

namespace TaskMaster.Models;

public class TaskComment
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }

    [JsonIgnore]
    public TaskItem TaskItem { get; set; } = null!;

    public Guid UserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
