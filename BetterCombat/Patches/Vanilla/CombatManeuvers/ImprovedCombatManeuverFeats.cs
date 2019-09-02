using BetterCombat.Components.Rulebook.CombatManeuvers;
using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.FactLogic;
using System;
using System.Linq;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{
    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class Library_ModifyCombatManeuverFeats_Patch
    { 

        static bool initialized = false;

        [Harmony12.HarmonyPrefix]
        static bool Prefix(LibraryScriptableObject __instance)
        {
            initialized = __instance.Initialized();
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(LibraryScriptableObject __instance)
        {
            if (initialized)
                return;

            foreach (var combatManeuver in Enum.GetValues(typeof(CombatManeuver)).Cast<CombatManeuver>())
            {
                if (combatManeuver != CombatManeuver.None && combatManeuver != CombatManeuver.Overrun && combatManeuver != CombatManeuver.Grapple && combatManeuver != CombatManeuver.DirtyTrickEntangle && combatManeuver != CombatManeuver.DirtyTrickSickened)
                {
                    var improvedCombatManeuverFeat = __instance.Get<BlueprintFeature>(CombatManeuverData.combatManeuverFeatIds[combatManeuver]);
                    improvedCombatManeuverFeat.RemoveComponent(improvedCombatManeuverFeat.GetComponent<AddFacts>());
                    improvedCombatManeuverFeat.SetName(Localization.CreateString(CombatManeuverData.newImprovedCombatManeuverFeatNameKeys[combatManeuver], CombatManeuverData.newImprovedCombatManeuverFeatNames[CombatManeuverData.newImprovedCombatManeuverFeatNameKeys[combatManeuver]]));
                    improvedCombatManeuverFeat.AddComponent(CombatManeuverDoNotProvokeAttack.Create(combatManeuver));
                    improvedCombatManeuverFeat.SetIcon(LoadIcons.Load(LoadIcons.IconsFolder + CombatManeuverData.improvedCombatManeuverFeatIcons[combatManeuver]));
                }
            }

            var improvedDirtyTrickFeat = __instance.Get<BlueprintFeature>(CombatManeuverData.combatManeuverFeatIds[CombatManeuver.DirtyTrickBlind]);
            improvedDirtyTrickFeat.AddComponent(CombatManeuverDoNotProvokeAttack.Create(CombatManeuver.DirtyTrickEntangle));
            improvedDirtyTrickFeat.AddComponent(CombatManeuverDoNotProvokeAttack.Create(CombatManeuver.DirtyTrickSickened));
        }
    }
}
