using BetterCombat.Helpers;
using BetterCombat.NewAbilities.CombatManeuvers;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.ActivatableAbilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Data
{
    public class CombatManeuverData
    {
        #region BlueprintIds

        internal static readonly Dictionary<CombatManeuver, string> combatManeuverFeatIds = new Dictionary<CombatManeuver, string>()
        {
            { CombatManeuver.Trip, "0f15c6f70d8fb2b49aa6cc24239cc5fa" }, // (Improved) Trip
            { CombatManeuver.BullRush, "b3614622866fe7046b787a548bbd7f59" }, // (Improved) Bull Rush
            { CombatManeuver.DirtyTrickBlind,  "ed699d64870044b43bb5a7fbe3f29494" }, // (Improved) Dirty Trick
            { CombatManeuver.Disarm,  "25bc9c439ac44fd44ac3b1e58890916f" }, // (Improved) Disarm
            { CombatManeuver.SunderArmor,  "9719015edcbf142409592e2cbaab7fe1" }  // (Improved) Sunder
        };

        internal static readonly Dictionary<CombatManeuver, string> combatManeuverActionIds = new Dictionary<CombatManeuver, string>()
        {
            { CombatManeuver.Trip, "6fd05c4ecfebd6f4d873325de442fc17" }, // Trip
            { CombatManeuver.BullRush, "7ab6f70c996fe9b4597b8332f0a3af5f" }, // Bull Rush
            { CombatManeuver.DirtyTrickBlind, "8b7364193036a8d4a80308fbe16c8187" }, // Dirty Trick - Blindness
            { CombatManeuver.DirtyTrickEntangle, "5f22daa9460c5844992bf751e1e8eb78" }, // Dirty Trick - Entangle
            { CombatManeuver.DirtyTrickSickened, "4921b86ee42c0b54e87a2f9b20521ab9" }, // Dirty Trick - Sickened
            { CombatManeuver.Disarm,  "45d94c6db453cfc4a9b99b72d6afe6f6" }, // Disarm
            { CombatManeuver.SunderArmor, "fa9bfb9fd997faf49a108c4b17a00504" }  // Sunder
        };

        internal static readonly Dictionary<string, string> abilitiesThatShouldReplaceContextActionCombatManeuver = new Dictionary<string, string>
        {
            { "a4445991c5bb0ca40ac152bb4bf46a3c", "AspectOfTheWolfTripAbility" },
            { "1202b3d188c9bdc46987a5da168ec6d9", "TwoHandedFighterPiledriverTripAbility" },
            { "b789cfc41fa326f419d77efc2e5c6632", "TwoHandedFighterPiledriverBullRushAbility"}
        };

        internal static string tandemTripAssetId = "d26eb8ab2aabd0e45a4d7eec0340bbce";

        #endregion

        #region Icons

        internal static readonly Dictionary<CombatManeuver, string> combatManeuverActionIcons = new Dictionary<CombatManeuver, string>
        {
            { CombatManeuver.BullRush, "Icon_Bull_Rush.png" },
            { CombatManeuver.DirtyTrickBlind, "Icon_Dirty_Trick_Blinded.png" },
            { CombatManeuver.DirtyTrickEntangle, "Icon_Dirty_Trick_Entangled.png" },
            { CombatManeuver.DirtyTrickSickened, "Icon_Dirty_Trick_Sickened.png" },
            { CombatManeuver.Disarm, "Icon_Disarm.png" },
            { CombatManeuver.SunderArmor, "Icon_Sunder_Armor.png" }
        };

        internal static readonly Dictionary<CombatManeuver, string> improvedCombatManeuverFeatIcons = new Dictionary<CombatManeuver, string>
        {
            { CombatManeuver.Trip, "Icon_Trip_Improved.png" },
            { CombatManeuver.BullRush, "Icon_Bull_Rush_Improved.png" },
            { CombatManeuver.DirtyTrickBlind, "Icon_Dirty_Trick_Blinded_Improved.png" },
            { CombatManeuver.Disarm, "Icon_Disarm_Improved.png" },
            { CombatManeuver.SunderArmor, "Icon_Sunder_Armor_Improved.png" }
        };

        #endregion

            #region Localization

        internal static readonly Dictionary<CombatManeuver, string> newImprovedCombatManeuverFeatNameKeys = new Dictionary<CombatManeuver, string>
        {
            { CombatManeuver.Trip,  "44faed15-7bca-412e-b2f5-b5b236a0e6e0" }, // Improved Trip
            { CombatManeuver.BullRush, "be72a033-e8ac-4da7-a41b-66956ee6794d" }, // Improved Bull Rush
            { CombatManeuver.DirtyTrickBlind, "3a253c9c-bd8e-4f37-a839-af304a98c8ee" }, // Improved Dirty Trick
            { CombatManeuver.Disarm, "01bd80d6-f7f5-43e7-9a40-f381da17ab50" }, // Improved Disarm
            { CombatManeuver.SunderArmor, "6ace229a-d135-4ba2-abc2-364629dfcaf4" } // Improved Sunder Armor
        };

        internal static readonly Dictionary<string, string> newImprovedCombatManeuverFeatNames = new Dictionary<string, string>
        {
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.Trip], "Improved Trip" },
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.BullRush], "Improved Bull Rush" },
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.DirtyTrickBlind], "Improved Dirty Trick" },
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.Disarm], "Improved Disarm" },
            { newImprovedCombatManeuverFeatNameKeys[CombatManeuver.SunderArmor], "Improved Sunder Armor" }
        };

        internal static readonly Dictionary<CombatManeuver, string> updatedImprovedCombatManeuverFeatDescriptionKeys = new Dictionary<CombatManeuver, string>
        {
            { CombatManeuver.Trip, "294cdc6a-8d05-4977-b7ff-a1ac1d9a56be" }, // Improved Trip
            { CombatManeuver.BullRush, "d8ca5ba2-7c38-49ad-afc7-26899432c9aa" }, // Improved Bull Rush
            { CombatManeuver.DirtyTrickBlind, "f74caddc-78f8-468e-8af6-371f8ab3b377" }, // Improved Dirty Trick
            { CombatManeuver.Disarm, "13d9f747-9c43-49e1-a50d-fed7695e8581" }, // Improved Disarm
            { CombatManeuver.SunderArmor, "f5cda27a-2587-4978-bdba-74bb2beabe89" }, // Improved Sunder Armor
        };

        internal static readonly Dictionary<string, string> updatedImprovedCombatManeuverFeatDescriptions = new Dictionary<string, string>
        {
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.Trip], "You do not provoke an attack of opportunity when performing a trip combat maneuver. In addition, you receive a +2 bonus on checks made to trip a foe. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries to trip you." },
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.BullRush], "You do not provoke an attack of opportunity when performing a bull rush combat maneuver. In addition, you receive a +2 bonus on checks made to bull rush a foe. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries to bull rush you." },
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.DirtyTrickBlind], "You do not provoke an attack of opportunity when performing a dirty trick combat maneuver. In addition, you receive a +2 bonus on checks made to attempt a dirty trick. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries a dirty trick on you." },
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.Disarm], "You do not provoke an attack of opportunity when performing a disarm combat maneuver. In addition, you receive a +2 bonus on checks made to disarm a foe. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries to disarm you." },
            { updatedImprovedCombatManeuverFeatDescriptionKeys[CombatManeuver.SunderArmor], "You do not provoke an attack of opportunity when performing a sunder armor combat maneuver. In addition, you receive a +2 bonus on checks made to sunder the armor of a foe. You also receive a +2 bonus to your Combat Maneuver Defense whenever an opponent tries to sunder your armor." }
        };

        internal static readonly Dictionary<CombatManeuver, string> combatManeuverActionDisplayNameKeys = new Dictionary<CombatManeuver, string>
        {
            { CombatManeuver.Trip, "9283fcc1-ba37-4711-93ac-6de17415e10f" }, // Trip
            { CombatManeuver.BullRush, "be940667-072a-4835-8122-8f32a8337e27" }, // Bull Rush
            { CombatManeuver.DirtyTrickBlind, "5d23f3f4-77e8-480f-b53f-292ab2aa0238" }, // Dirty Trick - Blindness
            { CombatManeuver.DirtyTrickEntangle, "614bc7b4-be29-4480-a58d-2d4c1662563c" }, // Dirty Trick - Entangle
            { CombatManeuver.DirtyTrickSickened, "d8277055-b9f2-4395-911a-b8d09b9f95f7" }, // Dirty Trick - Sickened
            { CombatManeuver.Disarm,  "d7f35120-78b5-4656-b045-17b9b6b3635c" }, // Disarm
            { CombatManeuver.SunderArmor, "fd428ed5-beb2-4986-832b-5193f7424f9f" }  // Sunder
        };

        internal static readonly Dictionary<string, string> combatManeuverActionDisplayNames = new Dictionary<string, string>
        {
            { combatManeuverActionDisplayNameKeys[CombatManeuver.Trip],  "Trip"},
            { combatManeuverActionDisplayNameKeys[CombatManeuver.BullRush], "Bull Rush" },
            { combatManeuverActionDisplayNameKeys[CombatManeuver.DirtyTrickBlind], "Dirty Trick — Blinded" },
            { combatManeuverActionDisplayNameKeys[CombatManeuver.DirtyTrickEntangle], "Dirty Trick — Entangled" },
            { combatManeuverActionDisplayNameKeys[CombatManeuver.DirtyTrickSickened], "Dirty Trick — Sickened" },
            { combatManeuverActionDisplayNameKeys[CombatManeuver.Disarm], "Disarm" },
            { combatManeuverActionDisplayNameKeys[CombatManeuver.SunderArmor], "Sunder Armor" }
        };

        internal static readonly Dictionary<CombatManeuver, string> updatedCombatManeuverActionDescriptionKeys = new Dictionary<CombatManeuver, string>
        {
            { CombatManeuver.Trip, "882f2dbb-e005-4b83-bbe0-83aa3230e932" }, // Trip
            { CombatManeuver.BullRush, "0f9dde7c-42a4-4779-ae83-ac3ff7979f0f" }, // Bull Rush
            { CombatManeuver.DirtyTrickBlind, "0986ab45-3f84-4540-8b5e-ad6e6ab04942" }, // Dirty Trick - Blindness
            { CombatManeuver.DirtyTrickEntangle, "66cdeddf-e956-4d8c-9eba-d1b7c4616148" }, // Dirty Trick - Entangle
            { CombatManeuver.DirtyTrickSickened, "763ccc85-1cf0-4a83-a9c5-00d8cc85fced" }, // Dirty Trick - Sickened
            { CombatManeuver.Disarm,  "981b15a4-a022-4525-a1bb-a2d2a3e42ce4" }, // Disarm
            { CombatManeuver.SunderArmor, "93591497-f353-4f87-b80b-01d6efcf8b99" }  // Sunder
        };

        internal static readonly Dictionary<string, string> updatedCombatManeuverActionDescriptions = new Dictionary<string, string>
        {
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.Trip],  "You can attempt to trip your opponent in place of a melee attack. If you do not have the Improved Trip feat, or a similar ability, initiating a trip provokes an attack of opportunity from the target of your maneuver.\nIf your combat maneuver is successful, the target is knocked prone.\nIf the target has more than two legs, add +2 to the DC of the combat maneuver attack roll for each additional leg it has. Some creatures—such as oozes, creatures without legs, and flying creatures—cannot be tripped."},
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.BullRush], "A bull rush attempts to push an opponent straight back without doing any harm. If you do not have the Improved Bull Rush feat, or a similar ability, initiating a bull rush provokes an attack of opportunity from the target of your maneuver.\nIf your combat maneuver is successful, your target is pushed back 5 feet. For every 5 by which your attack exceeds your opponent's CMD, you can push the target back an additional 5 feet.\nAn enemy being moved by a bull rush does not provoke an attack of opportunity because of the movement unless you possess the Greater Bull Rush feat. You cannot bull rush a creature into a square that is occupied by a solid object or obstacle." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.DirtyTrickBlind], "You can attempt to hinder a foe in melee as a standard action. If you do not have the Improved Dirty Trick feat or a similar ability, attempting a dirty trick provokes an attack of opportunity from the target of your maneuver.\nIf your attack is successful, the target is blinded.\nThis condition lasts for 1 round. For every 5 by which your attack exceeds your opponent's CMD, the penalty lasts 1 additional round." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.DirtyTrickEntangle], "You can attempt to hinder a foe in melee as a standard action. If you do not have the Improved Dirty Trick feat or a similar ability, attempting a dirty trick provokes an attack of opportunity from the target of your maneuver.\nIf your attack is successful, the target is entangled.\nThis condition lasts for 1 round. For every 5 by which your attack exceeds your opponent's CMD, the penalty lasts 1 additional round." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.DirtyTrickSickened], "You can attempt to hinder a foe in melee as a standard action. If you do not have the Improved Dirty Trick feat or a similar ability, attempting a dirty trick provokes an attack of opportunity from the target of your maneuver.\nIf your attack is successful, the target is sickened.\nThis condition lasts for 1 round. For every 5 by which your attack exceeds your opponent's CMD, the penalty lasts 1 additional round." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.Disarm], "You can attempt to disarm a foe in place of a melee attack.  If you do not have the Improved Disarm feat, or a similar ability, attempting to disarm a foe provokes an attack of opportunity from the target of your maneuver.\nIf your combat maneuver is successful, the target cannot use his weapons for 1 round.\nFor every 5 by which your attack exceeds your opponent's CMD, the disarmed condition lasts 1 additional round." },
            { updatedCombatManeuverActionDescriptionKeys[CombatManeuver.SunderArmor], "You can attempt to dislodge a piece of armor worn by your opponent, in place of a melee attack. If you do not have the Improved Sunder feat, or a similar ability, attempting to sunder armor provokes an attack of opportunity from the target of your maneuver.\nIf your combat maneuver is successful, the target loses its bonuses from armor for 1 round.\nFor every 5 by which your attack exceeds your opponent's CMD, the penalty lasts 1 additional round." }
        };

        #endregion

        #region ToggleAbilitiesCache
        private static BlueprintActivatableAbility _tripToggle;
        public static BlueprintActivatableAbility TripToggle
        {
            get
            {
                if (_tripToggle == null)
                    _tripToggle = Main.Library.Get<BlueprintActivatableAbility>(TripToggleAbility.Data.Guid);
                return _tripToggle;
            }
        }

        private static BlueprintActivatableAbility _disarmToggle;
        public static BlueprintActivatableAbility DisarmToggle
        {
            get
            {
                if (_disarmToggle == null)
                    _disarmToggle = Main.Library.Get<BlueprintActivatableAbility>(DisarmToggleAbility.Data.Guid);
                return _disarmToggle;
            }
        }

        private static BlueprintActivatableAbility _sunderArmorToggle;
        public static BlueprintActivatableAbility SunderArmorToggle
        {
            get
            {
                if (_sunderArmorToggle == null)
                    _sunderArmorToggle = Main.Library.Get<BlueprintActivatableAbility>(SunderArmorToggleAbility.Data.Guid);
                return _sunderArmorToggle;
            }
        }
        #endregion
    }
}
