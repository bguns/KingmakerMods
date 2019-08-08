using BetterCombat.Helpers;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.View.Equipment;
using System.Reflection;

namespace BetterCombat.Patches.Mod.QuickDraw
{
    [Harmony12.HarmonyPatch(typeof(UnitViewHandsEquipment), nameof(UnitViewHandsEquipment.HandleEquipmentSetChanged), Harmony12.MethodType.Normal)]
    class UnitViewHandsEquipment_HandleEquipmentSetChanged_QuickDrawPatch
    {
        private static FastGetter unitViewHandsEquipment_get_Active = Harmony.CreateGetter<UnitViewHandsEquipment>("Active");

        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitViewHandsEquipment __instance)
        {
            if (!Main.Settings.UseQuickDraw)
            {
                Main.Logger?.Write("Set CurrentEquipmentSetIndex: quickdraw is disabled in mod settings");
                return true;
            }

            Main.Logger?.Append("UnitViewHandsEquipment.HandleEquipmentSetChanged prefix triggered");
            Main.Logger?.Append($"- quickDraw: {UnitBody_set_CurrentHandEquipmentSetIndex_QuickDrawPatch.shouldQuickDraw}");
            if (!(bool)unitViewHandsEquipment_get_Active(__instance))
            {
                Main.Logger?.Flush();
                return true;
            }
            if (UnitBody_set_CurrentHandEquipmentSetIndex_QuickDrawPatch.shouldQuickDraw && __instance.InCombat && (__instance.Owner.Descriptor.State.CanAct || __instance.IsDollRoom))
            {
                Main.Logger?.Append(" - quickDraw active, should now trigger UpdateActiveWeaponSetImmediately...");
                Main.Logger?.Flush();
                MethodInfo updateImmediately = __instance.GetType().GetMethod("UpdateActiveWeaponSetImmediately", BindingFlags.NonPublic | BindingFlags.Instance);
                updateImmediately.Invoke(__instance, new object[0]);
                return false;
            }
            Main.Logger?.Flush();
            return true;
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitViewHandsEquipment), nameof(UnitViewHandsEquipment.HandleEquipmentSlotUpdated), Harmony12.MethodType.Normal)]
    class UnitViewHandsEquipment_HandleEquipmentSlotUpdated_QuickDrawPatch
    {

        private static FastGetter unitViewHandsEquipment_get_Active = Harmony.CreateGetter<UnitViewHandsEquipment>("Active");

        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitViewHandsEquipment __instance, HandSlot slot, ItemEntity previousItem)
        {
            if (!Main.Settings.UseQuickDraw)
            {
                Main.Logger?.Write("Set CurrentEquipmentSetIndex: quickdraw is disabled in mod settings");
                return true;
            }

            Main.Logger?.Append("HandleEquipmentSlotUpdated triggered.");
            Main.Logger?.Append($" - HandSlot hasItem: {slot.HasItem}");
            Main.Logger?.Append($" - previousItem: {(previousItem == null ? null : previousItem.Blueprint.Name)}");
            Main.Logger?.Append($" - quickDraw: {UnitBody_set_CurrentHandEquipmentSetIndex_QuickDrawPatch.shouldQuickDraw}");

            MethodInfo getHandSlotMethod = __instance.GetType().GetMethod("GetSlotData", BindingFlags.NonPublic | BindingFlags.Instance);
            UnitViewHandSlotData slotDataCheck = (UnitViewHandSlotData)getHandSlotMethod.Invoke(__instance, new object[] { slot });

            if (!(bool)unitViewHandsEquipment_get_Active(__instance) || slotDataCheck == null)
            {
                Main.Logger?.Flush();
                return true;
            }

            if (UnitBody_set_CurrentHandEquipmentSetIndex_QuickDrawPatch.shouldQuickDraw && __instance.InCombat && (__instance.Owner.Descriptor.State.CanAct || __instance.IsDollRoom) && slot.Active)
            {
                Main.Logger?.Append(" - quickDraw active, should now trigger ChangeEquipmentWithoutAnimation...");
                Main.Logger?.Flush();
                MethodInfo changeEquipmentWithoutAnimation = __instance.GetType().GetMethod("ChangeEquipmentWithoutAnimation", BindingFlags.NonPublic | BindingFlags.Instance);
                changeEquipmentWithoutAnimation.Invoke(__instance, new object[0]);
                return false;
            }
            Main.Logger?.Flush();
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitViewHandsEquipment __instance, HandSlot slot, ItemEntity previousItem)
        {
            Main.Logger?.Write("HandleEquipmentSlotUpdated finished.");
        }
    }
}
