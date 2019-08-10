using BetterCombat.Helpers;
using BetterCombat.Patches.Mod.QuickDraw;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Root;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items;
using Kingmaker.Localization;
using Kingmaker.Localization.Shared;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using Kingmaker.View.MapObjects;
using System;
using System.Linq;

namespace BetterCombat.NewActions
{

    public class DropWeapons : ContextAction
    {
        public override string GetCaption()
        {
            return "Drop Weapons action";
        }

        public override void RunAction()
        {
            MechanicsContext.Data data = ElementsContext.GetData<MechanicsContext.Data>();
            MechanicsContext mechanicsContext = (data != null) ? data.Context : null;
            if (mechanicsContext == null)
            {
                Main.Logger?.Error("Unable to drop weapons: no context found");
                return;
            }

            UnitEntityData unit = mechanicsContext.MaybeCaster;

            if (unit == null)
            {
                Main.Logger?.Error("Unable to drop weapons: caster is null");
            }

            unit.Descriptor.Body.DropCurrentWeaponSet();
        }
    }

    public static class DropHeldWeapons
    {

        public static void DropCurrentWeaponSet(this UnitBody unitBody)
        {

            UnitBody_set_CurrentHandEquipmentSetIndex_QuickDrawPatch.shouldQuickDraw = true;

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

            if (equipmentSet.PrimaryHand.HasItem)
            {
                ItemEntity primary = equipmentSet.PrimaryHand.Item;
                primary.Collection.Transfer(primary, droppedLoot.Loot);
            }
            if (equipmentSet.SecondaryHand.HasItem)
            {
                ItemEntity secondary = equipmentSet.SecondaryHand.Item;
                secondary.Collection.Transfer(secondary, droppedLoot.Loot);
            }

            UnitBody_set_CurrentHandEquipmentSetIndex_QuickDrawPatch.shouldQuickDraw = false;
        }

        private static readonly FastSetter blueprintScriptableObject_set_AssetId = Harmony.CreateFieldSetter<BlueprintScriptableObject>("m_AssetGuid");
        static readonly FastSetter blueprintAbility_set_Description = Harmony.CreateFieldSetter<BlueprintAbility>("m_Description");
        static readonly FastSetter blueprintAbility_set_Icon = Harmony.CreateFieldSetter<BlueprintAbility>("m_Icon");
        static readonly FastSetter blueprintAbility_set_DisplayName = Harmony.CreateFieldSetter<BlueprintAbility>("m_DisplayName");
        static readonly FastSetter blueprintAbility_set_m_IsFullRoundAction = Harmony.CreateFieldSetter<BlueprintAbility>("m_IsFullRoundAction");

        internal static BlueprintAbility CreateDropWeaponsAbility()
        {
            Main.Logger?.Write("Creating Drop Weapon Ability...");
            var ability = Library.Create<BlueprintAbility>();
            ability.name = "DropWeaponsAction";
            ability.Range = AbilityRange.Personal;
            ability.Type = AbilityType.Special;
            ability.CanTargetSelf = true;
            ability.NeedEquipWeapons = true;
            ability.ActionType = UnitCommand.CommandType.Free;
            //ability.MaterialComponent = new BlueprintAbility.MaterialComponentData() { Count = 1 };
            ability.LocalizedDuration = new LocalizedString();
            ability.LocalizedSavingThrow = new LocalizedString();
            blueprintAbility_set_m_IsFullRoundAction(ability, true);

            Main.Logger?.Write("Drop Weapon ability: setting uuids...");
            blueprintScriptableObject_set_AssetId(ability, "47148ed4e2b747a0be47858716a33ae9");
            blueprintAbility_set_Description(ability, Localization.CreateString("e349ae28-95e3-4dd1-a4dc-87e5581c9f8c", "Drop the currently active weapon set to the ground"));
            blueprintAbility_set_DisplayName(ability, Localization.CreateString("b5674bdd-aef1-4104-a0f8-2a0f355afb81", "Drop Weapons"));

            Main.Logger?.Write("Drop weapon ability: setting icon...");
            var iconAbility = Main.Library.Get<BlueprintAbility>("45d94c6db453cfc4a9b99b72d6afe6f6");
            blueprintAbility_set_Icon(ability, iconAbility.Icon);

            Main.Logger?.Write("Drop Weapon ability: creating action...");
            var actions = Library.Create<AbilityEffectRunAction>();
            actions.Actions = new ActionList();
            var action = Library.Create<DropWeapons>();
            actions.Actions.Actions = actions.Actions.Actions.AddToArray(action);

            Main.Logger?.Write("Drop Weapon ability: adding action to components...");
            ability.AddComponent(actions);

            return ability;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class DropWeapons_AddAction_Patch
    {

        static bool initialized = false;

        [Harmony12.HarmonyPrefix]
        static bool Prefix(LibraryScriptableObject __instance)
        {
            initialized = __instance.Initialized();
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(LibraryScriptableObject __instance)
        {
            if (initialized)
                return;

            Main.Logger?.Write("Adding Low Profile feat...");
            var action = DropHeldWeapons.CreateDropWeaponsAbility();

            __instance.GetAllBlueprints().Add(action);
            __instance.BlueprintsByAssetId[action.AssetGuid] = action;
        }
    }

    [Harmony12.HarmonyPatch(typeof(LocalizationManager), "LoadPack", Harmony12.MethodType.Normal)]
    class DropWeapons_AddLocalization_Patch
    {
        [Harmony12.HarmonyPostfix]
        static void Postfix(Locale locale, ref LocalizationPack __result)
        {
            Localization.AddStringToLocalizationPack("b5674bdd-aef1-4104-a0f8-2a0f355afb81", "Drop Weapons", __result);
            Localization.AddStringToLocalizationPack("e349ae28-95e3-4dd1-a4dc-87e5581c9f8c", "Drop the currently active weapon set to the ground", __result);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddDropActionOnInitialize_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>("47148ed4e2b747a0be47858716a33ae9");
            if (action == null)
                return;
            __instance.AddFact(action);
        }
    }

    [Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.PostLoad), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddDropActionIfNotPresentPostLoad_Patch
    {
        static LibraryScriptableObject library = Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var action = library.Get<BlueprintAbility>("47148ed4e2b747a0be47858716a33ae9");
            if (action != null && !__instance.HasFact(action))
                __instance.AddFact(action);
        }
    }
}
