using Microsoft.EntityFrameworkCore;
using TaskMaster.Data;
using TaskMaster.Models;
using TaskMaster.Repositories.Interfaces;

namespace TaskMaster.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> UserExistsByEmailAsync(string email)
    {
        return _context.Users.AnyAsync(user => user.Email == email);
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public Task<RefreshToken?> GetRefreshTokenWithUserAsync(string tokenHash)
    {
        return _context.RefreshTokens
            .Include(token => token.User)
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash);
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash)
    {
        return _context.RefreshTokens
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash);
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RotateRefreshTokenAsync(
        RefreshToken currentToken,
        RefreshToken replacement)
    {
        currentToken.IsRevoked = true;
        _context.RefreshTokens.Add(replacement);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
    {
        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();
    }
}
