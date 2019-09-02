using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using System;

namespace BetterCombat.NewAbilities.CombatManeuvers
{
    public class CombatManeuversStandardAbility
    {
        public static AbilityData Data = new AbilityData(
            "708e4347b8f74e61bc87495eedeba198",                                // Guid
            "CombatManeuversStandard",                                         // Name
            "59b9928d-f878-4599-8d0f-bde2dfed21ba",                            // DisplayNameLocalizationKey
            "Combat Maneuvers (Standard Action)",                              // DisplayName
            "77e529c6-aadc-4f4e-a1cf-8ef053a926c1",                            // DescriptionLocalizationKey
            "Perform a Standard action Combat Maneuver",                       // DescriptionText
            "ed699d64870044b43bb5a7fbe3f29494",                                // IconAssetId (Improved Dirty Trick),
            null,
            AbilityType.Special,                                               // AbilityType
            AbilityRange.Touch,                                                // AbilityRange
            UnitCommand.CommandType.Standard,                                  // ActionType
            false                                                              // IsFullRoundAction
            );

        internal static BlueprintAbility Create()
        {
            var ability = Library.CreateAbility(Data);

            ability.CanTargetSelf = true;
            ability.CanTargetEnemies = true;
            ability.EffectOnEnemy = AbilityEffectOnUnit.Harmful;
            ability.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Special;

            var variants = Library.Create<AbilityVariants>();
            variants.Variants = new BlueprintAbility[4];
            variants.Variants[0] = Main.Library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.BullRush]);
            variants.Variants[1] = Main.Library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.DirtyTrickBlind]);
            variants.Variants[2] = Main.Library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.DirtyTrickEntangle]);
            variants.Variants[3] = Main.Library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[CombatManeuver.DirtyTrickSickened]);

            ability.AddComponent(variants);

            return ability;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class CombatManeuversStandardAbility_AddAbility_Patch
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

            Main.Logger?.Write("Adding Combat Maneuvers (Standard) action...");
            var action = CombatManeuversStandardAbility.Create();

            __instance.GetAllBlueprints().Add(action);
            __instance.BlueprintsByAssetId[action.AssetGuid] = action;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LocalizationManager), "LoadPack", Harmony12.MethodType.Normal)]
    class CombatManeuversStandardAbility_AddLocalization_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix(Locale locale, ref LocalizationPack __result)
        {
            Localization.AddStringToLocalizationPack(CombatManeuversStandardAbility.Data.DisplayNameLocalizationKey, CombatManeuversStandardAbility.Data.DisplayNameText, __result);
            Localization.AddStringToLocalizationPack(CombatManeuversStandardAbility.Data.DescriptionLocalizationKey, CombatManeuversStandardAbility.Data.DescriptionText, __result);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddCombatManeuversStandardAbilityOnInitialize_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            foreach (var maneuver in new CombatManeuver[] { CombatManeuver.BullRush, CombatManeuver.DirtyTrickBlind, CombatManeuver.DirtyTrickEntangle, CombatManeuver.DirtyTrickSickened })
            {
                var action = library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[maneuver]);
                if (action != null && __instance.HasFact(action))
                    __instance.RemoveFact(action);
            }
            var groupAction = library.Get<BlueprintAbility>(CombatManeuversStandardAbility.Data.Guid);
            if (groupAction != null && !__instance.HasFact(groupAction))
                __instance.AddFact(groupAction);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.PostLoad), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddCombatManeuversStandardAbilityPostLoad_Patch
    {
        static LibraryScriptableObject library = Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            foreach (var maneuver in new CombatManeuver[] { CombatManeuver.BullRush, CombatManeuver.DirtyTrickBlind, CombatManeuver.DirtyTrickEntangle, CombatManeuver.DirtyTrickSickened })
            {
                var action = library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[maneuver]);
                if (action != null && __instance.HasFact(action))
                    __instance.RemoveFact(action);
            }
            var groupAction = library.Get<BlueprintAbility>(CombatManeuversStandardAbility.Data.Guid);
            if (groupAction != null && !__instance.HasFact(groupAction))
                __instance.AddFact(groupAction);
        }
    }
}
