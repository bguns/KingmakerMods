using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.RuleSystem.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{

    // Technically, the correct course of action (given the exception-based ruleset nature of pathfinder and the component-based 
    // architecture of Pathfinder: Kingmaker) would be to add a component to all maneuver-on-attack-feats (e.g. TrippingBite) to 
    // prevent the nex AoO on combat maneuver use (see explanation of Option 2 in CombatManeuverProvokeAttack).
    //
    // However, seeing as I'm not aware of any free-combat-maneuver-on-attack rule which *doesn't* state that this combat
    // maneuver does not provoke an AoO from the target, lazyness becomes an important and acceptable factor and I'm just 
    // going to patch the ManeuverOnAttack component, adding this functionality to all such abilities.

    [Harmony12.HarmonyPatch(typeof(ManeuverOnAttack), nameof(ManeuverOnAttack.OnEventDidTrigger), Harmony12.MethodType.Normal)]
    class ManeuverOnAttack_OnEventDidTrigger_NoAoO_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(ManeuverOnAttack __instance, RuleAttackWithWeapon evt)
        {
            CombatManeuverProvokeAttack.DoNotTriggerAoOForNextCombatManeuver(evt.Initiator);
            return true;
        }
    }
}
