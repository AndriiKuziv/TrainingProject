using AutoMapper;
using TrainingProject.Application.Dtos;
using TrainingProject.Application.Requests;
using TrainingProject.Domain.Models;
using TrainingProject.Domain.Repositories;

namespace TrainingProject.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<string> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = _mapper.Map<User>(request);
        var userId = await _userRepository.CreateAsync(user, cancellationToken);

        return userId;
    }

    public async Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        var updatedUser = _mapper.Map<User>(request);

        return await _userRepository.UpdateAsync(userId, updatedUser, cancellationToken);
    }

    public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _userRepository.DeleteAsync(userId, cancellationToken);
    }
}
