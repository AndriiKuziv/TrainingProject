using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using TrainingProject.Api.Tests.Integration.Helpers;
using TrainingProject.Application.Dtos;
using TrainingProject.Application.Requests;

namespace TrainingProject.Api.Tests.Integration.Controllers;

public class UsersControllerTests : IClassFixture<ApiTestFixture>
{
    private readonly HttpClient _client;

    public UsersControllerTests(ApiTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task GET_GetAllUsers_200OkReturnsAllExistingUsers()
    {
        // Arrange
        var firstUser = new CreateUserRequest() { Name = "User1" };
        var secondUser = new CreateUserRequest() { Name = "User2" };

        var firstId = await CreateUserAsync(firstUser);
        var secondId = await CreateUserAsync(secondUser);

        try
        {
            // Act
            var response = await _client.GetAsync("/Users/all");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var payload = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
            Assert.Contains(payload!, dto => dto.Id == firstId && dto.Name == firstUser.Name);
            Assert.Contains(payload!, dto => dto.Id == secondId && dto.Name == secondUser.Name);
        }
        finally
        {
            await DeleteUserIfExists(firstId);
            await DeleteUserIfExists(secondId);
        }
    }

    [Fact]
    public async Task GET_GetUserById_ValidRequest_200OkReturnsExistingUser()
    {
        // Arrange
        var user = new CreateUserRequest() { Name = "User1" };
        var userId = await CreateUserAsync(user);

        try
        {
            // Act
            var response = await _client.GetAsync($"/Users/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var payload = await response.Content.ReadFromJsonAsync<UserDto>();
            Assert.Equal(userId, payload!.Id);
            Assert.Equal(user.Name, payload.Name);
        }
        finally
        {
            await DeleteUserIfExists(userId);
        }
    }

    [Fact]
    public async Task GET_GetUser_MissingUser_404NotFoundReturnsProblemDetails()
    {
        // Arrange
        var missingUserId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.GetAsync($"/Users/{missingUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Not Found", problem?.Title);
        Assert.Equal((int)HttpStatusCode.NotFound, problem?.Status);
        Assert.Null(problem?.Detail);
    }

    [Fact]
    public async Task POST_CreateUser_ValidRequest_200OkReturnsCreatedAndLocationHeader()
    {
        // Arrange
        var request = new CreateUserRequest { Name = "User1" };
        CreatedUserPayload? createdPayload = null;

        try
        {
            // Act
            var response = await _client.PostAsJsonAsync("/Users", request);
            createdPayload = await response.Content.ReadFromJsonAsync<CreatedUserPayload>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            Assert.NotNull(createdPayload);
            Assert.False(string.IsNullOrWhiteSpace(createdPayload!.Id));
        }
        finally
        {
            if (createdPayload is not null)
            {
                await DeleteUserIfExists(createdPayload.Id);
            }
        }
    }

    [Fact]
    public async Task POST_CreateUser_InvalidRequest_400BadRequestReturnsProblemDetails()
    {
        // Arrange
        var invalidRequest = new CreateUserRequest { Name = string.Empty };

        // Act
        var response = await _client.PostAsJsonAsync("/Users", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Validation Failed", problem?.Title);
        Assert.Equal((int)HttpStatusCode.BadRequest, problem?.Status);
        Assert.False(string.IsNullOrWhiteSpace(problem?.Detail));
    }

    [Fact]
    public async Task PUT_UpdateUser_ValidRequest_200OkUpdatesExistingUser()
    {
        // Arrange
        var user = new CreateUserRequest() { Name = "OrigUser" };
        var userId = await CreateUserAsync(user);
        var updateRequest = new UpdateUserRequest { Name = "UpdatedUser" };

        try
        {
            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/Users/{userId}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

            var updatedUser = await _client.GetFromJsonAsync<UserDto>($"/Users/{userId}");
            Assert.NotNull(updatedUser);
            Assert.Equal(updateRequest.Name, updatedUser.Name);
        }
        finally
        {
            await DeleteUserIfExists(userId);
        }
    }

    [Fact]
    public async Task PUT_UpdateUser_InvalidRequest_400BadRequestReturnsProblemDetails()
    {
        // Arrange
        var user = new CreateUserRequest() { Name = "OrigUser" };
        var userId = await CreateUserAsync(user);
        var invalidRequest = new UpdateUserRequest { Name = string.Empty };

        // Act
        var response = await _client.PutAsJsonAsync($"/Users/{userId}", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Validation Failed", problem?.Title);
        Assert.Equal((int)HttpStatusCode.BadRequest, problem?.Status);
        Assert.False(string.IsNullOrWhiteSpace(problem?.Detail));
    }

    [Fact]
    public async Task DELETE_DeleteUser_ValidRequest_200OkDeletesExistingUser()
    {
        // Arrange
        var user = new CreateUserRequest() { Name = "User1" };
        var userId = await CreateUserAsync(user);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/Users/{userId}");

        var getResponse = await _client.GetAsync($"/Users/{userId}");
        var problem = await getResponse.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal("Not Found", problem.Title);
    }

    [Fact]
    public async Task DELETE_DeleteUser_MissingUser_404NotFoundReturnsProblemDetails()
    {
        // Arrange
        var missingUserId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync($"/Users/{missingUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Not Found", problem?.Title);
        Assert.Equal((int)HttpStatusCode.NotFound, problem?.Status);
        Assert.Null(problem?.Detail);
    }

    private async Task<string> CreateUserAsync(CreateUserRequest createUserRequest)
    {
        var response = await _client.PostAsJsonAsync("/Users", createUserRequest);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<CreatedUserPayload>();

        return payload!.Id;
    }

    private async Task DeleteUserIfExists(string userId)
    {
        await _client.DeleteAsync($"/Users/{userId}");
    }

    private record CreatedUserPayload(string Id);
}
