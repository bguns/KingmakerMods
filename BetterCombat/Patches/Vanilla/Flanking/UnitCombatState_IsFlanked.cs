using BetterCombat.Rules;
using Kingmaker.Controllers.Combat;
using Kingmaker.EntitySystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{

    // The vanilla IsFlanked is a property getter that does not take any parameters. It is calculated using data on the
    // target only. This is a problem, as whether a unit is flanking a target depends on their positioning relative to a
    // third unit. 
    //
    // There are two ways of solving this problem. One is to change all methods that call IsFlanked, so that they call a new method
    // instead. This works but it often means destructively patching pretty core functionality (such as RuleAttackRoll and
    // RuleCalculateAttackBonus). This seriously hampers further extensions to these rules, and makes compatibility with any
    // mod that wants to use a non-destructive prefix patch very tricky.
    //
    // The way this mod tries to fix this, is to non-destructively prefix the relevant methods and put the required parameters
    // on a custom stack. It then patches IsFlanked itself (destructively, but this is fine since this is the core problem we're
    // solving), so that when IsFlanked is called, it has access to the parameters it needs.
    //
    // This is not always possible, there is some functionality that checks other things than flanking, but does so with the same
    // erroneous logic, so it will still merely check the engagement status, rather than position, even when IsFlanked is patched.
    // These still need to be destructively patched so as to completely replace them. This is mostly fine as they're still usually
    // things that deal with the flanking rules (Outflank, Mad Dog's Pack Tactics, ...).
    // 
    // Other times the patched method will end up doing unnecessary work. Precise Strike, for example, can be fixed by putting the
    // required flanking parameters on the custom stack (the attacker, and a function that checks for Solo Tacics, or whether both 
    // flanking partners have Precise Strike). So the IsFlanked call will test all the necessary preconditions for Precise Strike
    // to work. However, the vanilla Precise Strike logic will then go on to check for Solo Tactics/Precise Strike in units engaged
    // to the target. This is unnecessary (flanking requires engagement anyway), however since it does not result in wrong results,
    // I still keep Precise Strike as is, so other mods could potentially still patch it without much compatability problems.
    //
    // However, the most important part of doing things this way is still to not break RuleAttackRoll and RuleCalculateAttackBonus.
    // Whether or not flanking-specific feats are destructively patched or not is not a crucial issue, as other mods aren't likely
    // to want to change them in another way.


    [Harmony12.HarmonyPatch(typeof(UnitCombatState), nameof(UnitCombatState.IsFlanked), Harmony12.MethodType.Getter)]
    static class UnitCombatState_get_IsFlanked_Patch
    {
        internal static Stack<FlankingParameters> flankingParameters = new Stack<FlankingParameters>();

        public static void PushFlankingParameters(FlankingParameters parameters)
        {
            flankingParameters.Push(parameters);
        }

        public static FlankingParameters? PopFlankingParametersIfTypeMatches(Type patchType)
        {
            if (flankingParameters.Count > 0 && flankingParameters.Peek().PatchType.Equals(patchType))
            {
                return flankingParameters.Pop();
            }

            return null;
        }

        [Harmony12.HarmonyPrefix]
        [Harmony12.HarmonyPriority(Harmony12.Priority.First)]
        static bool Prefix(UnitCombatState __instance, ref bool __result)
        {
            __result = false;

            // Without flanking parameters, this won't work, so the result is always false. This means all methods that call IsFlanked need
            // to be made compatible, however the alternative would be to defer to the base IsFlanked behaviour, which would result in 
            // unpredictable behaviour.
            if (flankingParameters.Count <= 0)
            {
                return false;
            }

            FlankingParameters currentParameters = flankingParameters.Pop();

            // Nonsensical parameters => false
            if (currentParameters.FlankedBy == null && !currentParameters.FlankedByAnyone || currentParameters.FlankedBy != null && currentParameters.FlankedByAnyone)
                return false;

            // Check if a specific unit is flanking the target (with a possible extra preconditions test)
            if (currentParameters.FlankedBy != null && !currentParameters.FlankedByAnyone)
            {
                __result = __instance.Unit.IsFlankedByUnit(currentParameters.FlankedBy, currentParameters.FlankingPreconditions);
            }
            // Check if the target is by any two attackers (adhering to possible preconditions)
            else if (currentParameters.FlankedByAnyone)
            {
                foreach (UnitEntityData unitEntityData in __instance.EngagedBy)
                {
                    // Ignore the ExceptFlankedBy unit (if it is not null) for the flanking test
                    if (currentParameters.ExceptFlankedBy == null || unitEntityData != currentParameters.ExceptFlankedBy)
                    {
                        if (__instance.Unit.IsFlankedByUnit(unitEntityData, currentParameters.FlankingPreconditions))
                        {
                            __result = true;
                            break;
                        }
                    }
                }
            }

            return false;
        }
    }

    public struct FlankingParameters
    {

        // The type of the patch corresponding to these parameters. This is necessary because the patched IsFlanked call will only
        // pop parameters off the stack if it is actually called. This is a problem when patching a method that only calls IsFlanked under
        // certain conditions. In this case, the method should have a postfix patch which cleans up the stack. For this to work, however,
        // that postfix method should somehow know if it is indeed the parameters object associated with that specific patch, which are 
        // (still) on top of the stack. Therefore, the prefix patch which sets the parameters should pass its own Type, so that the postfix
        // patch can pop the top parameters off the stack IF they match the type.
        // This can still break if an event can be cyclical (i.e. an event can trigger itself), but then there are much bigger problems anyway.
        internal Type PatchType { get; }

        // The unit for which we want to test if it is flanking the target
        internal UnitEntityData FlankedBy { get; }

        // If this is true, we wish to check whether the target is flanked by any two (or more) enemies. Cannot be true if FlankedBy != null
        internal bool FlankedByAnyone { get; }

        // If this is set and FlankedByAnyone is true, this unit is not considered for flanking purposes.
        internal UnitEntityData ExceptFlankedBy { get; }

        // A function that will be called when testing flanking participants, to check for possible additional conditions that the flanking
        // participants must match. Mainly used for flanking teamwork feats, where the flanking "partners" must both have the teamwork feat
        // (Solo Tactics notwithstanding).
        // 
        // The order of the parameters is target, owner/FlankedBy, otherFlankingPartner
        //
        // Example:
        //
        // Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> preciseStrikePreconditions =
        //      (target, owner, flankingPartner) => owner.Descriptor.State.Features.SoloTactics || flankingPartner.Descriptor.HasFact(preciseStrikeFact);
        internal Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> FlankingPreconditions { get; }

        public FlankingParameters(Type patchType, UnitEntityData flankedBy, bool flankedByAnyone, UnitEntityData exceptFlankedBy, Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> flankingPreconditions)
        {
            PatchType = patchType;
            FlankedBy = flankedBy;
            FlankedByAnyone = flankedByAnyone;
            ExceptFlankedBy = exceptFlankedBy;
            FlankingPreconditions = flankingPreconditions;
        }

        public FlankingParameters(Type patchType, UnitEntityData flankedBy) : this(patchType, flankedBy, false, null, null) { }
        public FlankingParameters(Type patchType, UnitEntityData flankedBy, Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> flankingPreconditions) : this(patchType, flankedBy, false, null, flankingPreconditions) { }
        public FlankingParameters(Type patchType, bool flankedByAnyone) : this(patchType, null, flankedByAnyone, null, null) { }
        public FlankingParameters(Type patchType, bool flankedByAnyone, UnitEntityData exceptFlankedBy) : this(patchType, null, flankedByAnyone, exceptFlankedBy, null) { }
    }
}
