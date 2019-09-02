using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.NewAbilities.CombatManeuvers
{
    public class TripToggleAbility
    {
        public static ActivatableAbilityData Data = new ActivatableAbilityData(
            "f59e405d9b3840a181a5668aae07c18b",                                                     // Guid
            "TripToggleAbility",                                                                    // Name
            CombatManeuverData.combatManeuverActionDisplayNameKeys[CombatManeuver.Trip],            // DisplayNameLocalizationKey (Trip)
            CombatManeuverData.combatManeuverActionDisplayNames[
                CombatManeuverData.combatManeuverActionDisplayNameKeys[CombatManeuver.Trip]
                ],                                                                                  // DisplayName
            CombatManeuverData.updatedCombatManeuverActionDescriptionKeys[CombatManeuver.Trip],     // DescriptionLocalizationKey
            CombatManeuverData.updatedCombatManeuverActionDescriptions[
                CombatManeuverData.updatedCombatManeuverActionDescriptionKeys[CombatManeuver.Trip]
                ],                                                                                  // DescriptionText
            CombatManeuverData.combatManeuverActionIds[CombatManeuver.Trip],                         // IconAssetId (Trip Action)
            null                                                                                    // IconFileName
            );

        internal static BlueprintActivatableAbility Create()
        {
            var ability = Library.CreateActivatableAbility(Data);
            ability.DeactivateImmediately = true;
            return ability;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class TripToggle_AddAbility_Patch
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

            Main.Logger?.Write("Adding Trip Toggle ability...");
            var action = TripToggleAbility.Create();

            __instance.GetAllBlueprints().Add(action);
            __instance.BlueprintsByAssetId[action.AssetGuid] = action;
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddTripToggleInitialize_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.Trip]);
            if (action != null && __instance.HasFact(action))
                __instance.RemoveFact(action);
            
            var tripToggle = library.Get<BlueprintActivatableAbility>(TripToggleAbility.Data.Guid);
            if (tripToggle != null && !__instance.HasFact(tripToggle))
                __instance.AddFact(tripToggle);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.PostLoad), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddTripTogglePostLoad_Patch
    {
        static LibraryScriptableObject library = Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.Trip]);
            if (action != null && __instance.HasFact(action))
                __instance.RemoveFact(action);

            var tripToggle = library.Get<BlueprintActivatableAbility>(TripToggleAbility.Data.Guid);
            if (tripToggle != null && !__instance.HasFact(tripToggle))
                __instance.AddFact(tripToggle);
        }
    }
}
