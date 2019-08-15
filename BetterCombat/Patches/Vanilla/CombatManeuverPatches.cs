using BetterCombat.Patches.Vanilla.CombatManeuvers;
using System;

namespace BetterCombat.Patches.Vanilla
{
    static class CombatManeuverPatches
    {
        internal static Type[] AllPatches =
        {
            typeof(LocalizationManager_FixCombatManeuverFeatText_Patch),
            typeof(RuleCombatManeuver_OnTrigger_ProvokeAoO_Patch),
            typeof(Library_ModifyCombatManeuverFeats_Patch),
            typeof(ManeuverOnAttack_OnEventDidTrigger_NoAoO_Patch),
            typeof(ContextActionBreakFree_RunAction_NoAoO_Patch),
            typeof(LibraryScriptableObject_CombatManeuverContextActions_Patch),
            typeof(CombatManeuverToggleGroupPatch),
            typeof(Library_TandemTripPrerequisites_Patch)
        };
    }  
}
