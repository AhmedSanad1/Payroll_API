using FluentAssertions;
using PayRollApi.Application.Common;

public class ApiResponseTests
{
    [Fact]
    public void SuccessResponse_sets_OK_status_code()
    {
        var response = new SuccessResponse<string>("done", "payload");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Object.Should().Be("payload");
    }

    [Fact]
    public void FailResponse_defaults_to_BadRequest_status_code()
    {
        var response = new FailResponse<string>("nope");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
