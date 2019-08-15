using BetterCombat.Helpers;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Root;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Commands;

namespace BetterCombat.Rules.CombatManeuvers
{
    public class RuleCheckCombatManeuverReplaceAttack : RulebookTargetEvent
    {
        public BlueprintItemWeapon Weapon { get; set; }

        public CombatManeuver Result { get; set; }

        public bool ForceNormalAttack { get; set; }

        public RuleCheckCombatManeuverReplaceAttack(UnitEntityData initiator, UnitEntityData target, BlueprintItemWeapon weapon) : base(initiator, target)
        {
            Result = CombatManeuver.None;
            Weapon = weapon;
        }

        public RuleCheckCombatManeuverReplaceAttack(UnitEntityData initiator, UnitEntityData target, AttackHandInfo attack) : base(initiator, target)
        {
            Result = CombatManeuver.None;
            Weapon = attack.Weapon.Blueprint;
        }

        public override void OnTrigger(RulebookEventContext context)
        {
            if (!Weapon.IsMelee)
                return;

            Result = Initiator.GetActiveCombatManeuverToggle();

            if (Result == CombatManeuver.Trip)
            {
                UnitState state = Target.Descriptor.State;
                                        // same checks as in UnitProneController, if this is true (and the unit is not in a cutscene), state.Prone.Active will be true on the next tick and we also don't want to trip again.
                if (state.Prone.Active || state.Prone.ShouldBeActive || !state.IsConscious || state.HasCondition(UnitCondition.Prone) || state.HasCondition(UnitCondition.Sleeping) || state.HasCondition(UnitCondition.Unconscious))
                {
                    Result = CombatManeuver.None;
                }
            }
            else if (Result == CombatManeuver.Disarm)
            {
                bool canBeDisarmed = false;
                // same checks as in RuleCombatManeuver. If the unit cannot be disarmed (further), don't attempt to disarm.
                ItemEntityWeapon maybeWeapon = Target.Body.PrimaryHand.MaybeWeapon;
                ItemEntityWeapon maybeWeapon2 = Target.Body.SecondaryHand.MaybeWeapon;

                if (maybeWeapon != null && !maybeWeapon.Blueprint.IsUnarmed && !maybeWeapon.Blueprint.IsNatural && !Target.Descriptor.Buffs.HasFact(BlueprintRoot.Instance.SystemMechanics.DisarmMainHandBuff))
                    canBeDisarmed = true;
                else if (maybeWeapon2 != null && !maybeWeapon2.Blueprint.IsUnarmed && !maybeWeapon2.Blueprint.IsNatural && !Target.Descriptor.Buffs.HasFact(BlueprintRoot.Instance.SystemMechanics.DisarmOffHandBuff))
                    canBeDisarmed = true;

                if (!canBeDisarmed)
                    Result = CombatManeuver.None;
            }
            else if (Result == CombatManeuver.SunderArmor)
            {
                if (Target.Descriptor.Buffs.HasFact(BlueprintRoot.Instance.SystemMechanics.SunderArmorBuff))
                    Result = CombatManeuver.None;
            }

            if (ForceNormalAttack)
                Result = CombatManeuver.None;
        }
    }
}
