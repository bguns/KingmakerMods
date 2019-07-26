using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Helpers
{
    static class Localization
    {
        internal static bool ChangeStringForLocale(string key, string value, Locale locale)
        {
            if (!LocalizationManager.CurrentLocale.Equals(locale))
            {
                Main.Logger?.Write($"ChangeStringForLocale: current locale is not {locale.ToString()}");
                return false;
            }

            string text;
            LocalizationManager.CurrentPack.Strings.TryGetValue(key, out text);
            if (text == null)
            {
                Main.Logger?.Error($"ChangeStringForLocale: text for key {key} not found.");
                return false;
            }

            LocalizationManager.CurrentPack.Strings[key] = value;
            return true;
        }

        internal static bool AddStringToLocalizationPack(string key, string value, LocalizationPack pack)
        {
            if (key == null || value == null)
                return false;
            return value.Equals(pack.Strings.PutIfAbsent(key, value));
        }
    }
}
