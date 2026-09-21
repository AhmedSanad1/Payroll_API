using System.Globalization;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using PayRollApi.Infrastructure.Services;

public class LangManagerTests
{
    private const string BrowserAcceptLanguage = "en-US,en;q=0.9,es;q=0.8,ar;q=0.7";

    private static LangManager CreateSut(string? acceptLanguage, string? negotiatedCulture)
    {
        var context = new DefaultHttpContext();
        if (acceptLanguage is not null)
            context.Request.Headers["Accept-Language"] = acceptLanguage;
        if (negotiatedCulture is not null)
            context.Features.Set<IRequestCultureFeature>(
                new RequestCultureFeature(new RequestCulture(negotiatedCulture), provider: null));

        return new LangManager(new HttpContextAccessor { HttpContext = context });
    }

    [Fact]
    public void GetLang_returns_the_culture_negotiated_by_request_localization_not_the_raw_header()
    {
        var sut = CreateSut(BrowserAcceptLanguage, "ar-SA");

        sut.GetLang().Should().Be("ar-SA");
    }

    [Fact]
    public void GetLang_always_returns_a_valid_culture_name_for_a_weighted_browser_header()
    {
        var sut = CreateSut(BrowserAcceptLanguage, negotiatedCulture: null);

        var act = () => new CultureInfo(sut.GetLang());

        act.Should().NotThrow<CultureNotFoundException>();
    }

    [Fact]
    public void GetLang_falls_back_to_current_ui_culture_when_there_is_no_http_context()
    {
        var sut = new LangManager(new HttpContextAccessor());

        sut.GetLang().Should().Be(CultureInfo.CurrentUICulture.Name);
    }
}
