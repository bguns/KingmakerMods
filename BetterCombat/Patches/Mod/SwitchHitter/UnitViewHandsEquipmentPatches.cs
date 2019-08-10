using BetterCombat.Helpers;
using BetterCombat.Rules.EquipItems;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.View.Equipment;
using System.Reflection;

namespace BetterCombat.Patches.Mod.SwitchHitter
{
    [Harmony12.HarmonyPatch(typeof(UnitViewHandsEquipment), nameof(UnitViewHandsEquipment.HandleEquipmentSetChanged), Harmony12.MethodType.Normal)]
    class UnitViewHandsEquipment_HandleEquipmentSetChanged_FreeActionPatch
    {
        private static FastGetter unitViewHandsEquipment_get_Active = Harmony.CreateGetter<UnitViewHandsEquipment>("Active");

        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitViewHandsEquipment __instance)
        {
            if (!(bool)unitViewHandsEquipment_get_Active(__instance))
                return true;
            
            if (__instance.Owner.IsFreeEquipmentChange() && __instance.InCombat && (__instance.Owner.Descriptor.State.CanAct || __instance.IsDollRoom))
            {
                MethodInfo updateImmediately = __instance.GetType().GetMethod("UpdateActiveWeaponSetImmediately", BindingFlags.NonPublic | BindingFlags.Instance);
                updateImmediately.Invoke(__instance, new object[0]);
                return false;
            }

            return true;
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitViewHandsEquipment), nameof(UnitViewHandsEquipment.HandleEquipmentSlotUpdated), Harmony12.MethodType.Normal)]
    class UnitViewHandsEquipment_HandleEquipmentSlotUpdated_FreeActionPatch
    {

        private static FastGetter unitViewHandsEquipment_get_Active = Harmony.CreateGetter<UnitViewHandsEquipment>("Active");

        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitViewHandsEquipment __instance, HandSlot slot, ItemEntity previousItem)
        {
            MethodInfo getHandSlotMethod = __instance.GetType().GetMethod("GetSlotData", BindingFlags.NonPublic | BindingFlags.Instance);
            UnitViewHandSlotData slotDataCheck = (UnitViewHandSlotData)getHandSlotMethod.Invoke(__instance, new object[] { slot });

            if (!(bool)unitViewHandsEquipment_get_Active(__instance) || slotDataCheck == null)
                return true;
            
            if (__instance.Owner.IsFreeEquipmentChange() && __instance.InCombat && (__instance.Owner.Descriptor.State.CanAct || __instance.IsDollRoom) && slot.Active)
            {
                MethodInfo changeEquipmentWithoutAnimation = __instance.GetType().GetMethod("ChangeEquipmentWithoutAnimation", BindingFlags.NonPublic | BindingFlags.Instance);
                changeEquipmentWithoutAnimation.Invoke(__instance, new object[0]);
                return false;
            }

            return true;
        }
    }
}
