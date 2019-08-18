using BetterCombat.Helpers;
using BetterCombat.Patches.Vanilla.CombatManeuvers;
using BetterCombat.Rules.CombatManeuvers;
using Kingmaker.Controllers.Combat;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.AttackOfOpportunity
{
    // In the vanilla game, Attack of Opportunities work roughly as follows (as far as I have been able to determine):
    // - UnitCombatEngagementController.ForceAttackOfOpportunity and UnitCombatEngagementController.ProvokeAttackOfOpportunity put the unit in question into an array
    // - When the game updates, UnitCombatEngagementController.Tick will loop over these arrays and call unit.CombatState.AttackOfOpportunity(target)
    // - UnitCombatState.AttackOfOpportunity will first check all the necessary preconditions for an AoO to be able to happen
    // - If the rules check out, it *starts a new UnitAttackOfOpportunity action* <= This step is crucial
    // - UnitAttackOfOpportunity is an attack action, which will first play an attack animation, and only at the end (on the "hit"), actually
    //      trigger the relevant rules to make an attack roll and possibly deal damage.
    // 
    // This presents a number of problems for actions that can potentially be interrupted by an AoO. 
    // For example (as I discovered pretty early in my combat maneuver modifications), when a unit is tripped, it gets ShouldBeProne set to true.
    // The very next game tick, the UnitProneController picks this up and then instantly sets the unit as Prone and calls unit.Commands.InterruptAll().
    // Afterwards, it politely asks the game if it could maybe start the falling prone animation. This is purely aesthetical, for all rules intents and 
    // purposes, the unit is prone and stops all of its running commands.
    //
    // So, to summarise, suppose we want to cause a trip attempt to grant the intended victim an AoO against his attacker.
    // - Just before the RuleCombatManeuver event triggers we somehow call an AoO from the target against the attacker.
    // - The game starts the UnitAttackOfOpportunity action. While the attack animation for AoO's is sped up, it still takes a couple of frames to complete.
    // - The RuleCombatManeuver resolves, succeeds, and sets the ShouldBeProne attribute to true
    // - The next tick, the UnitProneController sets the unit to prone and cancels all its commands.
    // - The AoO, which was intended to be applied before the actual Trip attempt, never resolves. The battle log will show an AoO entry, but no hit/miss/damage
    //      will ever happen.
    //
    // The only possible fix I could find, was to make AoO's actually instant. I.e. no command or animation is started, the rule is instantly triggered,
    // hit/miss/damage is applied, and the battle and combat log (floaty text above players heads and text log bottom right) show the actual AoO, hit/miss
    // and possible damage info (and that little screenshake on hit).
    //
    // It is somewhat confusing in hectic real-time battles, but then again, what isn't. Pathfinder is a turn-based game.


    [Harmony12.HarmonyPatch(typeof(UnitCombatState), nameof(UnitCombatState.AttackOfOpportunity), new Type[] { typeof(UnitEntityData) })]
    class UnitCombatState_AttackOfOpportunity_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitCombatState __instance, UnitEntityData target, ref bool __result)
        {
            __result = false;
            if (__instance.PreventAttacksOfOpporunityNextFrame || target.CombatState.PreventAttacksOfOpporunityNextFrame || !__instance.CanActInCombat && !__instance.Unit.Descriptor.State.HasCondition(UnitCondition.AttackOfOpportunityBeforeInitiative) || (!__instance.CanAttackOfOpportunity || !__instance.Unit.Descriptor.State.CanAct))
                return false;
            UnitPartForceMove unitPartForceMove = target.Get<UnitPartForceMove>();
            if (unitPartForceMove && !unitPartForceMove.ProvokeAttackOfOpportunity || (UnitCommand.CommandTargetUntargetable(__instance.Unit, target, null) || __instance.Unit.HasMotionThisTick) || (__instance.Unit.GetThreatHand() == null || __instance.AttackOfOpportunityCount <= 0 || (!target.Memory.Contains(__instance.Unit) || target.Descriptor.State.HasCondition(UnitCondition.ImmuneToAttackOfOpportunity))))
                return false;
            if (target.Descriptor.State.HasCondition(UnitCondition.UseMobilityToNegateAttackOfOpportunity))
            {
                RuleCalculateCMD ruleCalculateCmd = Rulebook.Trigger(new RuleCalculateCMD(target, __instance.Unit, CombatManeuver.None));
                if (Rulebook.Trigger(new RuleSkillCheck(target, StatType.SkillMobility, ruleCalculateCmd.Result)).IsPassed)
                    return false;
            }

            // Changed code: instantly trigger AoO check (from UnitAttackOfOpportunity.OnAction)
            //  === Original Start ===
            //  __instance.Unit.Commands.Run((UnitCommand) new UnitAttackOfOpportunity(target));
            //  EventBus.RaiseEvent<IAttackOfOpportunityHandler>((Action<IAttackOfOpportunityHandler>)(h => h.HandleAttackOfOpportunity(__instance.Unit, target)));
            //  === Original End   ===
            //  === Changed Start  ===

            RuleAttackWithWeapon aoo = new RuleAttackWithWeapon(__instance.Unit, target, __instance.Unit.GetThreatHand().Weapon, 0)
            {
                IsAttackOfOpportunity = true
            };
            var combatManeuver = Rulebook.Trigger(new RuleCheckCombatManeuverReplaceAttack(__instance.Unit, target, __instance.Unit.GetThreatHand().Weapon.Blueprint)).Result;

            if (!target.Descriptor.State.IsDead)
            {
                EventBus.RaiseEvent<IAttackOfOpportunityHandler>(h => h.HandleAttackOfOpportunity(__instance.Unit, target));
                if (combatManeuver != CombatManeuver.None)
                {
                    __instance.Unit.TriggerAttackReplacementCombatManeuver(target, __instance.Unit.GetThreatHand().Weapon, 0, combatManeuver);
                }
                else
                {
                    Rulebook.Trigger(aoo);
                }
            }
            // === Changed End    ===

            if (__instance.AttackOfOpportunityCount == __instance.AttackOfOpportunityPerRound)
                __instance.Cooldown.AttackOfOpportunity = 5.4f;
            --__instance.AttackOfOpportunityCount;

            // ===  Added start  === (from UnitAttack.TriggerAttackRule)
            if (combatManeuver == CombatManeuver.None && target.View != null && target.View.HitFxManager != null)
                target.View.HitFxManager.HandleAttackHit(aoo);
            // ===  Added end    ===

            __result = true;
            return false;
        }
    }
}
