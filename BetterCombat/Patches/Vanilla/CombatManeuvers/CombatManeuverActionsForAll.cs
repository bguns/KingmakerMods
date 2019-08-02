using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{
    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddCombatManeuverActionsOnInitialize_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            CombatManeuverData.combatManeuverActionIds.ForEach((key, value) =>
            {
                var action = library.Get<BlueprintAbility>(value);
                if (action != null)
                    __instance.AddFact(action);
            });
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.PostLoad), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddCombatManeuverActionsIfNotPresentPostLoad_Patch
    {
        static LibraryScriptableObject library = Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            CombatManeuverData.combatManeuverActionIds.ForEach((key, value) =>
            {
                var action = library.Get<BlueprintAbility>(value);
                if (action != null && !__instance.HasFact(action))
                    __instance.AddFact(action);
            });
        }
    }
}
