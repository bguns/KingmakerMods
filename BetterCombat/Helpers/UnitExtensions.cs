using BetterCombat.Rules.EquipItems;
using Kingmaker;
using Kingmaker.Blueprints.Root;
using Kingmaker.Items;
using Kingmaker.Utility;
using Kingmaker.View.MapObjects;
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

            unitBody.Owner.Unit.SetFreeEquipmentChange(false);
        }
    }
}
