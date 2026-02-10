using Microsoft.Extensions.Configuration;

namespace TrainingProject.Api.Tests.Integration.Helpers;

public sealed class ApiTestFixture : IDisposable
{
    public HttpClient Client { get; }

    public ApiTestFixture()
    {
        var baseUrl = "https://localhost:7119";

        Client = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public void Dispose() => Client.Dispose();
}
