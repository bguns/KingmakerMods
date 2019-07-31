using BetterCombat.Helpers;
using BetterCombat.Patches.Vanilla.CombatManeuvers;
using BetterCombat.Rules;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Controllers.Combat;
using Kingmaker.Controllers.Units;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla
{
    static class CombatManeuverPatches
    {
        internal static Type[] AllPatches =
        {
            typeof(UnitDescriptor_AddCombatManeuverActionsOnInitialize_Patch),
            typeof(LocalizationManager_FixCombatManeuverFeatText_Patch),
            typeof(RuleCombatManeuver_OnTrigger_ProvokeAoO_Patch),
            typeof(Library_ModifyCombatManeuverFeats_Patch),
            typeof(ManeuverOnAttack_OnEventDidTrigger_NoAoO_Patch),
            typeof(LibraryScriptableObject_CombatManeuverContextActions_Patch)
        };
    }  
}
