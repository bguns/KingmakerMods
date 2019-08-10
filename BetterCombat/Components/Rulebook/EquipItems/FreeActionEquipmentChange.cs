using BetterCombat.Rules.EquipItems;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.UnitLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Components.Rulebook.EquipItems
{
    [AllowedOn(typeof(BlueprintUnitFact))]
    public class FreeActionEquipmentChange : RuleInitiatorLogicComponent<RuleCheckFreeActionEquip>
    {
        public BlueprintUnitFact MustHaveFact;

        public bool HandsEquipmentSetChangeOnly;

        public bool EmptyHandsOnly;

        public override void OnEventAboutToTrigger(RuleCheckFreeActionEquip evt)
        {
            bool freeEquipmentChange = true;

            if (MustHaveFact != null && !evt.Initiator.Descriptor.HasFact(MustHaveFact))
                freeEquipmentChange = false;

            if (HandsEquipmentSetChangeOnly && !evt.IsHandsEquipmentSetChange)
                freeEquipmentChange = false;

            if (EmptyHandsOnly && !evt.Initiator.Body.CurrentHandsEquipmentSet.IsEmpty())
                freeEquipmentChange = false;

            evt.IsFreeAction = freeEquipmentChange;
        }

        public override void OnEventDidTrigger(RuleCheckFreeActionEquip evt)
        {
        }
    }
}
