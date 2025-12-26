using System.ComponentModel.DataAnnotations;

namespace TaskMaster.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "USER";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}   