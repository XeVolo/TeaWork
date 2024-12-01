using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TeaWork.Logic.Services
{
    public class LanguageService
    {

        public LanguageService()
        {
        }
        public string GetName()
        {
            return CookieRequestCultureProvider.DefaultCookieName;
        }
        public string GetValue(string culture) 
        {
            var cultureInfo = new CultureInfo(culture);
            var requestCulture = new RequestCulture(cultureInfo);
            return CookieRequestCultureProvider.MakeCookieValue(requestCulture);
        }
    }
}
