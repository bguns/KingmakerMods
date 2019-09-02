using BetterCombat.Components.Actions;
using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.Utility;
using System;

namespace BetterCombat.NewAbilities
{

    public class DropHeldWeaponsAbility
    {
        public static AbilityData Data = new AbilityData(
            "47148ed4e2b747a0be47858716a33ae9",                                // Guid
            "DropWeaponsAction",                                               // Name
            "b5674bdd-aef1-4104-a0f8-2a0f355afb81",                            // DisplayNameLocalizationKey
            "Drop Weapons",                                                    // DisplayName
            "e349ae28-95e3-4dd1-a4dc-87e5581c9f8c",                            // DescriptionLocalizationKey
            "Drop the currently active weapon set to the ground.",             // DescriptionText
            "ac8aaf29054f5b74eb18f2af950e752d",                                // IconAssetId (Two-Weapon Fighting)
            null,                                                              // IconFileName
            AbilityType.Special,                                               // AbilityType
            AbilityRange.Personal,                                             // AbilityRange
            UnitCommand.CommandType.Free,                                      // ActionType
            true                                                               // IsFullRoundAction
            );

        internal static BlueprintAbility Create()
        {
            var ability = Library.CreateAbility(Data);

            ability.CanTargetSelf = true;
            ability.NeedEquipWeapons = true;

            var actions = Library.Create<AbilityEffectRunAction>();
            actions.Actions = new ActionList();
            var action = Library.Create<DropWeapons>();
            actions.Actions.Actions = actions.Actions.Actions.AddToArray(action);

            ability.AddComponent(actions);

            return ability;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class DropWeapons_AddAction_Patch
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

            Main.Logger?.Write("Adding Drop Weapons action...");
            var action = DropHeldWeaponsAbility.Create();

            __instance.GetAllBlueprints().Add(action);
            __instance.BlueprintsByAssetId[action.AssetGuid] = action;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LocalizationManager), "LoadPack", Harmony12.MethodType.Normal)]
    class DropWeapons_AddLocalization_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix(Locale locale, ref LocalizationPack __result)
        {
            Localization.AddStringToLocalizationPack(DropHeldWeaponsAbility.Data.DisplayNameLocalizationKey, DropHeldWeaponsAbility.Data.DisplayNameText, __result);
            Localization.AddStringToLocalizationPack(DropHeldWeaponsAbility.Data.DescriptionLocalizationKey, DropHeldWeaponsAbility.Data.DescriptionText, __result);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddDropActionOnInitialize_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>(DropHeldWeaponsAbility.Data.Guid);
            if (action != null)
                __instance.AddFact(action);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.PostLoad), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddDropActionIfNotPresentPostLoad_Patch
    {
        static LibraryScriptableObject library = Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>(DropHeldWeaponsAbility.Data.Guid);
            if (action != null && !__instance.HasFact(action))
                __instance.AddFact(action);
        }
    }
}
