using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TrainingProject.Application.Requests;
using TrainingProject.Application.Services;

namespace TrainingProject.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserRequest> _createUserValidator;
    private readonly IValidator<UpdateUserRequest> _updateUserValidator;

    public UsersController(
        IUserService userService,
        IValidator<CreateUserRequest> createUserValidator,
        IValidator<UpdateUserRequest> updateUserValidator)
    {
        _userService = userService;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetAllUsersAsync(cancellationToken));
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(
        [FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound($"User with ID {userId} not found");
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _createUserValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var userId = await _userService.CreateUserAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetUserById), new { userId }, new { id = userId });
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] string userId,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _updateUserValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var success = await _userService.UpdateUserAsync(userId, request, cancellationToken);
        if (!success)
        {
            return BadRequest($"Failed to update the user with ID {userId}");
        }

        return Ok();
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        var success = await _userService.DeleteUserAsync(userId, cancellationToken);
        if (!success)
        {
            return BadRequest($"Failed to delete the user with ID {userId}");
        }

        return Ok();
    }
}
