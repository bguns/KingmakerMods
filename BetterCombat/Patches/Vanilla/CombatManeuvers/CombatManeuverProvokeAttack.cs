using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{
    
    // Two ways of going about (not) triggering AoO on a combat maneuver:
    //
    // 1) Add a fact to each combat maneuver action and force an AoO in its OnEventAboutToTrigger call unless the initiator has
    //      something which prevents this (e.g. the Improved combat maneuver feats).
    // 2) Patch the OnTrigger call for RuleCombatManeuver to always provoke an AoO unless some value is set to false. Add a fact
    //      to each improved combat maneuver feat which sets this value to false for the next combat maneuver.
    //
    // Option 1 uses less patches (initially). However, option 1 is not really compatible with an exception-based ruleset:
    // the *default* in pathfinder is that combat maneuvers provoke an AoO, and the exception created by e.g. the Improved
    // Combat Maneuver feats, is that they don't. Therefore the correct action is to patch the actual RuleCombatManeuver rule,
    // and add the necessary exceptions by way of extra components on the Improved combat maneuver feats.
    //
    // So, going with option 2, this file patches RuleCombatManeuver.OnTrigger
    static class CombatManeuverProvokeAttack
    {
        public static void DoNotTriggerAoOForNextCombatManeuver(UnitEntityData unit)
        {
            RuleCombatManeuver_OnTrigger_ProvokeAoO_Patch.provokeAoOOnCombatManeuverAttempt[unit.UniqueId] = false;
        }

        public static void ClearDoNotTriggerAoOFlagForUnit(UnitEntityData unit)
        {
            RuleCombatManeuver_OnTrigger_ProvokeAoO_Patch.provokeAoOOnCombatManeuverAttempt.Remove(unit.UniqueId);
        }
    }

    [Harmony12.HarmonyPatch(typeof(RuleCombatManeuver), nameof(RuleCombatManeuver.OnTrigger), Harmony12.MethodType.Normal)]
    class RuleCombatManeuver_OnTrigger_ProvokeAoO_Patch
    { 
        internal static Dictionary<string, bool> provokeAoOOnCombatManeuverAttempt = new Dictionary<string, bool>();

        [Harmony12.HarmonyPrefix]
        static bool Prefix(RuleCombatManeuver __instance, RulebookEventContext context)
        {
            bool provokeAoO;
            if (!provokeAoOOnCombatManeuverAttempt.TryGetValue(__instance.Initiator.UniqueId, out provokeAoO) || provokeAoO)
            {
                if (!__instance.Target.CombatState.IsEngage(__instance.Initiator))
                    return true;

                __instance.Target.CombatState.AttackOfOpportunity(__instance.Initiator);
            }
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(RuleCombatManeuver __instance, RulebookEventContext context)
        {
            provokeAoOOnCombatManeuverAttempt[__instance.Initiator.UniqueId] = true;
        }

        
    }
}
