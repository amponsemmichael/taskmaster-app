using TaskMaster.Models;

namespace TaskMaster.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<bool> UserExistsByEmailAsync(string email);
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task<RefreshToken?> GetRefreshTokenWithUserAsync(string tokenHash);
    Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task RotateRefreshTokenAsync(RefreshToken currentToken, RefreshToken replacement);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
}
