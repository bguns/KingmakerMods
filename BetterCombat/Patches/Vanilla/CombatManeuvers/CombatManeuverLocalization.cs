using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{


    [Harmony12.HarmonyPatch(typeof(LocalizationManager), "LoadPack", Harmony12.MethodType.Normal)]
    class LocalizationManager_FixCombatManeuverFeatText_Patch
    {

        [Harmony12.HarmonyPostfix]
        static void Postfix(Locale locale, ref LocalizationPack __result)
        {

            AddImprovedCombatManeuverLocalizedNames(__result);

            if (Locale.enGB.Equals(locale))
            {
                ChangeImprovedCombatManeuverFeatDescriptions(__result);
                ChangeCombatManeuverActionDescriptions(__result);
            }

        }

        private static void AddImprovedCombatManeuverLocalizedNames(LocalizationPack pack)
        {
            CombatManeuverData.newImprovedCombatManeuverFeatNames.ForEach((key, value) => Localization.AddStringToLocalizationPack(key, value, pack));
        }

        private static void ChangeImprovedCombatManeuverFeatDescriptions(LocalizationPack pack)
        {
            CombatManeuverData.updatedImprovedCombatManeuverFeatDescriptions.ForEach((key, value) => Localization.ChangeStringForLocalizationPack(key, value, pack));
        }

        private static void ChangeCombatManeuverActionDescriptions(LocalizationPack pack)
        {
            CombatManeuverData.updatedCombatManeuverActionDescriptions.ForEach((key, value) => Localization.ChangeStringForLocalizationPack(key, value, pack));
        }
    }
}
