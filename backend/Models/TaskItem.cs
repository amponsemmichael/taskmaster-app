using System.ComponentModel.DataAnnotations;

namespace TaskMaster.Models;

public class TaskItem
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    [Required]
    public string Priority { get; set; } = "MEDIUM"; 

    [Required]
    public string Status { get; set; } = "PENDING"; 

    public Guid AssignedToUserId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
