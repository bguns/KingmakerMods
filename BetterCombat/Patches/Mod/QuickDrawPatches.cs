using BetterCombat.Patches.Mod.QuickDraw;
using System;

namespace BetterCombat.Patches.Mod
{
    internal class QuickDrawPatches
    {
        internal static Type[] AllPatches =
        {
            typeof(UnitBody_set_CurrentHandEquipmentSetIndex_QuickDrawPatch),
            typeof(UnitViewHandsEquipment_HandleEquipmentSetChanged_QuickDrawPatch),
            typeof(UnitViewHandsEquipment_HandleEquipmentSlotUpdated_QuickDrawPatch)
#if DEBUG
            ,typeof(UnitViewHandsEquipment_StartCombatChangeAnimation__LoggerPatch),
            typeof(UnitViewHandsEquipment_UpdateActiveWeaponSetImmediately_LoggerPatch),
            typeof(UnitViewHandsEquimpent_ChangeEquipmentWithoutAnimation_LoggerPatch)
#endif
        };
    }
}
