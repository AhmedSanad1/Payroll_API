using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace PayRollApi.IntegrationTests;

// Own app instance, so the login limiter window isn't shared with other test classes.
public class RateLimitingTests : IDisposable
{
    private readonly CustomWebApplicationFactory factory = new();

    [Fact]
    public async Task Login_SixthAttemptWithinAMinute_IsRateLimited()
    {
        var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });

        for (var i = 0; i < 5; i++)
        {
            var response = await client.PostAsJsonAsync("/api/auth/login", new { Username = "nobody", Password = "wrong" });
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        var sixth = await client.PostAsJsonAsync("/api/auth/login", new { Username = "nobody", Password = "wrong" });
        sixth.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }

    public void Dispose() => factory.Dispose();
}
