using System.Net.Http.Headers;
using System.Net.Http.Json;
using PayRollApi.Application.Common;

namespace PayRollApi.IntegrationTests;

internal static class TestClientExtensions
{
    public static async Task<(System.Net.HttpStatusCode Status, ApiResponse<T>? Body)> GetJsonAsync<T>(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return (response.StatusCode, body);
    }

    public static async Task<(System.Net.HttpStatusCode Status, ApiResponse<T>? Body)> PostJsonAsync<T>(this HttpClient client, string url, object payload)
    {
        var response = await client.PostAsJsonAsync(url, payload);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return (response.StatusCode, body);
    }

    // Same as PostJsonAsync but also returns the raw response, for when you need the headers too.
    public static async Task<(HttpResponseMessage Response, ApiResponse<T>? Body)> PostAsJsonAsyncTracked<T>(this HttpClient client, string url, object payload)
    {
        var response = await client.PostAsJsonAsync(url, payload);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return (response, body);
    }

    public static async Task<(System.Net.HttpStatusCode Status, ApiResponse<T>? Body)> PutJsonAsync<T>(this HttpClient client, string url, object payload)
    {
        var response = await client.PutAsJsonAsync(url, payload);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();
        return (response.StatusCode, body);
    }

    public static void SetBearerToken(this HttpClient client, string accessToken) =>
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

    public static void SetRefreshCookie(this HttpClient client, string rawRefreshToken)
    {
        client.DefaultRequestHeaders.Remove("Cookie");
        client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={rawRefreshToken}");
    }

    // The refresh cookie only ever comes back as a Set-Cookie header — extract the raw value from it.
    public static string ExtractCookieValue(this HttpResponseMessage response, string cookieName)
    {
        var setCookie = response.Headers.GetValues("Set-Cookie").First(c => c.StartsWith(cookieName + "="));
        var firstSegment = setCookie.Split(';')[0];
        return firstSegment[(cookieName.Length + 1)..];
    }
}
