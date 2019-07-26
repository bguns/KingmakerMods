using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
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
    public class CombatManeuverProvokeAttack : RuleInitiatorLogicComponent<RuleCombatManeuver>
    {

        public CombatManeuver ManeuverType;
        public BlueprintUnitFact ImprovedManeuverFact;

        public static CombatManeuverProvokeAttack Create(CombatManeuver maneuverType, BlueprintUnitFact improvedManeuverFact)
        {
            var instance = Helpers.Library.Create<CombatManeuverProvokeAttack>();
            instance.ManeuverType = maneuverType;
            instance.ImprovedManeuverFact = improvedManeuverFact;
            return instance;
        }

        public override void OnEventAboutToTrigger(RuleCombatManeuver evt)
        {
            if (evt.Type == ManeuverType && !evt.Initiator.Descriptor.HasFact(ImprovedManeuverFact))
            {
                Main.Logger?.Write("Combat Maneuver AoO triggered");
                //RuleCombatManeuver_OnTrigger_AoO_Patch.AttackerTriggeredAttackOfOpportunity = true;
                evt.Target.CombatState.AttackOfOpportunity(evt.Initiator);
            }
        }

        public override void OnEventDidTrigger(RuleCombatManeuver evt)
        {
        }
    }

    [Harmony12.HarmonyPatch(typeof(RuleCombatManeuver), nameof(RuleCombatManeuver.OnTrigger), Harmony12.MethodType.Normal)]
    class RuleCombatManeuver_OnTrigger_AoO_Patch
    {
        internal static bool AttackerTriggeredAttackOfOpportunity = false;

        [Harmony12.HarmonyPrefix]
        static bool Prefix(RuleCombatManeuver __instance)
        {
            if (AttackerTriggeredAttackOfOpportunity)
            {
                Main.Logger?.Write("Combat Maneuver triggered AoO");
                __instance.Target.CombatState.AttackOfOpportunity(__instance.Initiator);
            }
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix()
        {
            AttackerTriggeredAttackOfOpportunity = false;
        }
    }
}
