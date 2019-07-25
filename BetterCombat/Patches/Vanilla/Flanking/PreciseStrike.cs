using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{

    // PreciseStrike.OnEventAboutToTrigger calls IsFlanked once. The context is an attacker with the Precise Strike feat
    // rolling for damage against a target, so we are interested in the owner of the event flanking the target and either
    // having Solo Tactics, or have a flanking partner who also has Precise Strike.
    //
    // The IsFlanked call is not conditional
    //
    // FlankingParameters: FlankedBy = __instance.Owner.Unit, FlankingPreconditions: owner has Solo Tactics or flankingPartner
    //      has Precise Strike

    [Harmony12.HarmonyPatch(typeof(PreciseStrike), nameof(PreciseStrike.OnEventAboutToTrigger), Harmony12.MethodType.Normal)]
    class PreciseStrike_OnEventAboutToTrigger_Patch
    {

        [Harmony12.HarmonyPrefix]
        static bool Prefix(PreciseStrike __instance, RulePrepareDamage evt)
        {
            BlueprintUnitFact preciseStrikeFact = __instance.PreciseStrikeFact;

            Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> hasPreciseStrike =
                (target, owner, flankingPartner) => owner.Descriptor.State.Features.SoloTactics || flankingPartner.Descriptor.HasFact(preciseStrikeFact);

            FlankingParameters flankingParameters = new FlankingParameters(typeof(PreciseStrike_OnEventAboutToTrigger_Patch), __instance.Owner.Unit, hasPreciseStrike);
            UnitCombatState_get_IsFlanked_Patch.PushFlankingParameters(flankingParameters);

            return true;
        }
    }
}
