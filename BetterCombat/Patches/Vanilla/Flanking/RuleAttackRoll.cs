using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{

    // RuleAttackRoll.OnTrigger calls IsFlanked once. The context is an attacker rolling to attack a target,
    // and IsFlanked is called to check whether or not the attack is a Sneak Attack:
    //
    // this.IsSneakAttack = this.IsHit && !this.ImmuneToSneakAttack && (this.IsTargetFlatFooted || this.Target.CombatState.IsFlanked) && (int) this.Initiator.Stats.SneakAttack > 0;
    //
    // We can fix the flanking here to use the proper parameters, and this will incidentally also fix the horrible Sneak Attack implementation,
    // as now a ranged attack will never have IsFlanked be true, so the target must be flat-footed for a ranged attack to count as a Sneak Attack
    //
    // The call to IsFlanked is conditional and thus needs a postfix to possibly clean up the parameter.
    //
    // FlankingParameters: FlankedBy = __instance.Initiator

    [Harmony12.HarmonyPatch(typeof(RuleAttackRoll), nameof(RuleAttackRoll.OnTrigger), Harmony12.MethodType.Normal)]
    static class RuleAttackRoll_OnTrigger_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(RuleAttackRoll __instance, RulebookEventContext context)
        {
            FlankingParameters flankedParameters = new FlankingParameters(typeof(RuleAttackRoll_OnTrigger_Patch), __instance.Initiator);
            UnitCombatState_get_IsFlanked_Patch.PushFlankingParameters(flankedParameters);
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(RuleAttackRoll __instance, RulebookEventContext context)
        {
            UnitCombatState_get_IsFlanked_Patch.PopFlankingParametersIfTypeMatches(typeof(RuleAttackRoll_OnTrigger_Patch));
        }
    }
}
