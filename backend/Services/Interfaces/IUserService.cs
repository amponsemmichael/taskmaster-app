using TaskMaster.DTOs.Users;

namespace TaskMaster.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<bool> UserExistsAsync(Guid id);
}