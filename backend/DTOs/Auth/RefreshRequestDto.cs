using System.ComponentModel.DataAnnotations;

namespace TaskMaster.DTOs.Auth;

public class RefreshRequestDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(256)]
    public string RefreshToken { get; set; } = string.Empty;
}

public class AuthTokensDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
