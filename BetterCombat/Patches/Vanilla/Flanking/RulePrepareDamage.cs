using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{
    // While RulePrepareDamage does check for IsFlanked, it does so in the context of checking for the sneak attack damage for the 
    // Arcane Trickster's Surprise Spell feature. The check is:
    //
    // this.Target.CombatState.IsFlanked ? (true ? 1 : 0) : (Rulebook.Trigger<RuleCheckTargetFlatFooted>(new RuleCheckTargetFlatFooted(this.Initiator, this.Target)).IsFlatFooted ? 1 : 0);
    //
    // The decompiled code is a bit hard to parse, but essentially this is 1 (which later becomes 'true') if the target is flanked, 
    // and if not, it is 1 when the target is flat-footed to the caster.
    //
    // This is wrong. It is consistent with the (horrible) implementation of sneak attack given the vanilla flanking rules (where a
    // ranged attack would count as a sneak attack if the target is flanked), but it is wrong with what Surprise Spell actually says
    // in the rules, which is that it only applies sneak attack damage if the target is flat-footed.
    //
    // We can fix this by making sure the above call to IsFlanked will always return false (which will make the result solely dependent
    // on the IsFlatFooted check). We do this by putting nonsensical parameters on the stack, which causes our patched IsFlanked to 
    // return false
    //
    // The call to IsFlanked is conditional and thus needs a postfix to possibly clean up the parameter.
    //
    // FlankingParameters: FlankedBy = __instance.Initiator, FlankedByAnyone = true

    [Harmony12.HarmonyPatch(typeof(RulePrepareDamage), nameof(RulePrepareDamage.OnTrigger), Harmony12.MethodType.Normal)]
    static class RulePrepareDamage_OnTrigger_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(RulePrepareDamage __instance, RulebookEventContext context)
        {
            FlankingParameters flankedParameters = new FlankingParameters(typeof(RulePrepareDamage_OnTrigger_Patch), __instance.Initiator, true, null, null);
            UnitCombatState_get_IsFlanked_Patch.PushFlankingParameters(flankedParameters);
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(RulePrepareDamage __instance, RulebookEventContext context)
        {
            UnitCombatState_get_IsFlanked_Patch.PopFlankingParametersIfTypeMatches(typeof(RulePrepareDamage_OnTrigger_Patch));
        }
    }
}
