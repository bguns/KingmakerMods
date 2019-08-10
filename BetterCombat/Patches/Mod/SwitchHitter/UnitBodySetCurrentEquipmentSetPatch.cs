using BetterCombat.Rules.EquipItems;
using Kingmaker.Items;
using Kingmaker.RuleSystem;

namespace BetterCombat.Patches.Mod.SwitchHitter
{
    [Harmony12.HarmonyPatch(typeof(UnitBody), nameof(UnitBody.CurrentHandEquipmentSetIndex), Harmony12.MethodType.Setter)]
    class UnitBody_set_CurrentHandEquipmentSetIndex_FreeActionPatch
    {

        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitBody __instance)
        {
            __instance.Owner.Unit.SetFreeEquipmentChange(Rulebook.Trigger(new RuleCheckFreeActionEquip(__instance.Owner.Unit) { IsHandsEquipmentSetChange = true }).IsFreeAction);
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitBody __instance)
        {
            __instance.Owner.Unit.SetFreeEquipmentChange(false);
        }
    }
}
