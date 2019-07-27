using BetterCombat.Helpers;
using BetterCombat.Rules;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.FactLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{
    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class Library_ModifyCombatManeuverFeats_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix()
        {
            foreach (var combatManeuver in Enum.GetValues(typeof(CombatManeuver)).Cast<CombatManeuver>())
            {
                if (combatManeuver != CombatManeuver.None && combatManeuver != CombatManeuver.Overrun && combatManeuver != CombatManeuver.Grapple && combatManeuver != CombatManeuver.DirtyTrickEntangle && combatManeuver != CombatManeuver.DirtyTrickSickened)
                {
                    Main.Logger?.Write($"Updating feats for {Enum.GetName(typeof(CombatManeuver), combatManeuver)}");
                    var improvedCombatManeuverFeat = library.Get<BlueprintFeature>(CombatManeuverData.combatManeuverFeatIds[combatManeuver]);
                    improvedCombatManeuverFeat.RemoveComponent(improvedCombatManeuverFeat.GetComponent<AddFacts>());
                    improvedCombatManeuverFeat.SetName(Localization.CreateString(CombatManeuverData.newImprovedCombatManeuverFeatNameKeys[combatManeuver], CombatManeuverData.newImprovedCombatManeuverFeatNames[CombatManeuverData.newImprovedCombatManeuverFeatNameKeys[combatManeuver]]));
                    improvedCombatManeuverFeat.AddComponent(CombatManeuverDoNotProvokeAttack.Create(combatManeuver));
                }
            }

            Main.Logger?.Write("Updating Dirty Trick - Entangle and Sickened");
            var improvedDirtyTrickFeat = library.Get<BlueprintFeature>(CombatManeuverData.combatManeuverFeatIds[CombatManeuver.DirtyTrickBlind]);
            improvedDirtyTrickFeat.AddComponent(CombatManeuverDoNotProvokeAttack.Create(CombatManeuver.DirtyTrickEntangle));
            improvedDirtyTrickFeat.AddComponent(CombatManeuverDoNotProvokeAttack.Create(CombatManeuver.DirtyTrickSickened));
        }
    }
}
