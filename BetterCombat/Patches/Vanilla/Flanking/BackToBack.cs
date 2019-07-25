using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.RuleSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{
    // The IsFlanked check for BackToBack is supposed to check if the owner of the event is flanked by the attacker.
    // (The AC bonus for Back To Back is only applied against attacks from opponents that are actually flanking you).
    //
    // The call to IsFlanked is conditional and thus needs a postfix to possibly clean up the parameter.
    //
    // FlankingParameters: FlankedBy = evt.Initiator
    //
    // NOTE: the logic is simple enough, but testing this is hard because the bad AI will never conciously try to flank

    [Harmony12.HarmonyPatch(typeof(BackToBack), nameof(BackToBack.OnEventAboutToTrigger), Harmony12.MethodType.Normal)]
    class BackToBack_OnEventAboutToTrigger_Patch
    {

        [Harmony12.HarmonyPrefix]
        static bool Prefix(BackToBack __instance, RuleCalculateAC evt)
        {
            UnitCombatState_get_IsFlanked_Patch.PushFlankingParameters(new FlankingParameters(typeof(BackToBack_OnEventAboutToTrigger_Patch), evt.Initiator));
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(BackToBack __instance, RuleCalculateAC evt)
        {
            UnitCombatState_get_IsFlanked_Patch.PopFlankingParametersIfTypeMatches(typeof(BackToBack_OnEventAboutToTrigger_Patch));
        }
    }
}
