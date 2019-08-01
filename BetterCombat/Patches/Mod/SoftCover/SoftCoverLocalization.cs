using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Mod.SoftCover
{
    [Harmony12.HarmonyPatch(typeof(LocalizationManager), "LoadPack", Harmony12.MethodType.Normal)]
    class LocalizationManager_AddSoftCoverLocalization_Patch
    {

        [Harmony12.HarmonyPostfix]
        static void Postfix(Locale locale, ref LocalizationPack __result)
        {
            //if (Locale.enGB.Equals(locale))
            //{
                AddSoftCoverLocalizedNames(__result);
            //}
            if (Locale.enGB.Equals(locale))
            {
                ChangeImprovedPreciseShotDescription(__result);
            }

        }

        private static void AddSoftCoverLocalizedNames(LocalizationPack pack)
        {
            Localization.AddStringToLocalizationPack(SoftCoverData.SoftCoverNameKey, SoftCoverData.SoftCoverName, pack);
            Localization.AddStringToLocalizationPack(SoftCoverData.SoftCoverPartialNameKey, SoftCoverData.SoftCoverPartialName, pack);
        }

        private static void ChangeImprovedPreciseShotDescription(LocalizationPack pack)
        {
            Localization.ChangeStringForLocalizationPack(SoftCoverData.ImprovedPreciseShotDescriptionId, SoftCoverData.ImprovedPreciseShotDescription, pack);
        }
    }
}
