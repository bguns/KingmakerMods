﻿using System;
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
            typeof(UnitDescriptor_AddCombatManeuversStandardAbilityPostLoad_Patch)
        };
    }
}
