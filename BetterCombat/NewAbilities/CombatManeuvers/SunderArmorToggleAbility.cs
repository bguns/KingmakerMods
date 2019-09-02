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
    public class SunderArmorToggleAbility
    {
        public static ActivatableAbilityData Data = new ActivatableAbilityData(
            "a0808193670849199f52deb731638297",                                                             // Guid
            "SunderArmorToggleAbility",                                                                      // Name
            CombatManeuverData.combatManeuverActionDisplayNameKeys[CombatManeuver.SunderArmor],              // DisplayNameLocalizationKey (Trip)
            CombatManeuverData.combatManeuverActionDisplayNames[
                CombatManeuverData.combatManeuverActionDisplayNameKeys[CombatManeuver.SunderArmor]
                ],                                                                                          // DisplayName
            CombatManeuverData.updatedCombatManeuverActionDescriptionKeys[CombatManeuver.SunderArmor],       // DescriptionLocalizationKey
            CombatManeuverData.updatedCombatManeuverActionDescriptions[
                CombatManeuverData.updatedCombatManeuverActionDescriptionKeys[CombatManeuver.SunderArmor]
                ],                                                                                           // DescriptionText
            //CombatManeuverData.combatManeuverActionIds[CombatManeuver.SunderArmor]                           // IconAssetId (Sunder Armor Action)
            null,                                                                                             // IconAssetId
            CombatManeuverData.combatManeuverActionIcons[CombatManeuver.SunderArmor]                          // IconFileName
            );

        internal static BlueprintActivatableAbility Create()
        {
            var ability = Library.CreateActivatableAbility(Data);
            ability.DeactivateImmediately = true;
            return ability;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class SunderArmorToggle_AddAbility_Patch
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

            Main.Logger?.Write("Adding Sunder Armor Toggle ability...");
            var action = SunderArmorToggleAbility.Create();

            __instance.GetAllBlueprints().Add(action);
            __instance.BlueprintsByAssetId[action.AssetGuid] = action;
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddSunderArmorToggleInitialize_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.SunderArmor]);
            if (action != null && __instance.HasFact(action))
                __instance.RemoveFact(action);
            
            var sunderArmorToggle = library.Get<BlueprintActivatableAbility>(SunderArmorToggleAbility.Data.Guid);
            if (sunderArmorToggle != null && !__instance.HasFact(sunderArmorToggle))
                __instance.AddFact(sunderArmorToggle);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.PostLoad), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddSunderArmorTogglePostLoad_Patch
    {
        static LibraryScriptableObject library = Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.SunderArmor]);
            if (action != null && __instance.HasFact(action))
                __instance.RemoveFact(action);

            var sunderArmorToggle = library.Get<BlueprintActivatableAbility>(SunderArmorToggleAbility.Data.Guid);
            if (sunderArmorToggle != null && !__instance.HasFact(sunderArmorToggle))
                __instance.AddFact(sunderArmorToggle);
        }
    }
}
