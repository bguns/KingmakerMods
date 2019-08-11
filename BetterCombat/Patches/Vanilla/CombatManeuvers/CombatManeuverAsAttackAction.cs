using BetterCombat.Data;
using BetterCombat.Helpers;
using BetterCombat.NewAbilities.CombatManeuvers;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UI.CombatText;
using Kingmaker.UI.Overtip;
using Kingmaker.UI.SettingsUI;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Mechanics;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{

    public static class CombatManeuverAsAttackUnitExtension
    {
        private static BlueprintActivatableAbility _tripToggle;
        private static BlueprintActivatableAbility TripToggle
        {
            get
            {
                if (_tripToggle == null)
                    _tripToggle = Main.Library.Get<BlueprintActivatableAbility>(TripToggleAbility.Data.Guid);
                return _tripToggle;
            }
        }

        private static BlueprintActivatableAbility _disarmToggle;
        private static BlueprintActivatableAbility DisarmToggle
        {
            get
            {
                if (_disarmToggle == null)
                    _disarmToggle = Main.Library.Get<BlueprintActivatableAbility>(DisarmToggleAbility.Data.Guid);
                return _disarmToggle;
            }
        }

        private static BlueprintActivatableAbility _sunderArmorToggle;
        private static BlueprintActivatableAbility SunderArmorToggle
        {
            get
            {
                if (_sunderArmorToggle == null)
                    _sunderArmorToggle = Main.Library.Get<BlueprintActivatableAbility>(SunderArmorToggleAbility.Data.Guid);
                return _sunderArmorToggle;
            }
        }

        public static CombatManeuver GetActiveCombatManeuverToggle(this UnitEntityData unit)
        {
            var combatManeuver = CombatManeuver.None;

            var tripToggle = unit.Descriptor.GetFact(TripToggle) as ActivatableAbility;
            var disarmToggle = unit.Descriptor.GetFact(DisarmToggle) as ActivatableAbility;
            var sunderArmorToggle = unit.Descriptor.GetFact(SunderArmorToggle) as ActivatableAbility;

            if (tripToggle.IsOn)
                combatManeuver = CombatManeuver.Trip;
            else if (disarmToggle.IsOn)
                combatManeuver = CombatManeuver.Disarm;
            else if (sunderArmorToggle.IsOn)
                combatManeuver = CombatManeuver.SunderArmor;

            return combatManeuver;
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitAttack), "TriggerAttackRule", Harmony12.MethodType.Normal)]
    static class UnitAttack_TriggerAttackRule_CombatManeuverPatch
    {
        private static FastSetter unitattack_set_LastAttackRule = Harmony.CreateSetter<UnitAttack>(nameof(UnitAttack.LastAttackRule));
        private static FastGetter unitattack_get_m_AllAttacks = Harmony.CreateFieldGetter<UnitAttack>("m_AllAttacks");
        private static FastGetter overtipManager_get_m_UnitControllers = Harmony.CreateFieldGetter<OvertipManager>("m_UnitControllers");    
       
        static bool Prefix(UnitAttack __instance, AttackHandInfo attack)
        {
            if (!attack.Weapon.Blueprint.IsMelee)
                return true;


            var combatManeuver = __instance.Executor.GetActiveCombatManeuverToggle();

            if (combatManeuver == CombatManeuver.None)
                return true;


            var lastAttackRule = new RuleAttackWithWeapon(__instance.Executor, __instance.Target, attack.Weapon, attack.AttackBonusPenalty)
            {
                IsRend = __instance.IsRend(__instance.PlannedAttack),
                IsFirstAttack = (attack.AttackNumber == 0),
                IsFullAttack = __instance.IsFullAttack,
                IsCharge = __instance.IsCharge,
                AttackNumber = attack.AttackNumber,
                AttacksCount = ((List<AttackHandInfo>)unitattack_get_m_AllAttacks(__instance)).Count
            };

            unitattack_set_LastAttackRule(__instance, lastAttackRule);

            attack.Target = __instance.Target;

            var combatManeuverRule = new RuleCombatManeuver(__instance.Executor, __instance.Target, combatManeuver);
            combatManeuverRule.ReplaceAttackBonus = new int?(__instance.Executor.Stats.BaseAttackBonus + attack.AttackBonusPenalty);

            BlueprintAbility combatManeuverAbility = Main.Library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[combatManeuver]);
            var abilityData = new Kingmaker.UnitLogic.Abilities.AbilityData(combatManeuverAbility, __instance.Executor.Descriptor);
            var context = abilityData.CreateExecutionContext(__instance.Target);

            Game.Instance.UI.BattleLogManager.HandleUseAbility(abilityData, null);

            var combatTextManager = ((Dictionary<EntityDataBase, OvertipController>)overtipManager_get_m_UnitControllers(Game.Instance.UI.BarkManager))[__instance.Executor].CombatText;
            MethodInfo settingsForbidsMethod = combatTextManager.GetType().GetMethod("SettingForbids", BindingFlags.NonPublic | BindingFlags.Instance);
            bool forbids = (bool)settingsForbidsMethod.Invoke(combatTextManager, new object[] { SettingsRoot.Instance.ShowSpellNameInCombatText });
            if (!forbids)
                combatTextManager.AddCombatText(combatManeuverAbility.Name, combatTextManager.DefaultColor, true, combatManeuverAbility.Icon);
            
            attack.IsHit = context.TriggerRule(combatManeuverRule).Success;

            return false;
        }
    }
}
