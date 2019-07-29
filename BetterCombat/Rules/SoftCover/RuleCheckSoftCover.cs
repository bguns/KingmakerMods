using BetterCombat.Helpers;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterCombat.Rules.SoftCover
{
    public class RuleCheckSoftCover : RulebookTargetEvent
    {
        public List<UnitEntityData> IgnoreUnits { get; private set; }

        public bool AttackerIgnoresCover { get; set; }

        public AttackType AttackType { get; private set; }

        public Cover Result { get; set; }

        public RuleCheckSoftCover(UnitEntityData attacker, UnitEntityData target, AttackType attackType)
            : base(attacker, target)
        {
            IgnoreUnits = new List<UnitEntityData>();
            Result = Cover.None;
            AttackType = attackType;
        }

        public override void OnTrigger(RulebookEventContext context)
        {
            if (AttackerIgnoresCover)
                return;

            Vector2 fromPosition = Initiator.Position.To2D();
            Vector2 toPosition = Target.Position.To2D();

            foreach (UnitEntityData unit in Game.Instance.State.AwakeUnits)
            {
                if (unit == Initiator || unit == Target)
                    continue;

                if (IgnoreUnits.Contains(unit))
                    continue;

                int sizeDifference = Target.Descriptor.State.Size - unit.Descriptor.State.Size;

                // e.g. a Small character cannot provide soft cover for a Large character
                if (sizeDifference >= 2)
                    continue;

                Vector2 unitPosition = unit.Position.To2D();
                // A unit that is closer to the attacker than to the target and is smaller than the attacker does not provide cover to the target
                if (unit.Descriptor.State.Size < Initiator.Descriptor.State.Size && Vector2.Distance(fromPosition, unitPosition) < Vector2.Distance(unitPosition, toPosition))
                    continue;

                // Filter nonsensical cases
                if (Vector2.Distance(fromPosition, toPosition) < Vector2.Distance(fromPosition, unitPosition) || VectorMath.AngleBetweenPoints(unitPosition, fromPosition, toPosition) < 90.0f)
                    continue;


                // DO THE THING!
                float lineDistance = Vector2.Distance(VectorMath.NearestPointOnLineToPoint(fromPosition, toPosition, unitPosition), unitPosition);

                float corpulence = unit.Corpulence;
                float half = unit.Corpulence * 0.5f;
                if (lineDistance < corpulence)
                {
                    if (Result == Cover.Partial || lineDistance < half)
                    {
                        Result = Cover.Full;
                        break;
                    }

                    Result = Cover.Partial;
                }
            }
        }
    }
}
