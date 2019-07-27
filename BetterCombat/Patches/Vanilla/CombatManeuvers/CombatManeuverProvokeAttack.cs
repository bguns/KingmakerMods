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
    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class ImprovedCombatManeuverDoNotProvokeAttack : RuleInitiatorLogicComponent<RuleCombatManeuver>
    {
        public CombatManeuver ManeuverType;

        public static ImprovedCombatManeuverDoNotProvokeAttack Create(CombatManeuver maneuverType)
        {
            var instance = Helpers.Library.Create<ImprovedCombatManeuverDoNotProvokeAttack>();
            instance.ManeuverType = maneuverType;
            //instance.ImprovedManeuverFact = improvedManeuverFact;
            return instance;
        }

        public override void OnEventAboutToTrigger(RuleCombatManeuver evt)
        {
            if (evt.Type == ManeuverType)
            {
                Main.Logger?.Write("Combat Maneuver will not trigger AoO");
                RuleCombatManeuver_OnTrigger_ProvokeAoO_Patch.DoNotTriggerAoOForNextCombatManeuver(evt.Initiator);
            }
        }

        public override void OnEventDidTrigger(RuleCombatManeuver evt)
        {
        }
    }

    [Harmony12.HarmonyPatch(typeof(RuleCombatManeuver), nameof(RuleCombatManeuver.OnTrigger), Harmony12.MethodType.Normal)]
    class RuleCombatManeuver_OnTrigger_ProvokeAoO_Patch
    { 
        private static Dictionary<string, bool> provokeAoOOnCombatManeuverAttempt = new Dictionary<string, bool>();

        [Harmony12.HarmonyPrefix]
        static bool Prefix(RuleCombatManeuver __instance, RulebookEventContext context)
        {
            bool provokeAoO;
            if (!provokeAoOOnCombatManeuverAttempt.TryGetValue(__instance.Initiator.UniqueId, out provokeAoO) || provokeAoO)
            {
                __instance.Target.CombatState.AttackOfOpportunity(__instance.Initiator);
            }
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(RuleCombatManeuver __instance, RulebookEventContext context)
        {
            provokeAoOOnCombatManeuverAttempt[__instance.Initiator.UniqueId] = true;
        }

        public static void DoNotTriggerAoOForNextCombatManeuver(UnitEntityData unit)
        {
            provokeAoOOnCombatManeuverAttempt[unit.UniqueId] = false;
        }
    }

    [Harmony12.HarmonyPatch(typeof(ManeuverOnAttack), nameof(ManeuverOnAttack.OnEventAboutToTrigger), Harmony12.MethodType.Normal)]
    class ManeuverOnAttack_OnEventDidTrigger_NoAoO_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(ManeuverOnAttack __instance, RuleAttackWithWeapon evt)
        {
            RuleCombatManeuver_OnTrigger_ProvokeAoO_Patch.DoNotTriggerAoOForNextCombatManeuver(evt.Initiator);
            return true;
        }
    }
}
