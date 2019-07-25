using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.RuleSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{

    // I honestly don't know where this is used. However, the IsFlanked call is a straightforward check whether or not
    // the initiator is flanking the target, without any complicating conditions surrounding it (the code also checks
    // whether the initiator is flat-footed to the target, but this merely overlaps with out fixed IsFlanked logic).
    // So we can put the obvious parameters on the stack and forget about this until we notice what bugs it causes.
    //
    // The IsFlanked call is not conditional
    //
    // FlankingParameters: FlankedBy = __instance.Owner.Unit

    [Harmony12.HarmonyPatch(typeof(FlankedAttackBonus), nameof(FlankedAttackBonus.OnEventAboutToTrigger), Harmony12.MethodType.Normal)]
    class FlankedAttackBonus_OnEventAboutToTrigger_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(FlankedAttackBonus __instance, RuleCalculateAttackBonus evt)
        {
            Main.Logger?.Write("FlankedAttackBonus event triggered");
            UnitCombatState_get_IsFlanked_Patch.PushFlankingParameters(new FlankingParameters(typeof(FlankedAttackBonus_OnEventAboutToTrigger_Patch), __instance.Owner.Unit));
            return true;
        }
    }
}
