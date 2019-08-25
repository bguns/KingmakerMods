using BetterCombat.Helpers;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Items.Slots;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;

namespace BetterCombat.Components.Actions
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

            if (unit.Descriptor.IsPet || unit.Body.IsPolymorphed)
                return;

            var currentWeaponSet = unit.Body.CurrentHandsEquipmentSet;

            if (currentWeaponSet.PrimaryHand.CanDrop() || currentWeaponSet.SecondaryHand.CanDrop())
            {
                unit.Commands.InterruptAll();

                unit.Descriptor.Body.DropCurrentWeaponSet();
            }
        }
    }
}
