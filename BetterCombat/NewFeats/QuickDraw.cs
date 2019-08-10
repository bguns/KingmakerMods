using BetterCombat.Components.Rulebook.EquipItems;
using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using System;

namespace BetterCombat.NewFeats
{
    public class QuickDrawFeat
    {
        public static FeatData Data = new FeatData(
            "ca0ac04222414763abd00f25c83e0e83",                                // Guid
            "QuickDrawFeature",                                                // Name
            "ff985c3f-d137-467a-8966-e28e164fbed8",                            // DisplayNameLocalizationKey
            "Quick Draw",                                                      // DisplayName
            "5b6d8512-7332-4511-901c-c8303be3b5fd",                            // DescriptionLocalizationKey
            "You can switch to a different set of weapons as a " +
            "free action, provided your hands are empty when you do so.",      // DescriptionText
            "697d64669eb2c0543abb9c9b07998a38"                                 // IconAssetId (Slashing Grace Feat)
            );

        internal static BlueprintFeature Create()
        {
            var feat = Library.CreateFeat(Data);

            var freeActionComponent = Library.Create<FreeActionEquipmentChange>(component =>
            {
                component.MustHaveFact = feat;
                component.EmptyHandsOnly = true;
                component.HandsEquipmentSetChangeOnly = true;
            });

            feat.AddComponent(freeActionComponent);

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
    class QuickDraw_AddFeat_Patch
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

            Main.Logger?.Write("Adding Quick Draw feat...");
            var feat = QuickDrawFeat.Create();

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
    class QuickDraw_AddLocalization_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix(Locale locale, ref LocalizationPack __result)
        {
            Localization.AddStringToLocalizationPack(QuickDrawFeat.Data.DisplayNameLocalizationKey, QuickDrawFeat.Data.DisplayNameText, __result);
            Localization.AddStringToLocalizationPack(QuickDrawFeat.Data.DescriptionLocalizationKey, QuickDrawFeat.Data.DescriptionText, __result);
        }
    }
}
