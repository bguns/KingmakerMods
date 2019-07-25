using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{
    // RuleCalculateAttackBonus.OnTrigger calls IsFlanked once. The context is an attacker rolling to attack a target,
    // so we are interested in whether the initiator of the event is flanking the target.
    //
    // The IsFlanked call is not conditional
    //
    // FlankingParameters: FlankedBy = __instance.Initiator

    [Harmony12.HarmonyPatch(typeof(RuleCalculateAttackBonus), nameof(RuleCalculateAttackBonus.OnTrigger), Harmony12.MethodType.Normal)]
    static class RuleCalculateAttackBonus_OnTrigger_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(RuleCalculateAttackBonus __instance, RulebookEventContext context)
        {
            FlankingParameters flankedParameters = new FlankingParameters(typeof(RuleCalculateAttackBonus_OnTrigger_Patch), __instance.Initiator);
            UnitCombatState_get_IsFlanked_Patch.PushFlankingParameters(flankedParameters);
            return true;
        }
    }
}
