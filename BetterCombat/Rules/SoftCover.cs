using BetterCombat.Helpers;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums;
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
    public enum Cover
    {
        None,
        Partial,
        Full,
        Improved,
        Total
    }

    public class RuleCheckSoftCover : RulebookTargetEvent
    {
        public List<UnitEntityData> IgnoreUnits { get; private set; }

        public Cover Result { get; set;}

        public RuleCheckSoftCover(UnitEntityData attacker, UnitEntityData target)
            : base(attacker, target)
        {
            IgnoreUnits = new List<UnitEntityData>();
            Result = Cover.None;
        }

        public override void OnTrigger(RulebookEventContext context)
        {
            Main.Logger?.Write($"RuleCheckSoftCover from {Initiator.Descriptor.CharacterName} to {Target.UniqueId}");
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

    /*[ComponentName("Soft Cover AC Bonus")]
    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class ACBonusSoftCover : RuleTargetLogicComponent<RuleCalculateAC>
    {
        public override void OnEventAboutToTrigger(RuleCalculateAC evt)
        {
            Main.Logger?.Write("ACBonusSoftCover");
            Cover cover = Rulebook.Trigger(new RuleCheckSoftCover(evt.Initiator, evt.Target)).Result;
            if (cover == Cover.Full)
                //__instance.AddTemporaryModifier(__instance.Target.Stats.AC.AddModifier(4, null, ModifierDescriptor.Other));
                evt.AddTemporaryModifier(evt.Target.Stats.AC.AddModifier(4, this, ModifierDescriptor.Other));
            else if (cover == Cover.Partial)
                //__instance.AddTemporaryModifier(__instance.Target.Stats.AC.AddModifier(2, null, ModifierDescriptor.Other));
                evt.AddTemporaryModifier(evt.Target.Stats.AC.AddModifier(2, this, ModifierDescriptor.Other));
            Main.Logger?.Write($"ACBonusSoftCover calculated - result = {cover.ToString()}");
        }

        public override void OnEventDidTrigger(RuleCalculateAC evt)
        {
        }
    }*/
}
