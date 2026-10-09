using System.ComponentModel.DataAnnotations;

namespace TaskMaster.DTOs.Tasks;

public class CreateTaskDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    [Required]
    [RegularExpression("(?i)^(LOW|MEDIUM|HIGH)$")]
    public string Priority { get; set; } = "MEDIUM";

    [Required]
    [RegularExpression("(?i)^(PENDING|IN_PROGRESS|COMPLETED)$")]
    public string Status { get; set; } = "PENDING";
}
