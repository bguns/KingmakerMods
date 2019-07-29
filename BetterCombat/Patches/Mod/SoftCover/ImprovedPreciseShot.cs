using BetterCombat.Helpers;
using BetterCombat.Rules.SoftCover;
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

            Main.Logger?.Write("Library_ModifyImprovedPreciseStrike_Patch postfix");
            var improvedPreciseStrikeFeat = __instance.Get<BlueprintFeature>(SoftCoverData.ImprovedPreciseShotFeatId);
            if (improvedPreciseStrikeFeat.GetComponent<IgnoreCoverOnRangedAttack>() == null)
            {
                Main.Logger?.Write("Library_ModifyImprovedPreciseStrike_Patch adding ignore cover on ranged attack component");
                improvedPreciseStrikeFeat.AddComponent(Library.Create<IgnoreCoverOnRangedAttack>());
            }
        }
    }
}
