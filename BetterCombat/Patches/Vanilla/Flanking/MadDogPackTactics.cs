using BetterCombat.Rules;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{

    // The implementation of Pack Tactics is wrong. What the implementation does is check whether Pack Tactics applies
    // before the attack roll is made. If it applies, it will add a +2 Circumstantial bonus to the attack roll.
    // 
    // However, Pack Tactics states:
    // "At 2nd level, a mad dog and her war beast gain a +4 bonus on attack rolls while flanking the same opponent (instead of the normal +2 bonus)."
    //
    // This makes Pack Tactics one of a number of feats and features that increase the normal flanking bonus (to +4 in this case). Another one of those
    // feats would be Outflank, for example. As Outflank makes the same mistake Pack Tactics does, in the unlikely situation where both would apply
    // (e.g. if a Mad Dog with Outflank is flanking a target with his pet, and also with another character with Outflank), the attacker would 
    // erroneously gain a total bonus of +6 (+2 from flanking, +2 circumstantial bonus from Pack Tactics, +2 circumstantial bonus from Outflank).
    //
    // To fix this, we disable the OnEventAboutToTrigger call, and instead modify the OnEventDidTrigger call. This call will check if the conditions
    // for Pack Tactics apply, and if they do, increase the attack roll's flanking bonus to 4 (and modify the attack roll's result accordingly), but only
    // if it's not already equal to or higher than 4.

    class MadDogPackTacticsPatch
    {

        internal static void OnEventAboutToTrigger(MadDogPackTactics madDogPackTacticsInstance, RuleCalculateAttackBonus evt)
        {
            // do nothing before the attack bonus is calculated
        }

        internal static void OnEventDidTrigger(MadDogPackTactics madDogPackTacticsInstance, RuleCalculateAttackBonus evt)
        {
            Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> packTacticsFlankingConditions =
            (target, owner, flankingPartner) => (owner.Descriptor.IsPet && owner.Descriptor.Master == flankingPartner) || (flankingPartner.Descriptor.IsPet && flankingPartner.Descriptor.Master == owner);

            if (evt.Target.IsFlankedByUnit(madDogPackTacticsInstance.Owner.Unit, packTacticsFlankingConditions))
            {
                evt.IncreaseFlankingBonusTo(4);
            }
        }
    }

    [Harmony12.HarmonyPatch(typeof(MadDogPackTactics), nameof(MadDogPackTactics.OnEventAboutToTrigger), Harmony12.MethodType.Normal)]
    class MadDogPackTactics_OnEventAboutToTrigger_Patch
    {
        static bool Prefix(MadDogPackTactics __instance, RuleCalculateAttackBonus evt)
        {
            MadDogPackTacticsPatch.OnEventAboutToTrigger(__instance, evt);
            return false;
        }
    }

    [Harmony12.HarmonyPatch(typeof(MadDogPackTactics), nameof(MadDogPackTactics.OnEventDidTrigger), Harmony12.MethodType.Normal)]
    class MadDogPackTactics_OnEventDidTrigger_Patch
    {

        static void Postfix(MadDogPackTactics __instance, RuleCalculateAttackBonus evt)
        {
            MadDogPackTacticsPatch.OnEventDidTrigger(__instance, evt);
        }
    }

}
