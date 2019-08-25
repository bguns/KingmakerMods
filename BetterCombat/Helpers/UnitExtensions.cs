using BetterCombat.Data;
using BetterCombat.NewAbilities.CombatManeuvers;
using BetterCombat.Rules.EquipItems;
using Kingmaker;
using Kingmaker.Blueprints.Root;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.Items.Slots;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.UI.CombatText;
using Kingmaker.UI.Overtip;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.Utility;
using Kingmaker.View.MapObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterCombat.Helpers
{
    public static class UnitExtensions
    {
        public static void DropCurrentWeaponSet(this UnitBody unitBody)
        {
            unitBody.Owner.Unit.SetFreeEquipmentChange(true);

            HandsEquipmentSet equipmentSet = unitBody.CurrentHandsEquipmentSet;
            if (!equipmentSet.PrimaryHand.HasItem && !equipmentSet.SecondaryHand.HasItem)
                return;

            DroppedLoot droppedLoot = UnityEngine.Object.FindObjectsOfType<DroppedLoot>().FirstOrDefault(o => unitBody.Owner.Unit.DistanceTo(o.transform.position) < 5.Feet().Meters);
            if (!droppedLoot)
            {
                droppedLoot = Game.Instance.EntityCreator.SpawnEntityView<DroppedLoot>(BlueprintRoot.Instance.Prefabs.DroppedLootBag, unitBody.Owner.Unit.Position, unitBody.Owner.Unit.View.transform.rotation,
                    Game.Instance.State.LoadedAreaState.MainState);
                droppedLoot.Loot = new ItemsCollection();
            }

            if (equipmentSet.PrimaryHand.CanDrop())
            {
                ItemEntity primary = equipmentSet.PrimaryHand.Item;
                primary.Collection.Transfer(primary, droppedLoot.Loot);
            }
            if (equipmentSet.SecondaryHand.CanDrop())
            {
                ItemEntity secondary = equipmentSet.SecondaryHand.Item;
                secondary.Collection.Transfer(secondary, droppedLoot.Loot);
            }

            unitBody.Owner.Unit.SetFreeEquipmentChange(false);
        }

        public static bool CanDrop(this HandSlot hand)
        {
            return hand.HasItem && !hand.Item.IsNonRemovable && (!hand.HasWeapon || !hand.Weapon.Blueprint.IsNatural);
        }

        private static FastGetter overtipManager_get_m_UnitControllers = Harmony.CreateFieldGetter<OvertipManager>("m_UnitControllers");

        public static CombatTextManager GetCombatTextManager(this UnitEntityData unit)
        {
            try
            {
                return ((Dictionary<EntityDataBase, OvertipController>)overtipManager_get_m_UnitControllers(Game.Instance.UI.BarkManager))[unit].CombatText;
            }
            catch (KeyNotFoundException)
            {
                Main.Logger?.Error($"GetCombatTextManager: unit {unit.CharacterName} ({unit.UniqueId}) not in Game.Instance.UI.BarkManager.m_UnitControllers");
                return null;
            }
            catch (ArgumentNullException)
            {
                Main.Logger?.Error($"GetCombatTextManager: unit is null");
                return null;
            }
        }

        #region CombatManeuvers
        
        public static CombatManeuver GetActiveCombatManeuverToggle(this UnitEntityData unit)
        {
            var combatManeuver = CombatManeuver.None;

            var tripToggle = unit.Descriptor.GetFact(CombatManeuverData.TripToggle) as ActivatableAbility;
            var disarmToggle = unit.Descriptor.GetFact(CombatManeuverData.DisarmToggle) as ActivatableAbility;
            var sunderArmorToggle = unit.Descriptor.GetFact(CombatManeuverData.SunderArmorToggle) as ActivatableAbility;

            if (tripToggle.IsOn)
                combatManeuver = CombatManeuver.Trip;
            else if (disarmToggle.IsOn)
                combatManeuver = CombatManeuver.Disarm;
            else if (sunderArmorToggle.IsOn)
                combatManeuver = CombatManeuver.SunderArmor;

            return combatManeuver;
        }

        public static bool TriggerAttackReplacementCombatManeuver(this UnitEntityData initiator, UnitEntityData target, ItemEntityWeapon weapon, int attackBonusPenalty, CombatManeuver combatManeuver)
        {
            var combatManeuverRule = new RuleCombatManeuver(initiator, target, combatManeuver);
            combatManeuverRule.ReplaceAttackBonus = new int?(initiator.Stats.BaseAttackBonus + attackBonusPenalty);

            BlueprintAbility combatManeuverAbility = Main.Library.Get<BlueprintAbility>(CombatManeuverData.combatManeuverActionIds[combatManeuver]);
            var abilityData = new Kingmaker.UnitLogic.Abilities.AbilityData(combatManeuverAbility, initiator.Descriptor);
            var context = abilityData.CreateExecutionContext(target);

            Game.Instance.UI.BattleLogManager.HandleUseAbility(abilityData, null);

            var combatTextManager = initiator.GetCombatTextManager();
            if (combatTextManager != null)
            {
                /*MethodInfo settingsForbidsMethod = combatTextManager.GetType().GetMethod("SettingForbids", BindingFlags.NonPublic | BindingFlags.Instance);
                bool forbids = (bool)settingsForbidsMethod.Invoke(combatTextManager, new object[] { SettingsRoot.Instance.ShowSpellNameInCombatText });
                if (!forbids)
                    combatTextManager.AddCombatText(combatManeuverAbility.Name, combatTextManager.DefaultColor, true, combatManeuverAbility.Icon);*/

                var mockSpellRule = new RuleCastSpell(abilityData, target);
                combatTextManager.OnEventDidTrigger(mockSpellRule);
            }

            return context.TriggerRule(combatManeuverRule).Success;
        }
        #endregion
    }
}
