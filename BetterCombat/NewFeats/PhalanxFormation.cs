using BetterCombat.Components.Rulebook.SoftCover;
using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.NewFeats
{
    public class PhalanxFormationFeat
    {
        public static FeatData Data = new FeatData(
            "f1e93e123f4c4e04978d0ec58597aa5a",                                 // Guid
            "PhalanxFormationFeature",                                          // Name
            "d89af0c4-ad48-4896-9ff5-472d8689d2c4",                             // DisplayNameLocalizationKey
            "Phalanx Formation",                                                // DisplayName
            "e6b6928c-97b1-4771-b636-f6bd67355166",                             // DescriptionLocalizationKey
            "When you wield a reach weapon with which you are proficient, " +
                "allies don’t provide soft cover to opponents you attack " +
                "with reach.",                                                  // DescriptionText
            "308cd7dc4f10efd428f531bbf4f2823d"                                  // IconAssetId (Penetrating Strike Feat)
            );

        internal static BlueprintFeature Create()
        {
            var feat = Library.CreateFeat(Data);

            var ignoreCoverComponent = Library.Create<IgnoreSoftCover>(component =>
            {
                component.MeleeReachAttacksOnly = true;
                component.OnlyIgnoreAllies = true;
            });

            feat.AddComponent(ignoreCoverComponent);

            var babPrereq = Library.Create<PrerequisiteStatValue>(component =>
            {
                component.Stat = StatType.BaseAttackBonus;
                component.Value = 1;
            });

            feat.AddComponent(babPrereq);

            return feat;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class PhalanxFormation_AddFeat_Patch
    {

        static bool initialized = false;

        [Harmony12.HarmonyPrefix]
        static bool Prefix(LibraryScriptableObject __instance)
        {
            initialized = __instance.Initialized();
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(LibraryScriptableObject __instance)
        {
            if (initialized)
                return;

            Main.Logger?.Write("Adding Phalanx Formation feat...");
            var feat = PhalanxFormationFeat.Create();

            __instance.GetAllBlueprints().Add(feat);
            __instance.BlueprintsByAssetId[feat.AssetGuid] = feat;

            var featGroups = FeatSelection.AllCombat;
            featGroups = featGroups.AddToArray(FeatSelection.Basic);

            foreach (string featGroupId in featGroups)
            {
                __instance.AddFeatToFeatureGroup(feat, featGroupId);
            }
        }
    }

    [Harmony12.HarmonyPatch(typeof(LocalizationManager), "LoadPack", Harmony12.MethodType.Normal)]
    class PhalanxFormation_AddLocalization_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix(Locale locale, ref LocalizationPack __result)
        {
            Localization.AddStringToLocalizationPack(PhalanxFormationFeat.Data.DisplayNameLocalizationKey, PhalanxFormationFeat.Data.DisplayNameText, __result);
            Localization.AddStringToLocalizationPack(PhalanxFormationFeat.Data.DescriptionLocalizationKey, PhalanxFormationFeat.Data.DescriptionText, __result);
        }
    }
}
