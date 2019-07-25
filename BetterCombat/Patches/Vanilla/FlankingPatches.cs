using Kingmaker.RuleSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla
{
    internal class FlankingPatches
    {
        internal static Type[] AllPatches =
        {
            typeof(Flanking.UnitCombatState_get_IsFlanked_Patch),
            typeof(Flanking.RuleCalculateAttackBonus_OnTrigger_Patch),
            typeof(Flanking.RuleAttackRoll_OnTrigger_Patch),
            typeof(Flanking.RulePrepareDamage_OnTrigger_Patch),
            typeof(Flanking.PreciseStrike_OnEventAboutToTrigger_Patch),
            typeof(Flanking.MadDogPackTactics_OnEventAboutToTrigger_Patch),
            typeof(Flanking.MadDogPackTactics_OnEventDidTrigger_Patch),
            typeof(Flanking.FlankedAttackBonus_OnEventAboutToTrigger_Patch),
            typeof(Flanking.OutflankAttackBonus_OnEventAboutToTrigger_Patch),
            typeof(Flanking.OutflankAttackBonus_OnEventDidTrigger_Patch),
            typeof(Flanking.OutflankProvokeAttack_RuleAttackRoll_OnTrigger_Patch),
            typeof(Flanking.OutflankProvokeAttack_OnEventDidTrigger_Patch),
            typeof(Flanking.BackToBack_OnEventAboutToTrigger_Patch),
            typeof(Flanking.ModifyD20_CheckTandemTrip_Patch)
        };
    }
}
