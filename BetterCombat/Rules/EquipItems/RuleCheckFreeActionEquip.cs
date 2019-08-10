using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem;
using System.Collections.Generic;

namespace BetterCombat.Rules.EquipItems
{
    public static class UnitFreeEquipmentChangeExtension
    {
        private static Dictionary<UnitEntityData, bool> _unitsFreeActionEquip = new Dictionary<UnitEntityData, bool>();

        public static void SetFreeEquipmentChange(this UnitEntityData unit, bool freeEquip)
        {
            _unitsFreeActionEquip[unit] = freeEquip;
        }

        public static bool IsFreeEquipmentChange(this UnitEntityData unit)
        {
            bool freeEquipmentChange;
            return _unitsFreeActionEquip.TryGetValue(unit, out freeEquipmentChange) && freeEquipmentChange;
        }
    }

    public class RuleCheckFreeActionEquip : RulebookEvent
    {

        public bool IsHandsEquipmentSetChange;

        public RuleCheckFreeActionEquip(UnitEntityData initiator) 
            : base(initiator)
        {
            IsFreeAction = false;
        }

        public bool IsFreeAction { get; set; }

        public override void OnTrigger(RulebookEventContext context)
        {
        }
    }
}
