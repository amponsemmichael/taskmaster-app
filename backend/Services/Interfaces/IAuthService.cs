using TaskMaster.DTOs.Auth;

namespace TaskMaster.Services;

public interface IAuthService
{
    Task<AuthTokensDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthTokensDto> LoginAsync(LoginRequestDto request);
    Task<AuthTokensDto> RefreshAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
}
