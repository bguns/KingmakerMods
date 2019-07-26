using BetterCombat.Helpers;
using BetterCombat.Patches.Vanilla.CombatManeuvers;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Controllers.Combat;
using Kingmaker.Controllers.Units;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Parts;
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

        static readonly Dictionary<CombatManeuver, string> combatManeuverFeatIds = new Dictionary<CombatManeuver, string>()
        {
            { CombatManeuver.Trip, "0f15c6f70d8fb2b49aa6cc24239cc5fa" }, // (Improved) Trip
            { CombatManeuver.BullRush, "b3614622866fe7046b787a548bbd7f59" }, // (Improved) Bull Rush
            { CombatManeuver.DirtyTrickBlind,  "ed699d64870044b43bb5a7fbe3f29494" }, // (Improved) Dirty Trick
            { CombatManeuver.Disarm,  "25bc9c439ac44fd44ac3b1e58890916f" }, // (Improved) Disarm
            { CombatManeuver.SunderArmor,  "9719015edcbf142409592e2cbaab7fe1" }  // (Improved) Sunder
        };

        static readonly Dictionary<CombatManeuver, string> combatManeuverActionIds = new Dictionary<CombatManeuver, string>()
        {
            { CombatManeuver.Trip, "6fd05c4ecfebd6f4d873325de442fc17" }, // Trip
            { CombatManeuver.BullRush, "7ab6f70c996fe9b4597b8332f0a3af5f" }, // Bull Rush
            { CombatManeuver.DirtyTrickBlind, "8b7364193036a8d4a80308fbe16c8187" }, // Dirty Trick - Blindness
            { CombatManeuver.DirtyTrickEntangle, "5f22daa9460c5844992bf751e1e8eb78" }, // Dirty Trick - Entangle
            { CombatManeuver.DirtyTrickSickened, "4921b86ee42c0b54e87a2f9b20521ab9" }, // Dirty Trick - Sickened
            { CombatManeuver.Disarm,  "45d94c6db453cfc4a9b99b72d6afe6f6" }, // Disarm
            { CombatManeuver.SunderArmor, "fa9bfb9fd997faf49a108c4b17a00504" }  // Sunder
        };

        static readonly Dictionary<CombatManeuver, string> newImprovedCombatManeuverFeatNameKeys = new Dictionary<CombatManeuver, string>
        {
            { CombatManeuver.Trip,  "44faed15-7bca-412e-b2f5-b5b236a0e6e0" }, // Improved Trip
            { CombatManeuver.BullRush, "be72a033-e8ac-4da7-a41b-66956ee6794d" }, // Improved Bull Rush
            { CombatManeuver.DirtyTrickBlind, "3a253c9c-bd8e-4f37-a839-af304a98c8ee" }, // Improved Dirty Trick
            { CombatManeuver.Disarm, "01bd80d6-f7f5-43e7-9a40-f381da17ab50" }, // Improved Disarm
            { CombatManeuver.SunderArmor, "6ace229a-d135-4ba2-abc2-364629dfcaf4" } // Improved Sunder Armor
        };

        static readonly Dictionary<string, string> newImprovedCombatManeuverFeatNames = new Dictionary<string, string>
        {
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.Trip], "Improved Trip" },
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.BullRush], "Improved Bull Rush" },
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.DirtyTrickBlind], "Improved Dirty Trick" },
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.Disarm], "Improved Disarm" },
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.SunderArmor], "Improved Sunder Armor" }
        };

        
        internal static void AddCombatManeuverActionsToUnit(UnitDescriptor unit)
        {
            combatManeuverActionIds.ForEach((key, value) =>
            {
                var action = library.Get<BlueprintAbility>(value);
                if (action != null)
                    unit.AddFact(action);
            });
        }

        /*internal static void RemoveAddedActionsFromCombatManeuverFeats()
        {
            foreach (var combatManeuverFeatId in combatManeuverFeatIds)
            {
                var feat = library.Get<BlueprintFeature>(combatManeuverFeatId);
                if (feat != null)
                    feat.RemoveComponent(feat.GetComponent<AddFacts>());
            }
        }*/

        internal static void FixImprovedCombatManeuverLocalizationText(LocalizationPack pack)
        {
            newImprovedCombatManeuverFeatNames.ForEach((key, value) => Localization.AddStringToLocalizationPack(key, value, pack));
        }

        static FastSetter blueprintFact_set_name = Harmony.CreateFieldSetter<BlueprintUnitFact>("m_DisplayName");

        internal static void FixCombatManeuverFeats()
        {

            foreach (var combatManeuver in Enum.GetValues(typeof(CombatManeuver)).Cast<CombatManeuver>())
            {
                if (combatManeuver != CombatManeuver.None && combatManeuver != CombatManeuver.Overrun && combatManeuver != CombatManeuver.Grapple && combatManeuver != CombatManeuver.DirtyTrickEntangle && combatManeuver != CombatManeuver.DirtyTrickSickened)
                {
                    Main.Logger?.Write($"Updating feats for {Enum.GetName(typeof(CombatManeuver), combatManeuver)}");
                    var improvedCombatManeuverFeat = library.Get<BlueprintFeature>(combatManeuverFeatIds[combatManeuver]);
                    improvedCombatManeuverFeat.RemoveComponent(improvedCombatManeuverFeat.GetComponent<AddFacts>());
                    blueprintFact_set_name(improvedCombatManeuverFeat as BlueprintUnitFact, newImprovedCombatManeuverFeatNameKeys[combatManeuver]);

                    var combatManeuverAction = library.Get<BlueprintAbility>(combatManeuverActionIds[combatManeuver]);
                    combatManeuverAction.AddComponent(CombatManeuverProvokeAttack.Create(combatManeuver, improvedCombatManeuverFeat));
                }
            }

            Main.Logger?.Write("Updating actions for Dirty Trick");
            var improvedDirtyTrickFeat = library.Get<BlueprintFeature>(combatManeuverFeatIds[CombatManeuver.DirtyTrickBlind]);

            var dirtyTrickEntangleAction = library.Get<BlueprintAbility>(combatManeuverActionIds[CombatManeuver.DirtyTrickEntangle]);
            dirtyTrickEntangleAction.AddComponent(CombatManeuverProvokeAttack.Create(CombatManeuver.DirtyTrickEntangle, improvedDirtyTrickFeat));

            var dirtyTrickSicknessAction = library.Get<BlueprintAbility>(combatManeuverActionIds[CombatManeuver.DirtyTrickSickened]);
            dirtyTrickSicknessAction.AddComponent(CombatManeuverProvokeAttack.Create(CombatManeuver.DirtyTrickSickened, improvedDirtyTrickFeat));
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddCombatManeuverActionsOnInitialize_Patch
    {

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            CombatManeuverPatches.AddCombatManeuverActionsToUnit(__instance);
            /*var tripAction = Main.Library.Get<BlueprintAbility>("6fd05c4ecfebd6f4d873325de442fc17");
            var improvedTripFeat = Main.Library.Get<BlueprintFeature>("0f15c6f70d8fb2b49aa6cc24239cc5fa");
            tripAction.AddComponent(CombatManeuverProvokeAttack.Create(CombatManeuver.Trip, improvedTripFeat));*/
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class Library_ModifyCombatManeuverFeats_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix()
        {
            CombatManeuverPatches.FixCombatManeuverFeats();
        }
    }

    [Harmony12.HarmonyPatch(typeof(LocalizationManager), "LoadPack", Harmony12.MethodType.Normal)]
    class LocalizationManager_FixCombatManeuverFeatText_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix(Locale locale, ref LocalizationPack __result)
        {
            /*Localization.ChangeStringForLocale("9283fcc1-ba37-4711-93ac-6de17415e10f", "Improved Trip", Locale.enGB);
            Localization.ChangeStringForLocale("be940667-072a-4835-8122-8f32a8337e27", "Improved Bull Rush", Locale.enGB);
            Localization.ChangeStringForLocale("a7d3be4d-18ee-4f8c-a540-c3940b2274e9", "Improved Dirty Trick", Locale.enGB);
            Localization.ChangeStringForLocale("d7f35120-78b5-4656-b045-17b9b6b3635c", "Improved Disarm", Locale.enGB);
            Localization.ChangeStringForLocale("fd428ed5-beb2-4986-832b-5193f7424f9f", "Improved Sunder Armor", Locale.enGB);*/

            if (locale != null && locale.Equals(Locale.enGB))
            {
                CombatManeuverPatches.FixImprovedCombatManeuverLocalizationText(__result);
            }
            
        }
    }

    [Harmony12.HarmonyPatch(typeof(LocalizedString), "LoadString", new Type[] { typeof(LocalizationPack), typeof(Locale)})]
    class LocalizedString_LoadString_Patch
    {
        static FastGetter get_localizedString_mKey = Harmony.CreateFieldGetter(typeof(LocalizedString), "m_Key");

        [Harmony12.HarmonyPrefix]
        static bool Prefix(LocalizedString __instance, LocalizationPack pack, Locale locale, ref string __result)
        {
            Main.Logger?.Write("LocalizedString.LoadString");
            object keyObj = get_localizedString_mKey(__instance);
            Main.Logger?.Write("LocalizedString keyObj gotten");
            if (pack == null || (object)keyObj == (object)null)
            {
                __result = string.Empty;
                return false;
            }
            Main.Logger?.Write("keyobj != null");
            string key = (string)keyObj;
            Main.Logger?.Write("keyObj casted to string");
            if ((bool)__instance.Shared)
            {
                Main.Logger?.Write("(bool)__instance.Shared == true");
                LocalizedString localizedString = __instance;
                for (int index = 0; (bool)localizedString.Shared && index < 50; localizedString = localizedString.Shared.String)
                    ++index;
                key = (string)get_localizedString_mKey(localizedString);
            }
            if (key != string.Empty)
            {
                __result = pack.GetText(key, true);
                return false;
            }
            __result = string.Empty;
            return false;
        }
    }
}
