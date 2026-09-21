using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs;

namespace PayRollApi.IntegrationTests;

// "No cookie" is a normal case (the SPA hits refresh on every load) — must be a clean 401, not an exception.
public class AuthRefreshTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task Refresh_WithoutCookie_Returns401WithSessionMessage()
    {
        var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost"), HandleCookies = false });

        var response = await client.PostAsync("/api/auth/refresh", null);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<LoggedUser>>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        body!.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        body.Object.Should().BeNull();
        body.Message.Should().NotBe("An error occurred.");
    }

    [Fact]
    public async Task Refresh_WithUnknownCookie_Returns401AndClearsCookie()
    {
        var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost"), HandleCookies = false });
        client.SetRefreshCookie("not-a-real-token");

        var response = await client.PostAsync("/api/auth/refresh", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        // A dead cookie is expired so the browser stops re-sending it on every page load.
        response.Headers.TryGetValues("Set-Cookie", out var setCookies).Should().BeTrue();
        setCookies!.Should().Contain(c => c.StartsWith("refreshToken=") && c.Contains("expires=Thu, 01 Jan 1970"));
    }
}
