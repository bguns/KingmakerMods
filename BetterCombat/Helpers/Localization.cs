using Kingmaker.Blueprints.Facts;
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
        internal static bool ChangeStringForLocalizationPack(string key, string value, LocalizationPack pack)
        {
            string text;
            pack.Strings.TryGetValue(key, out text);
            if (text == null)
            {
                Main.Logger?.Error($"Localization.ChangeStringForLocalizationPack: text for key `{key}` not found.");
                return false;
            }

            pack.Strings[key] = value;
            return true;
        }

        internal static bool AddStringToLocalizationPack(string key, string value, LocalizationPack pack)
        {
            if (key == null || value == null)
                return false;
            return value.Equals(pack.Strings.PutIfAbsent(key, value));
        }

        internal static LocalizedString CreateString(string key, string value)
        {
            // Sanity checks
            var strings = LocalizationManager.CurrentPack.Strings;
            String oldValue;
            if (!strings.TryGetValue(key, out oldValue)) 
                Main.Logger?.Write($"Localization.CreateString: localization key `{key}` not present in current pack.");
            if (value != null && value != oldValue)
                Main.Logger?.Write($"Localization.CreateString: current pack already has a different value for key `{key}`: `{oldValue}`");

            var localized = new LocalizedString();
            localizedString_m_Key(localized, key);
            return localized;
        }

        static FastSetter localizedString_m_Key = Harmony.CreateFieldSetter<LocalizedString>("m_Key");

        #region Extension methods

        

        #endregion
    }
}
