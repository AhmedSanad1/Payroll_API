using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Application.Services
{
    public class Localizer(ILangManager langManager) : ILocalizer
    {
        public string Get(string key) => Languages.ResourceManager.GetString(key,new System.Globalization.CultureInfo(langManager.GetLang())) ?? key;
    }
}
