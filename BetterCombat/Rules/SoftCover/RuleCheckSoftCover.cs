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
            if (!Main.Settings.UseSoftCover)
            {
                Main.Logger?.Write("Soft Cover Rules disabled, result of soft cover check is Cover.None");
                Result = Cover.None;
                return;
            }

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


            Vector2 toPosition = Target.Position.To2D();
            Vector2 fromPosition = Initiator.Position.To2D();

            int i = 0;

            List<UnitEntityData> unitsToCheck = new List<UnitEntityData>();

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

                if (unit.Descriptor.State.IsDead)
                {
                    Main.Logger?.Append($"{i}: unit {unit.CharacterName} ({unit.UniqueId}) is dead and does not count for cover");
                    continue;
                }

                if (unit.Descriptor.State.Prone.Active)
                {
                    Main.Logger?.Append($"{i}: unit {unit.CharacterName} ({unit.UniqueId}) is prone and does not count for cover");
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

                unitsToCheck.Add(unit);
                Main.Logger?.Append($"{i}: unit {unit.CharacterName} ({unit.UniqueId}) added to list of potential cover-providing units");
            }

            if (unitsToCheck.Count < 1)
            {
                Main.Logger?.Append("No units could possibly provide cover.");
                Main.Logger?.Flush();
                return;
            }


            Vector2 direction = (toPosition - fromPosition).normalized;
            Vector2 up = new Vector2(direction.y, -direction.x);
            Vector2 down = new Vector2(-direction.y, direction.x);

            Vector2 fromPointsStart = fromPosition + up * Initiator.Corpulence;
            Vector2 fromPointsEnd = fromPosition + down * Initiator.Corpulence;

            Vector2 tangent1 = Vector2.zero;
            Vector2 tangent2 = Vector2.zero;
            VectorMath.TangentPointsOnCircleFromPoint(fromPointsStart, toPosition, Target.Corpulence, out tangent1, out tangent2);

            Vector2 toPointsStart = Vector2.zero;

            float normDist = 0.0f;
            if (Pathfinding.VectorMath.SegmentsIntersect2D(fromPosition, toPosition, fromPointsStart, tangent1, out normDist))
                toPointsStart = tangent2;
            else
                toPointsStart = tangent1;

            Vector2 tangent3 = Vector2.zero;
            Vector2 tangent4 = Vector2.zero;
            VectorMath.TangentPointsOnCircleFromPoint(fromPointsEnd, toPosition, Target.Corpulence, out tangent3, out tangent4);

            Vector2 toPointsEnd = Vector2.zero;
            if (Pathfinding.VectorMath.SegmentsIntersect2D(fromPosition, toPosition, fromPointsEnd, tangent3, out normDist))
                toPointsEnd = tangent4;
            else
                toPointsEnd = tangent3;

            Vector2[] fromPoints = VectorMath.FindEquidistantPointsOnArc(fromPointsStart, fromPointsEnd, fromPosition, Initiator.Corpulence, 10);
            Vector2[] toPoints = VectorMath.FindEquidistantPointsOnArc(toPointsStart, toPointsEnd, toPosition, Target.Corpulence, 10);

            Main.Logger?.Append("Looping over lines...");
            int raysBlocked = 0;
            for (i = 0; i < fromPoints.Length; i++)
            {
                Main.Logger?.Append($" - Checking line {i} ({fromPoints[i]} to {toPoints[i]})...");
#if DEBUG
                if (i > 0)
                {
                    Main.Logger?.Append($"   * Validate: lines {i} and {i - 1} intersect: {Pathfinding.VectorMath.SegmentsIntersect2D(fromPoints[i - 1], toPoints[i - 1], fromPoints[i], toPoints[i], out normDist)}");
                }
#endif
                for (int j = 0; j < unitsToCheck.Count; j++)
                {
                    var p = VectorMath.NearestPointOnSegmentToPoint(fromPoints[i], toPoints[i], unitsToCheck[j].Position.To2D());
                    var d = Vector2.Distance(p, unitsToCheck[j].Position.To2D());
                    Main.Logger?.Append($"   * closest distance of line {i} to unit {unitsToCheck[j].CharacterName} ({unitsToCheck[j].UniqueId}): {d}. Unit's corpulence: {unitsToCheck[j].Corpulence}");
                    if (Vector2.Distance(p, unitsToCheck[j].Position.To2D()) < unitsToCheck[j].Corpulence)
                    {
                        raysBlocked++;
                        Main.Logger?.Append($"   * line {i} is obstructed by unit {unitsToCheck[j].CharacterName} ({unitsToCheck[j].UniqueId}). Number of obstructed lines so far = {raysBlocked}. Continuing to next line...");
                        break;
                    }
                    else
                    {
                        Main.Logger?.Append($"   * line {i} is not obstructed by unit {unitsToCheck[j].CharacterName} ({unitsToCheck[j].UniqueId})");
                    }
                }

                if (raysBlocked > 6)
                    break;

            }

            Main.Logger?.Append($"Finished looping over target lines. Total lines blocked: {raysBlocked}");
            if (raysBlocked > 6)
                Result = Cover.Full;
            else if (raysBlocked > 2)
                Result = Cover.Partial;
            else
                Result = Cover.None;


            Main.Logger?.Append($"Final cover result: {Result}");
            Main.Logger?.Flush();
        }
    }
}
