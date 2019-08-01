using BetterCombat.Rules.SoftCover;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.RuleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Components.Rulebook.SoftCover
{
    [AllowedOn(typeof(BlueprintUnitFact))]
    public class IgnoreSoftCoverOnRangedAttack : RuleInitiatorLogicComponent<RuleCheckSoftCover>
    {
        public override void OnEventAboutToTrigger(RuleCheckSoftCover evt)
        {
            if (evt.AttackType.IsRanged())
                evt.AttackerIgnoresCover = true;
        }


        public override void OnEventDidTrigger(RuleCheckSoftCover evt)
        {
        }
    }
}
