using BetterCombat.Helpers;
using BetterCombat.Patches.Vanilla.CombatManeuvers;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Controllers.Units;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla
{
    static class CombatManeuverPatches
    {
        static LibraryScriptableObject library => Main.Library;

        static readonly List<string> combatManeuverFeatIds = new List<string>()
        {
            "0f15c6f70d8fb2b49aa6cc24239cc5fa", // (Improved) Trip
            "b3614622866fe7046b787a548bbd7f59", // (Improved) Bull Rush
            "ed699d64870044b43bb5a7fbe3f29494", // (Improved) Dirty Trick
            "25bc9c439ac44fd44ac3b1e58890916f", // (Improved) Disarm
            "9719015edcbf142409592e2cbaab7fe1"  // (Improved) Sunder
        };

        static readonly List<string> combatManeuverActionIds = new List<string>()
        {
            "6fd05c4ecfebd6f4d873325de442fc17", // Trip
            "7ab6f70c996fe9b4597b8332f0a3af5f", // Bull Rush
            "8b7364193036a8d4a80308fbe16c8187", // Dirty Trick - Blindness
            "5f22daa9460c5844992bf751e1e8eb78", // Dirty Trick - Entangle
            "4921b86ee42c0b54e87a2f9b20521ab9", // Dirty Trick - Sickened
            "45d94c6db453cfc4a9b99b72d6afe6f6", // Disarm
            "fa9bfb9fd997faf49a108c4b17a00504"  // Sunder
        };

        internal static void AddCombatManeuverActionsToUnit(UnitDescriptor unit)
        {
            foreach (var combatManeuverActionId in combatManeuverActionIds)
            {
                var action = library.Get<BlueprintAbility>(combatManeuverActionId);
                if (action != null)
                    unit.AddFact(action);
            }
        }

        internal static void RemoveAddedActionsFromCombatManeuverFeats()
        {
            foreach (var combatManeuverFeatId in combatManeuverFeatIds)
            {
                var feat = library.Get<BlueprintFeature>(combatManeuverFeatId);
                if (feat != null)
                    feat.RemoveComponent(feat.GetComponent<AddFacts>());
            }
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddCombatManeuverActionsOnInitialize_Patch
    {

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            CombatManeuverPatches.AddCombatManeuverActionsToUnit(__instance);
            var tripAction = Main.Library.Get<BlueprintAbility>("6fd05c4ecfebd6f4d873325de442fc17");
            var improvedTripFeat = Main.Library.Get<BlueprintFeature>("0f15c6f70d8fb2b49aa6cc24239cc5fa");
            tripAction.AddComponent(CombatManeuverProvokeAttack.Create(CombatManeuver.Trip, improvedTripFeat));
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class Library_ModifyCombatManeuverFeats_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix()
        {
            CombatManeuverPatches.RemoveAddedActionsFromCombatManeuverFeats();
        }
    }

    [Harmony12.HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.Init), Harmony12.MethodType.Normal)]
    class LocalizationManager_FixCombatManeuverFeatText_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix()
        {
            Localization.ChangeStringForLocale("9283fcc1-ba37-4711-93ac-6de17415e10f", "Improved Trip", Locale.enGB);
            Localization.ChangeStringForLocale("be940667-072a-4835-8122-8f32a8337e27", "Improved Bull Rush", Locale.enGB);
            Localization.ChangeStringForLocale("a7d3be4d-18ee-4f8c-a540-c3940b2274e9", "Improved Dirty Trick", Locale.enGB);
            Localization.ChangeStringForLocale("d7f35120-78b5-4656-b045-17b9b6b3635c", "Improved Disarm", Locale.enGB);
            Localization.ChangeStringForLocale("fd428ed5-beb2-4986-832b-5193f7424f9f", "Improved Sunder Armor", Locale.enGB);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitCommands), nameof(UnitCommands.InterruptAll), new Type[] { })]
    class UnitCommands_InterruptAll_Patch
    {
        internal static Stack<Func<UnitCommand, bool>> predicates = new Stack<Func<UnitCommand, bool>>();

        public static void PushInterruptAllCommandsPredicate(Func<UnitCommand, bool> predicate)
        {
            predicates.Push(predicate);
        }


        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitCommands __instance)
        {
            Main.Logger?.Write("Prefix InterruptAll");
            if (predicates.Count > 0)
            {
                Main.Logger?.Write("Redirecting to InterruptAll(predicate)");
                var predicate = predicates.Pop();
                __instance.InterruptAll(predicate);
                return false;
            }
            return true;
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitProneController), nameof(UnitProneController.Tick), new Type[] { typeof(UnitEntityData) })]
    class UnitProneController_Tick_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitEntityData unit)
        {
            Main.Logger?.Write("Pushing parameters");
            UnitCommands_InterruptAll_Patch.PushInterruptAllCommandsPredicate((cmd) => !cmd.IsFinished && !(cmd is UnitAttackOfOpportunity));
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitEntityData unit)
        {
            Main.Logger?.Write("Clearing parameters");
            UnitCommands_InterruptAll_Patch.predicates.Clear();
        }
    }

}
