using BetterCombat.Rules.SoftCover;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Controllers.Combat;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Components.Rulebook.SoftCover
{
    [AllowedOn(typeof(BlueprintUnitFact))]
    public class IgnoreSoftCover : RuleInitiatorLogicComponent<RuleCheckSoftCover>
    {

        public bool MeleeReachAttacksOnly;

        public bool RangedAttacksOnly;
        public bool MeleeAttacksOnly;
        public bool TouchAttacksOnly;

        public bool OnlyIgnoreAllies;

        public override void OnEventAboutToTrigger(RuleCheckSoftCover evt)
        {
            var hand = evt.Initiator.GetThreatHand();
            if ((MeleeReachAttacksOnly && hand != null && hand.HasWeapon && hand.Weapon.Blueprint.Type.AttackRange > GameConsts.MinWeaponRange) || (!MeleeReachAttacksOnly && CheckAttackType(evt)))
            {
                if (!OnlyIgnoreAllies)
                {
                    evt.AttackerIgnoresCover = true;
                }
                else
                {
                    foreach (UnitEntityData unit in Game.Instance.State.AwakeUnits)
                    {
                        if (unit.IsAlly(evt.Initiator))
                            evt.IgnoreUnits.Add(unit);
                    }
                }
            }
        }

        public bool CheckAttackType(RuleCheckSoftCover evt)
        {
            return !RangedAttacksOnly && !MeleeAttacksOnly && !TouchAttacksOnly
                    || RangedAttacksOnly && TouchAttacksOnly && evt.AttackType == AttackType.RangedTouch
                    || MeleeAttacksOnly && TouchAttacksOnly && evt.AttackType == AttackType.Touch
                    || RangedAttacksOnly && !TouchAttacksOnly && evt.AttackType.IsRanged()
                    || MeleeAttacksOnly && !TouchAttacksOnly && evt.AttackType.IsMelee()
                    || TouchAttacksOnly && !RangedAttacksOnly && !MeleeAttacksOnly && evt.AttackType.IsTouch();
        }

        public override void OnEventDidTrigger(RuleCheckSoftCover evt)
        {
        }
    }
}
