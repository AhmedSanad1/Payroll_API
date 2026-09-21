using PayRollApi.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace PayRollApi.Infrastructure.Services
{
    public class LangManager(IHttpContextAccessor accessor) : ILangManager
    {
        // Take the culture the localization middleware already picked — the raw
        // Accept-Language header is a weighted list, not a valid culture name.
        public string GetLang()
        {
            var negotiated = accessor?.HttpContext?.Features
                .Get<IRequestCultureFeature>()?.RequestCulture.UICulture;

            return (negotiated ?? CultureInfo.CurrentUICulture).Name;
        }
    }
}
