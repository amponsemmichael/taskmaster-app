using System.ComponentModel.DataAnnotations;

namespace TaskMaster.DTOs.Tasks;

public class UpdateTaskDto
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [RegularExpression("(?i)^(LOW|MEDIUM|HIGH)$")]
    public string? Priority { get; set; }

    [RegularExpression("(?i)^(PENDING|IN_PROGRESS|COMPLETED)$")]
    public string? Status { get; set; }
}