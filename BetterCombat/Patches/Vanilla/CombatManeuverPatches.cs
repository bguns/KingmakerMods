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

        static readonly Dictionary<CombatManeuver, string> updatedImprovedCombatManeuverFeatDescriptionKeys = new Dictionary<CombatManeuver, string>
        {
            { CombatManeuver.Trip, "294cdc6a-8d05-4977-b7ff-a1ac1d9a56be" }, // Improved Trip
            { CombatManeuver.BullRush, "d8ca5ba2-7c38-49ad-afc7-26899432c9aa" }, // Improved Bull Rush
            { CombatManeuver.DirtyTrickBlind, "f74caddc-78f8-468e-8af6-371f8ab3b377" }, // Improved Dirty Trick
            { CombatManeuver.Disarm, "13d9f747-9c43-49e1-a50d-fed7695e8581" }, // Improved Disarm
            { CombatManeuver.SunderArmor, "f5cda27a-2587-4978-bdba-74bb2beabe89" }, // Improved Sunder Armor
        };

        static readonly Dictionary<string, string> updatedImprovedCombatManeuverFeatDescriptions = new Dictionary<string, string>
        {
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.Trip], "You do not provoke an attack of opportunity when performing a trip combat maneuver. In addition, you receive a +2 bonus on checks made to trip a foe. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries to trip you." },
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.BullRush], "You do not provoke an attack of opportunity when performing a bull rush combat maneuver. In addition, you receive a +2 bonus on checks made to bull rush a foe. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries to bull rush you." },
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.DirtyTrickBlind], "You do not provoke an attack of opportunity when performing a dirty trick combat maneuver. In addition, you receive a +2 bonus on checks made to attempt a dirty trick. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries a dirty trick on you." },
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.Disarm], "You do not provoke an attack of opportunity when performing a disarm combat maneuver. In addition, you receive a +2 bonus on checks made to disarm a foe. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries to disarm you." },
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.SunderArmor], "You do not provoke an attack of opportunity when performing a sunder armor combat maneuver. In addition, you receive a +2 bonus on checks made to sunder the armor of a foe. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries to sunder your armor." }
        };

        static readonly Dictionary<CombatManeuver, string> updatedCombatManeuverActionDescriptionKeys = new Dictionary<CombatManeuver, string>
        {
            { CombatManeuver.Trip, "882f2dbb-e005-4b83-bbe0-83aa3230e932" }, // Trip
            { CombatManeuver.BullRush, "0f9dde7c-42a4-4779-ae83-ac3ff7979f0f" }, // Bull Rush
            { CombatManeuver.DirtyTrickBlind, "0986ab45-3f84-4540-8b5e-ad6e6ab04942" }, // Dirty Trick - Blindness
            { CombatManeuver.DirtyTrickEntangle, "66cdeddf-e956-4d8c-9eba-d1b7c4616148" }, // Dirty Trick - Entangle
            { CombatManeuver.DirtyTrickSickened, "763ccc85-1cf0-4a83-a9c5-00d8cc85fced" }, // Dirty Trick - Sickened
            { CombatManeuver.Disarm,  "981b15a4-a022-4525-a1bb-a2d2a3e42ce4" }, // Disarm
            { CombatManeuver.SunderArmor, "93591497-f353-4f87-b80b-01d6efcf8b99" }  // Sunder
        };

        static readonly Dictionary<string, string> updatedCombatManeuverActionDescriptions = new Dictionary<string, string>
        {
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.Trip],  "You can attempt to trip your opponent in place of a melee attack. If you do not have the Improved Trip feat, or a similar ability, initiating a trip provokes an attack of opportunity from the target of your maneuver.\nIf your combat maneuver is successful, the target is knocked prone.\nIf the target has more than two legs, add +2 to the DC of the combat maneuver attack roll for each additional leg it has. Some creatures—such as oozes, creatures without legs, and flying creatures—cannot be tripped."},
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.BullRush], "A bull rush attempts to push an opponent straight back without doing any harm. If you do not have the Improved Bull Rush feat, or a similar ability, initiating a bull rush provokes an attack of opportunity from the target of your maneuver.\nIf your combat maneuver is successful, your target is pushed back 5 feet. For every 5 by which your attack exceeds your opponent's CMD, you can push the target back an additional 5 feet.\nAn enemy being moved by a bull rush does not provoke an attack of opportunity because of the movement unless you possess the Greater Bull Rush feat. You cannot bull rush a creature into a square that is occupied by a solid object or obstacle." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.DirtyTrickBlind], "You can attempt to hinder a foe in melee as a standard action. If you do not have the Improved Dirty Trick feat or a similar ability, attempting a dirty trick provokes an attack of opportunity from the target of your maneuver.\nIf your attack is successful, the target is blinded.\nThis condition lasts for 1 round. For every 5 by which your attack exceeds your opponent's CMD, the penalty lasts 1 additional round." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.DirtyTrickEntangle], "You can attempt to hinder a foe in melee as a standard action. If you do not have the Improved Dirty Trick feat or a similar ability, attempting a dirty trick provokes an attack of opportunity from the target of your maneuver.\nIf your attack is successful, the target is entangled.\nThis condition lasts for 1 round. For every 5 by which your attack exceeds your opponent's CMD, the penalty lasts 1 additional round." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.DirtyTrickSickened], "You can attempt to hinder a foe in melee as a standard action. If you do not have the Improved Dirty Trick feat or a similar ability, attempting a dirty trick provokes an attack of opportunity from the target of your maneuver.\nIf your attack is successful, the target is sickened.\nThis condition lasts for 1 round. For every 5 by which your attack exceeds your opponent's CMD, the penalty lasts 1 additional round." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.Disarm], "You can attempt to disarm a foe in melee as a standard action.  If you do not have the Improved Disarm feat, or a similar ability, attempting to disarm a foe provokes an attack of opportunity from the target of your maneuver.\nIf your combat maneuver is successful, the target cannot use his weapons for 1 round.\nFor every 5 by which your attack exceeds your opponent's CMD, the disarmed condition lasts 1 additional round." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.SunderArmor], "You can attempt to dislodge a piece of armor worn by your opponent. If you do not have the Improved Sunder feat, or a similar ability, attempting to sunder armor provokes an attack of opportunity from the target of your maneuver.\nIf your combat maneuver is successful, the target loses its bonuses from armor for 1 round.\nFor every 5 by which your attack exceeds your opponent's CMD, the penalty lasts 1 additional round." }
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

        internal static void AddImprovedCombatManeuverLocalizedNames(LocalizationPack pack)
        {
            newImprovedCombatManeuverFeatNames.ForEach((key, value) => Localization.AddStringToLocalizationPack(key, value, pack));
        }

        internal static void ChangeImprovedCombatManeuverFeatDescriptions(LocalizationPack pack)
        {
            updatedImprovedCombatManeuverFeatDescriptions.ForEach((key, value) => Localization.ChangeStringForLocalizationPack(key, value, pack));
        }

        internal static void ChangeCombatManeuverActionDescriptions(LocalizationPack pack)
        {
            updatedCombatManeuverActionDescriptions.ForEach((key, value) => Localization.ChangeStringForLocalizationPack(key, value, pack));
        }

        internal static void FixCombatManeuverFeats()
        {

            foreach (var combatManeuver in Enum.GetValues(typeof(CombatManeuver)).Cast<CombatManeuver>())
            {
                if (combatManeuver != CombatManeuver.None && combatManeuver != CombatManeuver.Overrun && combatManeuver != CombatManeuver.Grapple && combatManeuver != CombatManeuver.DirtyTrickEntangle && combatManeuver != CombatManeuver.DirtyTrickSickened)
                {
                    Main.Logger?.Write($"Updating feats for {Enum.GetName(typeof(CombatManeuver), combatManeuver)}");
                    var improvedCombatManeuverFeat = library.Get<BlueprintFeature>(combatManeuverFeatIds[combatManeuver]);
                    improvedCombatManeuverFeat.RemoveComponent(improvedCombatManeuverFeat.GetComponent<AddFacts>());
                    improvedCombatManeuverFeat.SetName(Localization.CreateString(newImprovedCombatManeuverFeatNameKeys[combatManeuver], newImprovedCombatManeuverFeatNames[newImprovedCombatManeuverFeatNameKeys[combatManeuver]]));

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
            if (Locale.enGB.Equals(locale))
            {
                CombatManeuverPatches.AddImprovedCombatManeuverLocalizedNames(__result);
                CombatManeuverPatches.ChangeImprovedCombatManeuverFeatDescriptions(__result);
                CombatManeuverPatches.ChangeCombatManeuverActionDescriptions(__result);
            }
            
        }
    }
}
