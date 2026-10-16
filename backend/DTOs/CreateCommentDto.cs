using System.ComponentModel.DataAnnotations;

namespace TaskMaster.DTOs;

public class CreateCommentDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(4000)]
    public string Content { get; set; } = string.Empty;
}
