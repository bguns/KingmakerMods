using Kingmaker;
using System;
using UnityModManagerNet;
using UnityEngine;
using System.Collections.Generic;
using BetterCombat.Patches.Vanilla;
using Kingmaker.Blueprints;
using BetterCombat.Patches.Vanilla.CombatManeuvers;

namespace BetterCombat
{
    public class Main
    {

        public static bool Enabled;

        internal static Log Logger;

        internal static Settings settings;

        static string testedGameVersion = "2.0.6";

        internal static HarmonyPatcher HarmonyPatcher;

        internal static LibraryScriptableObject Library;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Logger = new Log(modEntry.Logger);
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
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

            if (HarmonyPatcher.ApplyPatch(typeof(UnitDescriptor_AddCombatManeuverActionsOnInitialize_Patch), "Combat Maneuver patches") 
                && HarmonyPatcher.ApplyPatch(typeof(Library_ModifyCombatManeuverFeats_Patch), "Combat Maneuver patches")
                && HarmonyPatcher.ApplyPatch(typeof(LocalizationManager_FixCombatManeuverFeatText_Patch), "Combat Maneuver text fix")) {
                Logger.Append("Combat Maneuver patches: OK");
            }

            //HarmonyPatcher.ApplyPatch(typeof(UnitCommands_InterruptAll_Patch), "Interrupt all patch");
            //HarmonyPatcher.ApplyPatch(typeof(UnitProneController_Tick_Patch), "Prone controller patch");
            HarmonyPatcher.ApplyPatch(typeof(UnitCombatState_AttackOfOpportunity_Patch), "AoO Hard Patch");
            HarmonyPatcher.ApplyPatch(typeof(RuleCombatManeuver_OnTrigger_AoO_Patch), "AoO Combat Maneuver fix");

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
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
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
