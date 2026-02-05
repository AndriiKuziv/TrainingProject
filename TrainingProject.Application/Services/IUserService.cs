using TrainingProject.Application.Dtos;
using TrainingProject.Application.Requests;

namespace TrainingProject.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken);

    Task<UserDto> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

    Task<string> CreateUserAsync(CreateUserRequest userDto, CancellationToken cancellationToken);

    Task<bool> UpdateUserAsync(string userId, UpdateUserRequest userDto, CancellationToken cancellationToken);

    Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken);
}
