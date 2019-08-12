using BetterCombat.NewAbilities.CombatManeuvers;
using Kingmaker.UnitLogic.ActivatableAbilities;
using System.Linq;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{
    // ActivatableAbilities have a Group property, however it is a hard-coded enum that we can't extend, so we patch
    // OnTurnOn to manually check for the grouping.

    [Harmony12.HarmonyPatch(typeof(ActivatableAbility), "OnTurnOn", Harmony12.MethodType.Normal)]
    class CombatManeuverToggleGroupPatch
    {
        private static string[] combatManeuverToggleAbilityBlueprintGuids = { TripToggleAbility.Data.Guid, DisarmToggleAbility.Data.Guid, SunderArmorToggleAbility.Data.Guid };

        [Harmony12.HarmonyPrefix]
        static bool Prefix(ActivatableAbility __instance)
        {
            if (combatManeuverToggleAbilityBlueprintGuids.Contains(__instance.Blueprint.AssetGuid))
            {
                ActivatableAbility[] array = (from a in __instance.Owner.ActivatableAbilities.Enumerable
                                              where a.IsOn
                                                && a != __instance
                                                && combatManeuverToggleAbilityBlueprintGuids.Contains(a.Blueprint.AssetGuid)                                              
                                              select a).ToArray();
                foreach (var toggle in array)
                {
                    toggle.IsOn = false;
                }
            }
            return true;
        }
    }
}
