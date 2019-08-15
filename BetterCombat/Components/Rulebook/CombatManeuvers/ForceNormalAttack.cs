using BetterCombat.Rules.CombatManeuvers;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;

namespace BetterCombat.Components.Rulebook.CombatManeuvers
{
    [AllowedOn(typeof(BlueprintUnitFact))]
    public class ForceNormalAttack : RuleInitiatorLogicComponent<RuleCheckCombatManeuverReplaceAttack>
    {
        public override void OnEventAboutToTrigger(RuleCheckCombatManeuverReplaceAttack evt)
        {
            evt.ForceNormalAttack = true;
        }

        public override void OnEventDidTrigger(RuleCheckCombatManeuverReplaceAttack evt)
        {
        }
    }
}
