using BetterCombat.NewAbilities.CombatManeuvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.NewAbilities
{
    internal static class NewAbilitiesPatches
    {
        internal static Type[] AllPatches =
        {
            typeof(DropWeapons_AddLocalization_Patch),
            typeof(DropWeapons_AddAction_Patch),
            typeof(UnitDescriptor_AddDropActionOnInitialize_Patch),
            typeof(UnitDescriptor_AddDropActionIfNotPresentPostLoad_Patch),

            typeof(CombatManeuversStandardAbility_AddLocalization_Patch),
            typeof(CombatManeuversStandardAbility_AddAbility_Patch),
            typeof(UnitDescriptor_AddCombatManeuversStandardAbilityOnInitialize_Patch),
            typeof(UnitDescriptor_AddCombatManeuversStandardAbilityPostLoad_Patch),

            typeof(TripToggle_AddAbility_Patch),
            typeof(UnitDescriptor_AddTripToggleInitialize_Patch),
            typeof(UnitDescriptor_AddTripTogglePostLoad_Patch),

            typeof(DisarmToggle_AddAbility_Patch),
            typeof(UnitDescriptor_AddDisarmToggleInitialize_Patch),
            typeof(UnitDescriptor_AddDisarmTogglePostLoad_Patch),

            typeof(SunderArmorToggle_AddAbility_Patch),
            typeof(UnitDescriptor_AddSunderArmorToggleInitialize_Patch),
            typeof(UnitDescriptor_AddSunderArmorTogglePostLoad_Patch)
        };
    }
}
