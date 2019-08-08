using BetterCombat.Helpers;
using BetterCombat.NewFeats;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Items;
using Kingmaker.UnitLogic;

namespace BetterCombat.Patches.Mod.QuickDraw
{
    [Harmony12.HarmonyPatch(typeof(UnitBody), nameof(UnitBody.CurrentHandEquipmentSetIndex), Harmony12.MethodType.Setter)]
    class UnitBody_set_CurrentHandEquipmentSetIndex_QuickDrawPatch
    {

        static BlueprintFeature quickDraw;

        internal static bool shouldQuickDraw = false;

        [Harmony12.HarmonyPrefix]
        static bool Prefix(UnitBody __instance)
        {
            if (!Main.Settings.UseQuickDraw)
            {
                Main.Logger?.Write("Set CurrentEquipmentSetIndex: quickdraw is disabled in mod settings");
                return true;
            }

            if (quickDraw == null)
            {
                quickDraw = Main.Library.Get<BlueprintFeature>(QuickDrawFeat.Data.Guid);
            }

            Main.Logger?.Append("Set CurrentEquipmentSetIndex prefix triggered.");
            Main.Logger?.Append($" - CurrentHandsEquipmentSet.IsEmpty(): {__instance.CurrentHandsEquipmentSet.IsEmpty()}");
            Main.Logger?.Append($" - Unit has quickdraw: {__instance.Owner.Unit.Descriptor.HasFact(quickDraw)}");
            if (__instance.CurrentHandsEquipmentSet.IsEmpty() && __instance.Owner.Unit.Descriptor.HasFact(quickDraw))
            {
                Main.Logger?.Append($" - Setting quick draw flag to true...");
                shouldQuickDraw = true;
            }
            Main.Logger?.Flush();
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitBody __instance)
        {
            if (!Main.Settings.UseQuickDraw)
                return;

            Main.Logger?.Append("Set CurrentEquipmentSetIndex postfix triggered.");
            Main.Logger?.Append(" - Setting quick draw flag to false...");
            shouldQuickDraw = false;
            Main.Logger?.Flush();
        }
    }
}
