using TaskMaster.DTOs.Users;
using TaskMaster.Repositories.Interfaces;
using TaskMaster.Services.Interfaces;

namespace TaskMaster.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<bool> UserExistsAsync(Guid id)
    {
        return await _userRepository.ExistsAsync(id);
    }
}
