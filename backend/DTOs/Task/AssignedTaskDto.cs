using System.ComponentModel.DataAnnotations;

namespace TaskMaster.DTOs.Tasks;

public class AssignedTaskDto
{
    [Required]
    public Guid UserId { get; set; }
}
