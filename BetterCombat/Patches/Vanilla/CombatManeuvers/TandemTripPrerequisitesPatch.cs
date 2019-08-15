using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using System;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{
    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class Library_TandemTripPrerequisites_Patch
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

            var tandemTripFeat = __instance.Get<BlueprintFeature>(CombatManeuverData.tandemTripAssetId);
            var prerequisite = tandemTripFeat.GetComponent<PrerequisiteFeature>();
            tandemTripFeat.RemoveComponent(prerequisite);
        }
    }
}
