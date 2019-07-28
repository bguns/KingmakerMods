using BetterCombat.Helpers;
using BetterCombat.Rules;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.SoftCover
{
    [Harmony12.HarmonyPatch(typeof(RuleCalculateAC), nameof(RuleCalculateAC.OnTrigger), Harmony12.MethodType.Normal)]
    class RuleCalculateAC_SoftCover_Patch
    {
        [Harmony12.HarmonyPrefix]
        static bool Prefix(RuleCalculateAC __instance, RulebookEventContext context)
        {
            var softCoverUnitFact = Helpers.Library.Create<BlueprintUnitFact>();
            var softCoverPartialUnitFact = Helpers.Library.Create<BlueprintUnitFact>();
            softCoverUnitFact.SetName(Localization.CreateString(SoftCoverData.SoftCoverNameKey, SoftCoverData.SoftCoverName));
            softCoverPartialUnitFact.SetName(Localization.CreateString(SoftCoverData.SoftCoverPartialNameKey, SoftCoverData.SoftCoverPartialName));
            Main.Logger?.Write("SoftCover prefix patch triggered");
            Cover cover = Rulebook.Trigger(new RuleCheckSoftCover(__instance.Initiator, __instance.Target)).Result;
            if (cover == Cover.Full)
                //__instance.AddTemporaryModifier(__instance.Target.Stats.AC.AddModifier(4, null, ModifierDescriptor.Other));
                __instance.AddBonus(4, new Fact(softCoverUnitFact));
            else if (cover == Cover.Partial)
                //__instance.AddTemporaryModifier(__instance.Target.Stats.AC.AddModifier(2, null, ModifierDescriptor.Other));
                __instance.AddBonus(2, new Fact(softCoverPartialUnitFact));
                Main.Logger?.Write($"SoftCover calculated - result = {cover.ToString()}");
            return true;
        }
    }

    /*[Harmony12.HarmonyPatch(typeof(UnitDescriptor), nameof(UnitDescriptor.Initialize), Harmony12.MethodType.Normal)]
    class UnitDescriptor_AddSoftCoverFact_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix(UnitDescriptor __instance)
        {
            var softCoverFact = Helpers.Library.Create<ACBonusSoftCover>();
            __instance.AddFact(softCoverFact);
        }
    }*/
}
