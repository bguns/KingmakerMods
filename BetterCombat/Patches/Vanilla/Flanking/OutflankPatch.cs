using BetterCombat.Helpers;
using BetterCombat.Rules;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{
    // Hooo boy, Outflank. The attack bonus part is straightforward enough, but the AoO on crit part is
    // unfortunately quite poorly worded when one considers its interaction with Solo Tactics.
    // 
    // Luckily, there's an official FAQ on the matter:
    //
    //  If an inquisitor uses Solo Tactics (Advanced Player's Guide, page 40) with the Outflank feat (APG, page 165), 
    //  does the enemy provoke attacks of opportunity when hit with a critical hit?
    //  --
    //  Yes, but only when the inquisitors allies score a critical hit against a foe that they both flank.
    //  In this case, the enemy provokes an attack of opportunity from the inquisitor. The reverse is not true, 
    //  since her allies can only gain bonuses from teamwork feats if they themselves possess them. (JMB, 11/24/10)
    //      –Jason Bulmahn (11/24/10)
    //  https://paizo.com/threads/rzs2lfjm&page=3?Solo-Tactics-Outflank-Who-gets-the-AoO#111
    // 
    // So, when an ally crits, an inquistor with Solo Tactics that is flanking the target with that ally, will get an AoO.
    // In the vanilla version, the wrong reading is used, where the Solo Tactics inquisitor grants an AoO to 
    // all his allies when he crits.
    //
    // Unfortunately, the correct rulle presents us with a problem: OutflankProvokeAttack is a RuleInitiatorLogicComponent<RuleAttackRoll>. 
    // Meaning the OnEventAboutToTrigger and OnEventTrigger methods are only called when a unit that has Outflank 
    // makes (initiates) an attack roll against a unit. It will NOT trigger when just any unit makes an attack roll.
    //
    // So in order for the AoO on crit part of Outflank to work correctly, we need to actually disable the OutflankProvokeAttack event,
    // and Postfix the RuleAttackRoll.OnTrigger() event, so as to more or less hard-code in a check for Outflank.
    static class OutflankPatch
    {
        static LibraryScriptableObject library => Main.Library;

        internal static void OutflankProvokeAttackPatch(RuleAttackRoll attackRollInstance, RulebookEventContext context)
        {
            if (!attackRollInstance.IsCriticalConfirmed)
                return;

            var outflankFeature = library.Get<BlueprintFeature>("422dab7309e1ad343935f33a4d6e9f11");

            Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> outflankParticipantConditions =
                (target, aooTestUnit, attacker) => aooTestUnit.Descriptor.State.Features.SoloTactics || aooTestUnit.Descriptor.HasFact(outflankFeature) && attacker.Descriptor.HasFact(outflankFeature);

            foreach(UnitEntityData aooTestUnit in attackRollInstance.Target.CombatState.EngagedBy)
            {
                if (attackRollInstance.Target.IsFlankedByUnits(aooTestUnit, attackRollInstance.Initiator, outflankParticipantConditions))
                {
                    Main.Logger?.Write("Outflank provoked AoO");
                    Game.Instance.CombatEngagementController.ForceAttackOfOpportunity(aooTestUnit, attackRollInstance.Target);
                }
            }
        }


        // While the attack bonus portion of the Outflank feat is straightforward, the vanilla implementation makes the same mistake
        // as the implementation for the Mad Dog's Pack Tactics feature. See the MadDogPackTactics patch for an explanation of this problem
        // (and its solution).
        internal static void OnEventAboutToTrigger(OutflankAttackBonus outflankAttackBonusInstance, RuleCalculateAttackBonus evt)
        {
            // do nothing before the attack bonus is calculated
        }

        internal static void OnEventDidTrigger(OutflankAttackBonus outflankAttackBonusInstance, RuleCalculateAttackBonus evt)
        {
            BlueprintUnitFact outflankFact = outflankAttackBonusInstance.OutflankFact;

            Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> outflankPreconditions =
                (target, owner, flankingPartner) => owner.Descriptor.State.Features.SoloTactics || flankingPartner.Descriptor.HasFact(outflankFact);

            if (evt.Target.IsFlankedByUnit(outflankAttackBonusInstance.Owner.Unit, outflankPreconditions))
            {
                evt.IncreaseFlankingBonusTo(4);
            }
        }
    }

    [Harmony12.HarmonyPatch(typeof(OutflankAttackBonus), nameof(OutflankAttackBonus.OnEventAboutToTrigger), Harmony12.MethodType.Normal)]
    class OutflankAttackBonus_OnEventAboutToTrigger_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(OutflankAttackBonus __instance, RuleCalculateAttackBonus evt)
        {
            OutflankPatch.OnEventAboutToTrigger(__instance, evt);
            return false;
        }
    }

    [Harmony12.HarmonyPatch(typeof(OutflankAttackBonus), nameof(OutflankAttackBonus.OnEventDidTrigger), Harmony12.MethodType.Normal)]
    class OutflankAttackBonus_OnEventDidTrigger_Patch
    {

        [Harmony12.HarmonyPostfix]
        static void Postfix(OutflankAttackBonus __instance, RuleCalculateAttackBonus evt)
        {
            OutflankPatch.OnEventDidTrigger(__instance, evt);
        }
    }

    [Harmony12.HarmonyPatch(typeof(RuleAttackRoll), nameof(RuleAttackRoll.OnTrigger), Harmony12.MethodType.Normal)]
    class OutflankProvokeAttack_RuleAttackRoll_OnTrigger_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix(RuleAttackRoll __instance, RulebookEventContext context)
        {
            OutflankPatch.OutflankProvokeAttackPatch(__instance, context);
        }
    }

    // disable old OutflankProvokeAttack behaviour
    [Harmony12.HarmonyPatch(typeof(OutflankProvokeAttack), nameof(OutflankProvokeAttack.OnEventDidTrigger), Harmony12.MethodType.Normal)]
    class OutflankProvokeAttack_OnEventDidTrigger_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(OutflankProvokeAttack __instance, RuleAttackRoll evt)
        {
            return false;
        }
    }
}
