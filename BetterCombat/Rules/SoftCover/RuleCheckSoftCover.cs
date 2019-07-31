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
            Main.Logger?.Append("RuleCheckSoftCover Triggered");
            Main.Logger?.Append($"Attacker = {Initiator.CharacterName} ({Initiator.UniqueId})");
            Main.Logger?.Append($"Target = {Target.CharacterName} ({Target.UniqueId})");
            Main.Logger?.Append($"#IgnoreUnits = {IgnoreUnits.Count}");
            Main.Logger?.Append($"AttackerIgnoresCover = {AttackerIgnoresCover}");
            Main.Logger?.Append($"AttackType = {AttackType}");

            if (AttackerIgnoresCover)
            {
                Main.Logger?.Flush();
                return;
            }

            Vector2 fromPosition = Initiator.Position.To2D();
            Vector2 toPosition = Target.Position.To2D();

            int i = 0;

            Main.Logger?.Append("Looping over units in combat...");
            foreach (UnitEntityData unit in Game.Instance.State.AwakeUnits)
            {
                i++;
                if (unit == Initiator || unit == Target)
                {
                    Main.Logger?.Append($"{i}: unit {unit.CharacterName} ({unit.UniqueId}) is either the Initiator or the Target and does not count for cover");
                    continue;
                }

                if (IgnoreUnits.Contains(unit))
                {
                    Main.Logger?.Append($"{i}: unit {unit.CharacterName} ({unit.UniqueId}) is in the IgnoreUnits list and does not count for cover");
                    continue;
                }

                int sizeDifference = Target.Descriptor.State.Size - unit.Descriptor.State.Size;

                // e.g. a Small character cannot provide soft cover for a Large character
                if (sizeDifference >= 2)
                {
                    Main.Logger?.Append($"{i}: the target is of size {Target.Descriptor.State.Size}, while unit {unit.CharacterName} ({unit.UniqueId}) is of size {unit.Descriptor.State.Size}, so the unit does not count for cover.");
                    continue;
                }

                Vector2 unitPosition = unit.Position.To2D();
                // A unit that is closer to the attacker than to the target and is smaller than the attacker does not provide cover to the target
                if (unit.Descriptor.State.Size < Initiator.Descriptor.State.Size && Vector2.Distance(fromPosition, unitPosition) < Vector2.Distance(unitPosition, toPosition))
                {
                    Main.Logger?.Append($"{i}: the attacker is of size {Initiator.Descriptor.State.Size}, the unit is of size {unit.Descriptor.State.Size}, and the unit is {Vector2.Distance(fromPosition, unitPosition)} away from the attacker and {Vector2.Distance(unitPosition, toPosition)} away from the target, so the unit does not count for cover.");
                    continue;
                }

                // Filter nonsensical cases
                if (Vector2.Distance(fromPosition, toPosition) < Vector2.Distance(fromPosition, unitPosition) || VectorMath.AngleBetweenPoints(unitPosition, fromPosition, toPosition) < 90.0f)
                {
                    Main.Logger?.Append($"{i}: Possibility of {unit.CharacterName} ({unit.UniqueId}) providing cover considered nonsensical: ");
                    Main.Logger?.Append($" - Distance from attacker to target: {Vector2.Distance(fromPosition, toPosition)}");
                    Main.Logger?.Append($" - Distance from attacker to unit: {Vector2.Distance(fromPosition, unitPosition)}");
                    Main.Logger?.Append($" - Angle between attacker - unit - target: {VectorMath.AngleBetweenPoints(unitPosition, fromPosition, toPosition)}");
                    continue;
                }


                // DO THE THING!
                float lineDistance = Vector2.Distance(VectorMath.NearestPointOnLineToPoint(fromPosition, toPosition, unitPosition), unitPosition);

                Main.Logger?.Append($"{i}: Calculating potential cover provided by unit {unit.CharacterName} ({unit.UniqueId})...");
                Main.Logger?.Append($" - Distance from a line between the attacker and target positions to the covering unit's position = {lineDistance}");

                float unitCorpulence = unit.Corpulence;
                float targetCorpulence = Target.Corpulence;

                Main.Logger?.Append($" - Target's corpulence = {targetCorpulence}");
                Main.Logger?.Append($" - Covering unit's corpulence = {unitCorpulence}");

                if (lineDistance < unitCorpulence + targetCorpulence)
                {
                    Main.Logger?.Append($" - Distance is smaller than target's corpulence + unit's corpulence, cover is provided by {unit.CharacterName} ({unit.UniqueId})");
                    if (Result == Cover.Partial || lineDistance < unitCorpulence)
                    {
                        if (Result == Cover.Partial)
                            Main.Logger?.Append($" - Cover is already partial, upgrading to full cover...");
                        else if (lineDistance < unitCorpulence)
                            Main.Logger?.Append($" - Distance is smaller than the covering unit's corpulence (i.e. the attack passes through the covering unit's circle), result is full cover");
                        Result = Cover.Full;
                        Main.Logger.Append($" - Cover is full, stopping loop...");
                        break;
                    }
                    Main.Logger?.Append($" - Distance is not smaller than the covering unit's corpulence (i.e. the attack does not pass through the covering unit's circle), result is partial cover");
                    Result = Cover.Partial;
                }
            }

            Main.Logger?.Append($"Final cover result: {Result}");
            Main.Logger?.Flush();
        }
    }
}
