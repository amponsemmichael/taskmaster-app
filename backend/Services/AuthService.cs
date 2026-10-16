using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskMaster.DTOs.Auth;
using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;

namespace TaskMaster.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _config;

    // Refresh tokens are valid for 7 days
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    public AuthService(IAuthRepository authRepository, IConfiguration config)
    {
        _authRepository = authRepository;
        _config = config;
    }

    public async Task<AuthTokensDto> RegisterAsync(RegisterRequestDto request)
    {
        if (await _authRepository.UserExistsByEmailAsync(request.Email))
            throw new InvalidOperationException("User already exists");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _authRepository.AddUserAsync(user);

        return await IssueTokensAsync(user);
    }

    public async Task<AuthTokensDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        return await IssueTokensAsync(user);
    }

    public async Task<AuthTokensDto> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var stored = await _authRepository.GetRefreshTokenWithUserAsync(HashRefreshToken(refreshToken));

        if (stored == null || stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        // Rotate: revoke the old token and issue a new pair
        var replacement = CreateRefreshToken(stored.UserId);
        await _authRepository.RotateRefreshTokenAsync(stored, replacement.RefreshToken);
        return new AuthTokensDto
        {
            AccessToken = GenerateAccessToken(stored.User),
            RefreshToken = replacement.Token
        };
    }

    public async Task RevokeAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var stored = await _authRepository.GetRefreshTokenAsync(HashRefreshToken(refreshToken));

        if (stored == null || stored.IsRevoked)
            throw new UnauthorizedAccessException("Refresh token not found or already revoked");

        await _authRepository.RevokeRefreshTokenAsync(stored);
    }

    private async Task<AuthTokensDto> IssueTokensAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = CreateRefreshToken(user.Id);
        await _authRepository.AddRefreshTokenAsync(refreshToken.RefreshToken);

        return new AuthTokensDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }

    private static (string Token, RefreshToken RefreshToken) CreateRefreshToken(Guid userId)
    {
        var token = GenerateRefreshTokenValue();
        return (token, new RefreshToken
        {
            UserId = userId,
            TokenHash = HashRefreshToken(token),
            ExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime)
        });
    }

    private static string HashRefreshToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured.")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Jwt:Issuer is not configured."),
            audience: _config["Jwt:Audience"]
                ?? throw new InvalidOperationException("Jwt:Audience is not configured."),
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshTokenValue()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
