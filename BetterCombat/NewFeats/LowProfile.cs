using BetterCombat.Components.Rulebook.SoftCover;
using BetterCombat.Data;
using BetterCombat.Helpers;
using BetterCombat.Rules.Prerequisites;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using System;

namespace BetterCombat.NewFeats
{
    public class LowProfileFeat
    {
        public static FeatData Data = new FeatData(
            "adb468d1af064635bd61c1bc606eb724",                                 // Guid
            "LowProfileFeature",                                                // Name
            "ab153f26-f04b-4aff-9471-bddf012c722a",                             // DisplayNameLocalizationKey
            "Low Profile",                                                      // DisplayName
            "d2581359-5136-4f47-8ebd-1f66f8c89b07",                             // DescriptionLocalizationKey
            "You gain a +1 dodge bonus to AC against ranged attacks. " +
                "In addition, you do not provide soft cover to creatures " +
                "when ranged attacks pass through your square.",                // DescriptionText
            "d09b20029e9abfe4480b356c92095623"                                  // IconAssetId (Thoughness Feat)
            );

        internal static BlueprintFeature Create()
        {
            var feat = Library.CreateFeat(Data);

            var acComponent = Library.Create<ACBonusAgainstAttacks>(component =>
            {
                component.AgainstRangedOnly = true;
                component.Value = 1;
                component.Descriptor = ModifierDescriptor.Dodge;
            });

            var ignoreSoftCoverComponent = Library.Create<IgnoreUnitForSoftCover>(component =>
            {
                component.RangedAttacksOnly = true;
            });

            feat.AddComponent(acComponent);
            feat.AddComponent(ignoreSoftCoverComponent);

            var dexPrerequisite = Library.Create<PrerequisiteStatValue>(p =>
            {
                p.Value = 13;
                p.Stat = StatType.Dexterity;
            });

            var sizePrerequisite = Library.Create<PrerequisiteCharacterSize>(p =>
            {
                p.Value = Size.Small;
                p.OrSmaller = true;
            });

            feat.AddComponent(dexPrerequisite);
            feat.AddComponent(sizePrerequisite);

            return feat;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class LowProfile_AddFeat_Patch
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

            Main.Logger?.Write("Adding Low Profile feat...");
            var feat = LowProfileFeat.Create();

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
    class LowProfile_AddLocalization_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix(Locale locale, ref LocalizationPack __result)
        {
            Localization.AddStringToLocalizationPack(LowProfileFeat.Data.DisplayNameLocalizationKey, LowProfileFeat.Data.DisplayNameText, __result);
            Localization.AddStringToLocalizationPack(LowProfileFeat.Data.DescriptionLocalizationKey, LowProfileFeat.Data.DescriptionText, __result);
        }
    }
}
