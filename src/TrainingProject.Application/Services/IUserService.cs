using TrainingProject.Application.Dtos;
using TrainingProject.Application.Requests;

namespace TrainingProject.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken);

    Task<UserDto> GetUserByIdAsync(string userId, CancellationToken cancellationToken);

    Task<CreateUserDto> CreateUserAsync(CreateUserRequest userDto, CancellationToken cancellationToken);

    Task<UpdateUserDto> UpdateUserAsync(string userId, UpdateUserRequest userDto, CancellationToken cancellationToken);

    Task<DeleteUserDto> DeleteUserAsync(string userId, CancellationToken cancellationToken);
}
