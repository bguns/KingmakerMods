using Kingmaker.View.Equipment;

namespace BetterCombat.Patches.Mod.SwitchHitter
{
    [Harmony12.HarmonyPatch(typeof(UnitViewHandsEquipment), "UpdateActiveWeaponSetImmediately", Harmony12.MethodType.Normal)]
    class UnitViewHandsEquipment_UpdateActiveWeaponSetImmediately_LoggerPatch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitViewHandsEquipment __instance)
        {
            Main.Logger?.Append("UpdateActiveWeaponSetImmediately triggered.");
            Main.Logger?.Append($" - Standard action cooldown: {__instance.Owner.CombatState.Cooldown.StandardAction}");
            Main.Logger?.Flush();
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitViewHandsEquipment __instance)
        {
            Main.Logger?.Append("UpdateActiveWeaponSetImmediately done.");
            Main.Logger?.Append($" - Standard action cooldown: {__instance.Owner.CombatState.Cooldown.StandardAction}");
            Main.Logger?.Flush();
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitViewHandsEquipment), nameof(UnitViewHandsEquipment.StartCombatChangeAnimation), Harmony12.MethodType.Normal)]
    class UnitViewHandsEquipment_StartCombatChangeAnimation__LoggerPatch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitViewHandsEquipment __instance)
        {
            Main.Logger?.Append("StartCombatChangeAnimation triggered.");
            Main.Logger?.Append($" - Standard action cooldown: {__instance.Owner.CombatState.Cooldown.StandardAction}");
            Main.Logger?.Flush();
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitViewHandsEquipment __instance)
        {
            Main.Logger?.Append("StartCombatChangeAnimation done.");
            Main.Logger?.Append($" - Standard action cooldown: {__instance.Owner.CombatState.Cooldown.StandardAction}");
            Main.Logger?.Append(" - note: this method starts a coroutine, so cooldown might be changed after method finish.");
            Main.Logger?.Flush();
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitViewHandsEquipment), "ChangeEquipmentWithoutAnimation", Harmony12.MethodType.Normal)]
    class UnitViewHandsEquimpent_ChangeEquipmentWithoutAnimation_LoggerPatch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitViewHandsEquipment __instance)
        {
            Main.Logger?.Write("ChangeEquipmentWithoutAnimation triggered...");
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitViewHandsEquipment __instance)
        {
            Main.Logger?.Write("ChangeEquipmentWithoutAnimation finished...");
        }
    }
}
