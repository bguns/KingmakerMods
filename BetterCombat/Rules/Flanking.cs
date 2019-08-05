using BetterCombat.Helpers;
using Kingmaker.Controllers.Combat;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterCombat.Rules
{
    public static class Flanking
    {
        public static bool IsFlankedByUnit(
            this UnitEntityData target,
            UnitEntityData attacker,
            Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> flankingPreconditions = null)
        {
            // You can only flank if you're not flat-footed vs. your target. Perhaps this should be a patch on IsEngage,
            // but I'm not sure what the consequences of that would be, so we do it here.
            if (attacker.IsFlatFootedTo(target))
            {
                return false;
            }

            foreach (UnitEntityData attacker2 in target.CombatState.EngagedBy)
            {
                if (attacker2 == attacker)
                    continue;

                if (target.IsFlankedByUnits(attacker, attacker2, flankingPreconditions))
                    return true;
            }
            return false;
        }

        public static bool IsFlankedByUnits(
            this UnitEntityData target,
            UnitEntityData unit1,
            UnitEntityData unit2,
            Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> flankingPreconditions = null)
        {
            // Block nonsensical parameters. It takes three to tango.
            if (target == null || unit1 == null || unit2 == null || target == unit1 || target == unit2 || unit1 == unit2)
                return false;

            // Flanking requires not being flat-footed (see above).
            if (unit1.IsFlatFootedTo(target))
                return false;
            
            if (unit2.IsFlatFootedTo(target))
                return false;
           

            // Flanking requires threatening
            if (!unit1.IsEngage(target) || !unit2.IsEngage(target))
                return false;

            // Check for any extra preconditions on the flanking participants
            if (flankingPreconditions != null && !flankingPreconditions(target, unit1, unit2))
                return false;

            // Ideally, the flanking partners should be more than 120 degrees from each other with respect to the target.
            // However, I found that with the isometric perspective, the angle is often hard to judge, so we're lenient here
            // and make it 115 degrees. 
            return VectorMath.AngleBetweenPoints(target.Position.To2D(), unit1.Position.To2D(), unit2.Position.To2D()) > 115.0f;
        }

        

        #region Extension methods

        private static bool IsFlatFootedTo(this UnitEntityData unit1, UnitEntityData unit2)
        {
            return Rulebook.Trigger(new RuleCheckTargetFlatFooted(unit2, unit1)).IsFlatFooted;
        }

        // Setter for private property Result
        static readonly FastSetter setResult = Harmony.CreateSetter<RuleCalculateAttackBonus>(nameof(RuleCalculateAttackBonus.Result));

        public static void IncreaseFlankingBonusTo(this RuleCalculateAttackBonus evt, int increaseTo)
        {
            int diff = 0;

            if (evt.FlankingBonus <= increaseTo)
            {
                diff = increaseTo - evt.FlankingBonus;
                evt.FlankingBonus = increaseTo;
            }
            if (diff > 0)
                setResult(evt, evt.Result + diff);
        }

        #endregion
    }
}
