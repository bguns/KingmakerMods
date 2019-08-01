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
        public bool RangedOnly;
        public bool MeleeOnly;
        public bool TouchOnly;

        public void OnEventAboutToTrigger(RuleCheckSoftCover evt)
        {
            if (Check(evt))
                evt.IgnoreUnits.Add(Owner.Unit);
        }
        
        private bool Check(RuleCheckSoftCover evt)
        {
            return !RangedOnly && !MeleeOnly && !TouchOnly
                    || RangedOnly && TouchOnly && evt.AttackType == AttackType.RangedTouch
                    || MeleeOnly && TouchOnly && evt.AttackType == AttackType.Touch
                    || RangedOnly && !TouchOnly && evt.AttackType.IsRanged()
                    || MeleeOnly && !TouchOnly && evt.AttackType.IsMelee()
                    || TouchOnly && !RangedOnly && !MeleeOnly && evt.AttackType.IsTouch();
        }
        public void OnEventDidTrigger(RuleCheckSoftCover evt)
        {
        }
    }
}
