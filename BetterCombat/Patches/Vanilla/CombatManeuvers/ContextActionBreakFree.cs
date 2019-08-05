using BetterCombat.Helpers;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{

    // ContextActionBreakFree is an action used on many buffs with an entangle-like effect (Entangle, Web, Obsidian Flow etc...).
    // It (potentially) allows a unit to make a grapple check to break free with a Grapple check, however this check sets the
    // caster of the effect as the target of the grapple. In rare circumstances, this could thus give the caster of e.g. Web
    // an AoO against a unit that tried to break free from his web using a Grapple combat maneuver.
    //
    // This is obviously wrong, therefore we need to prevent an AoO for combat maneuvers on the RunAction method. However,
    // TryBreakFree does not necessarily use a combat maneuver (if the Mobility or Athletics skill bonus is higher than the CMB,
    // it will use one of those instead). So after the method runs, we need to clean up the "do not provoke aoo" flag for the
    // unit that may or may not have used a combat maneuver.
    //
    // Note that TryBreakFree is also apparently used for "normal" grapple situations, which *would* trigger an AoO, so we
    // cannot patch that method directly.

    [Harmony12.HarmonyPatch(typeof(ContextActionBreakFree), nameof(ContextActionBreakFree.RunAction), Harmony12.MethodType.Normal)]
    class ContextActionBreakFree_RunAction_NoAoO_Patch
    {
        private static TargetWrapper Target
        {
            get
            {
                return ElementsContext.GetData<MechanicsContext.Data>()?.CurrentTarget;
            }
        }

        [Harmony12.HarmonyPrefix]
        static bool Prefix(ContextActionBreakFree __instance)
        {
            if (Target != null)
                CombatManeuverProvokeAttack.DoNotTriggerAoOForNextCombatManeuver(Target.Unit);
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(ContextActionBreakFree __instance)
        {
            if (Target != null)
                CombatManeuverProvokeAttack.ClearDoNotTriggerAoOFlagForUnit(Target.Unit);
        }
    }
}
