using BetterCombat.Rules.SoftCover;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic;
using System.Linq;

namespace BetterCombat.Components.Rulebook.SoftCover
{
    [AllowedOn(typeof(BlueprintUnitFact))]
    public class IgnoreUnitForSoftCover : OwnedGameLogicComponent<UnitDescriptor>, IGlobalRulebookHandler<RuleCheckSoftCover>, IRulebookHandler<RuleCheckSoftCover>, IGlobalRulebookSubscriber
    {
        public bool RangedAttacksOnly;
        public bool MeleeAttacksOnly;
        public bool TouchAttacksOnly;

        public void OnEventAboutToTrigger(RuleCheckSoftCover evt)
        {
            if (CheckAttackType(evt))
                evt.IgnoreUnits.Add(Owner.Unit);
        }
        
        private bool CheckAttackType(RuleCheckSoftCover evt)
        {
            return !RangedAttacksOnly && !MeleeAttacksOnly && !TouchAttacksOnly
                    || RangedAttacksOnly && TouchAttacksOnly && evt.AttackType == AttackType.RangedTouch
                    || MeleeAttacksOnly && TouchAttacksOnly && evt.AttackType == AttackType.Touch
                    || RangedAttacksOnly && !TouchAttacksOnly && evt.AttackType.IsRanged()
                    || MeleeAttacksOnly && !TouchAttacksOnly && evt.AttackType.IsMelee()
                    || TouchAttacksOnly && !RangedAttacksOnly && !MeleeAttacksOnly && evt.AttackType.IsTouch();
        }
        public void OnEventDidTrigger(RuleCheckSoftCover evt)
        {
        }
    }
}
