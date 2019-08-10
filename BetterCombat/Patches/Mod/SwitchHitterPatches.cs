using BetterCombat.Patches.Mod.SwitchHitter;
using System;

namespace BetterCombat.Patches.Mod
{
    internal class SwitchHitterPatches
    {
        internal static Type[] AllPatches =
        {
            typeof(UnitBody_set_CurrentHandEquipmentSetIndex_FreeActionPatch),
            typeof(UnitViewHandsEquipment_HandleEquipmentSetChanged_FreeActionPatch),
            typeof(UnitViewHandsEquipment_HandleEquipmentSlotUpdated_FreeActionPatch)
#if DEBUG
            ,typeof(UnitViewHandsEquipment_StartCombatChangeAnimation__LoggerPatch),
            typeof(UnitViewHandsEquipment_UpdateActiveWeaponSetImmediately_LoggerPatch),
            typeof(UnitViewHandsEquimpent_ChangeEquipmentWithoutAnimation_LoggerPatch)
#endif
        };
    }
}
