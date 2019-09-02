using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using System;


namespace BetterCombat.NewAbilities.CombatManeuvers
{
    public class DisarmToggleAbility
    {
        public static ActivatableAbilityData Data = new ActivatableAbilityData(
            "dc8f4de1da32443db350ac82385ac13d",                                                         // Guid
            "DisarmToggleAbility",                                                                      // Name
            CombatManeuverData.combatManeuverActionDisplayNameKeys[CombatManeuver.Disarm],              // DisplayNameLocalizationKey (Trip)
            CombatManeuverData.combatManeuverActionDisplayNames[
                CombatManeuverData.combatManeuverActionDisplayNameKeys[CombatManeuver.Disarm]
                ],                                                                                      // DisplayName
            CombatManeuverData.updatedCombatManeuverActionDescriptionKeys[CombatManeuver.Disarm],       // DescriptionLocalizationKey
            CombatManeuverData.updatedCombatManeuverActionDescriptions[
                CombatManeuverData.updatedCombatManeuverActionDescriptionKeys[CombatManeuver.Disarm]
                ],                                                                                      // DescriptionText
            null,                                                                                         // IconAssetId
            CombatManeuverData.combatManeuverActionIcons[CombatManeuver.Disarm]                           // IconFileName
            );

        internal static BlueprintActivatableAbility Create()
        {
            var ability = Library.CreateActivatableAbility(Data);
            ability.DeactivateImmediately = true;
            return ability;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class DisarmToggle_AddAbility_Patch
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

            Main.Logger?.Write("Adding Disarm Toggle ability...");
            var action = DisarmToggleAbility.Create();

            __instance.GetAllBlueprints().Add(action);
            __instance.BlueprintsByAssetId[action.AssetGuid] = action;
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddDisarmToggleInitialize_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.Disarm]);
            if (action != null && __instance.HasFact(action))
                __instance.RemoveFact(action);
            
            var disarmToggle = library.Get<BlueprintActivatableAbility>(DisarmToggleAbility.Data.Guid);
            if (disarmToggle != null && !__instance.HasFact(disarmToggle))
                __instance.AddFact(disarmToggle);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.PostLoad), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddDisarmTogglePostLoad_Patch
    {
        static LibraryScriptableObject library = Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.Disarm]);
            if (action != null && __instance.HasFact(action))
                __instance.RemoveFact(action);

            var disarmToggle = library.Get<BlueprintActivatableAbility>(DisarmToggleAbility.Data.Guid);
            if (disarmToggle != null && !__instance.HasFact(disarmToggle))
                __instance.AddFact(disarmToggle);
        }
    }
}
