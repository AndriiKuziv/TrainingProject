using AutoMapper;
using TrainingProject.Application.Dtos;
using TrainingProject.Application.Requests;
using TrainingProject.Domain.Models;
using TrainingProject.Domain.Repositories;

namespace TrainingProject.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly IValidationService _validationService;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(
        IValidationService validationService,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _validationService = validationService;
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

    public async Task<CreateUserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        await _validationService.ValidateAsync(request, cancellationToken);

        var user = _mapper.Map<User>(request);
        var createUser = await _userRepository.CreateAsync(user, cancellationToken);

        return _mapper.Map<CreateUserDto>(createUser);
    }

    public async Task<UpdateUserDto> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        await _validationService.ValidateAsync(request, cancellationToken);

        var existingUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        var updatedUser = _mapper.Map<User>(request);

        var resultUser = await _userRepository.UpdateAsync(userId, updatedUser, cancellationToken);

        return _mapper.Map<UpdateUserDto>(resultUser);
    }

    public async Task<DeleteUserDto> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var deletedUser = await _userRepository.DeleteAsync(userId, cancellationToken);

        return _mapper.Map<DeleteUserDto>(deletedUser);
    }
}
