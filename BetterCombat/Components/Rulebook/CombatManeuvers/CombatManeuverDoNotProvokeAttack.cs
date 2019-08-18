using BetterCombat.Patches.Vanilla.CombatManeuvers;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.RuleSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Components.Rulebook.CombatManeuvers
{
    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class CombatManeuverDoNotProvokeAttack : RuleInitiatorLogicComponent<RuleCombatManeuver>
    {
        public CombatManeuver ManeuverType;

        public static CombatManeuverDoNotProvokeAttack Create(CombatManeuver maneuverType)
        {
            var instance = Helpers.Library.Create<CombatManeuverDoNotProvokeAttack>();
            instance.ManeuverType = maneuverType;
            return instance;
        }

        public override void OnEventAboutToTrigger(RuleCombatManeuver evt)
        {
            if (evt.Type == ManeuverType)
            {
                CombatManeuverProvokeAttack.DoNotTriggerAoOForNextCombatManeuver(evt.Initiator);
            }
        }

        public override void OnEventDidTrigger(RuleCombatManeuver evt)
        {
        }
    }
}
