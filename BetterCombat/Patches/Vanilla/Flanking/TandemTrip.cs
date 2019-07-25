using Kingmaker.Controllers.Combat;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.Flanking
{
    // Tandem Trip does not actually rely on flanking as per the rules. Merely
    // threatening the target with a teamwork partner is enough. The vanilla code
    // uses the vanilla IsFlanked logic to basically check if at least two opponents
    // are threatening the target. Kinda sorta works, but since we patched IsFlanked,
    // we need to modify this feat.
    //
    // Even if I were to implement functionality to be able to defer to the default
    // IsFlanked implementation (which I'm not inclined to do), the feat would still
    // be wrong, as Tandem Trip only requires threatening, whereas the default 
    // IsFlanked code checks if units are actually attacking the target.
    class TandemTrip
    {
        internal static bool CheckTandemTrip(ModifyD20 modifyD20Instance, RuleCombatManeuver evt)
        {
            if (evt.Type != CombatManeuver.Trip || !modifyD20Instance.Owner.Unit.IsEngage(evt.Target))
                return false;

            if (modifyD20Instance.Owner.State.Features.SoloTactics)
                return true; 

            foreach(UnitEntityData unitEntityData in evt.Target.CombatState.EngagedBy)
            {
                if (unitEntityData != modifyD20Instance.Owner.Unit && unitEntityData.Descriptor.HasFact(modifyD20Instance.TandemTripFeature))
                    return true;
            }

            return false;
        }
    }

    [Harmony12.HarmonyPatch(typeof(ModifyD20), "CheckTandemTrip", Harmony12.MethodType.Normal)]
    static class ModifyD20_CheckTandemTrip_Patch
    {
        static bool Prefix(ModifyD20 __instance, RuleCombatManeuver evt, ref bool __result)
        {
            __result = TandemTrip.CheckTandemTrip(__instance, evt);
            return false;
        }
    }
}
