using Kingmaker;
using System;
using UnityModManagerNet;
using UnityEngine;
using BetterCombat.Patches.Vanilla;
using Kingmaker.Blueprints;
using BetterCombat.Patches.Vanilla.AttackOfOpportunity;
using BetterCombat.Patches.Mod;
using BetterCombat.Patches.Compatibility.CallOfTheWild;
using BetterCombat.Patches.Compatibility.EldritchArcana;
using BetterCombat.NewFeats;
using BetterCombat.NewAbilities;
using BetterCombat.Patches.Vanilla.CombatManeuvers;

namespace BetterCombat
{
    public class Main
    {

        public static bool Enabled;

        internal static Log Logger;

        internal static Settings Settings;

        static string testedGameVersion = "2.0.6";

        internal static HarmonyPatcher HarmonyPatcher;

        internal static LibraryScriptableObject Library;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Logger = new Log(modEntry.Logger);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            HarmonyPatcher = new HarmonyPatcher(Harmony12.HarmonyInstance.Create(modEntry.Info.Id));

            if (!HarmonyPatcher.ApplyPatch(typeof(LibraryScriptableObject_LoadDictionary_Patch), "Library patch"))
            {
                // If we can't patch this, nothing will work, so want the mod to turn red in UMM.
                throw Error("Failed to patch LibraryScriptableObject.LoadDictionary(), cannot load mod");
            }

            Logger.Append("Patching...");
            if (HarmonyPatcher.ApplyPatches(FlankingPatches.AllPatches, "Flanking patches"))
            {
                Logger.Append("Flanking patches: OK");
            }

            if (HarmonyPatcher.ApplyPatch(typeof(UnitCombatState_AttackOfOpportunity_Patch), "AoO Hard Patch"))
            {
                Logger.Append("Instant AoO patch: OK");
            }

            if (HarmonyPatcher.ApplyPatches(CombatManeuverPatches.AllPatches, "Combat maneuver patches"))
            {
                Logger.Append("Combat Maneuver patches: OK");
            }

            if (HarmonyPatcher.ApplyPatches(SoftCoverPatches.AllPatches, "Soft cover patch"))
            {
                Logger.Append("Soft cover patches: OK");
            }

            if (HarmonyPatcher.ApplyPatches(SwitchHitterPatches.AllPatches, "Switch Hitter patch"))
            {
                Logger.Append("Switch Hitter patch: OK");
            }

            if (HarmonyPatcher.ApplyPatches(NewFeatsPatches.AllPatches, "New feats patches"))
            {
                Logger.Append("New feats patches: OK");
            }

            if (HarmonyPatcher.ApplyPatches(NewAbilitiesPatches.AllPatches, "New abilities patches"))
            {
                Logger.Append("New abilities patches: OK");
            }

            if (HarmonyPatcher.ApplyPatch(typeof(UnitAttack_TriggerAttackRule_CombatManeuverPatch), "Combat manuever as attack"))
            {
                Logger.Append("Combat Maneuvers as attack actions patch: OK");
            }

            // Compatibility
            if (HarmonyPatcher.ApplyPatches(CallOfTheWildPatches.AllPatches, "Call of the Wild compatibility"))
            {
                Logger.Append("Compatibility - Call of the Wild: OK");
            }

            if (HarmonyPatcher.ApplyPatches(EldritchArcanaPatches.AllPatches, "Eldritch Arcana compatibility"))
            {
                Logger.Append("Compatibility - Eldritch Arcana: OK");
            }

            Logger.Flush();

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!Enabled)
                return;

            var fixedWidth = new GUILayoutOption[1] { GUILayout.ExpandWidth(false) };
            if (testedGameVersion != GameVersion.GetVersion())
            {
                GUILayout.Label($"<b>This mod was tested against game version {testedGameVersion}, but you are running {GameVersion.GetVersion()}.</b>", fixedWidth);
            }

            if (HarmonyPatcher == null)
            {
                GUILayout.Label($"<b>Error: failed to instantiate Harmony patcher.</b>");
            }
            if (HarmonyPatcher.FailedPatches.Count > 0)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("<b>Error: Some patches failed to apply. These features may not work:</b>", fixedWidth);
                foreach (var featureName in HarmonyPatcher.FailedPatches)
                {
                    GUILayout.Label($"  • <b>{featureName}</b>", fixedWidth);
                }
                GUILayout.EndVertical();
            }
            if (HarmonyPatcher.FailedLoading.Count > 0)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("<b>Error: Some assets failed to load. Saves using these features won't work:</b>", fixedWidth);
                foreach (var featureName in HarmonyPatcher.FailedLoading)
                {
                    GUILayout.Label($"  • <b>{featureName}</b>", fixedWidth);
                }
                GUILayout.EndVertical();
            }

            Settings.UseSoftCover = GUILayout.Toggle(Settings.UseSoftCover, "Use Soft Cover rules", fixedWidth);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }

        internal static void SafeLoad(Action load, String name)
        {
            try
            {
                load();
            }
            catch (Exception e)
            {
                HarmonyPatcher?.FailedLoading.Add(name);
                Logger.Error(e);
            }
        }

        internal static T SafeLoad<T>(Func<T> load, String name)
        {
            try
            {
                return load();
            }
            catch (Exception e)
            {
                HarmonyPatcher?.FailedLoading.Add(name);
                Logger.Error(e);
                return default(T);
            }
        }

        internal static Exception Error(String message)
        {
            Logger?.Error(message);
            return new InvalidOperationException(message);
        }
    }
}
