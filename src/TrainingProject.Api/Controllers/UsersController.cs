using Microsoft.AspNetCore.Mvc;
using TrainingProject.Application.Requests;
using TrainingProject.Application.Services;

namespace TrainingProject.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
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

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var createdUser = await _userService.CreateUserAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] string userId,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var updatedUser = await _userService.UpdateUserAsync(userId, request, cancellationToken);

        return Ok(updatedUser);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        await _userService.DeleteUserAsync(userId, cancellationToken);

        return NoContent();
    }
}
