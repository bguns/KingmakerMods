using BetterCombat.Components.Rulebook.SoftCover;
using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using System;

namespace BetterCombat.Patches.Mod.SoftCover
{
    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class Library_ModifyImprovedPreciseShot_Patch
    {
        static bool initialized = false;

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

            var improvedPreciseStrikeFeat = __instance.Get<BlueprintFeature>(SoftCoverData.ImprovedPreciseShotFeatId);
            if (improvedPreciseStrikeFeat.GetComponent<IgnoreSoftCover>() == null)
            {
                var ignoreSoftCoverComponent = Library.Create<IgnoreSoftCover>(component =>
                {
                    component.RangedAttacksOnly = true;
                });
                improvedPreciseStrikeFeat.AddComponent(ignoreSoftCoverComponent);
            }
        }
    }
}
