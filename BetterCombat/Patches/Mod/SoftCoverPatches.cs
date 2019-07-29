using BetterCombat.Patches.Mod.SoftCover;
using System;

namespace BetterCombat.Patches.Mod
{
    internal class SoftCoverPatches
    {
        internal static Type[] AllPatches =
        {
            typeof(LocalizationManager_AddSoftCoverLocalization_Patch),
            typeof(RuleCalculateAC_SoftCover_Patch),
            typeof(Library_ModifyImprovedPreciseShot_Patch)
        };
    }
}
