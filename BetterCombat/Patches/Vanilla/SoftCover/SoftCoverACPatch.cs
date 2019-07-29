using BetterCombat.Helpers;
using BetterCombat.Rules;
using Kingmaker.Blueprints.Facts;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;

namespace BetterCombat.Patches.Vanilla.SoftCover
{
    [Harmony12.HarmonyPatch(typeof(RuleCalculateAC), nameof(RuleCalculateAC.OnTrigger), Harmony12.MethodType.Normal)]
    class RuleCalculateAC_SoftCover_Patch
    {
        private static Fact _softCoverFact;
        private static Fact _softCoverPartialFact;

        private static Fact SoftCoverFact
        {
            get
            {
                if (_softCoverFact == null)
                {
                    var softCoverUnitFact = Library.Create<BlueprintUnitFact>();
                    softCoverUnitFact.SetName(Localization.CreateString(SoftCoverData.SoftCoverNameKey, SoftCoverData.SoftCoverName));
                    _softCoverFact = new Fact(softCoverUnitFact);
                }
                return _softCoverFact;
            }
        }

        private static Fact SoftCoverPartialFact
        {
            get
            {
                if (_softCoverPartialFact == null)
                {
                    var softCoverPartialUnitFact = Library.Create<BlueprintUnitFact>();
                    softCoverPartialUnitFact.SetName(Localization.CreateString(SoftCoverData.SoftCoverPartialNameKey, SoftCoverData.SoftCoverPartialName));
                    _softCoverPartialFact = new Fact(softCoverPartialUnitFact);
                }
                return _softCoverPartialFact;
            }
        }

        [Harmony12.HarmonyPrefix]
        static bool Prefix(RuleCalculateAC __instance, RulebookEventContext context)
        {
            Main.Logger?.Write("SoftCover prefix patch triggered");
            Cover cover = Rulebook.Trigger(new RuleCheckSoftCover(__instance.Initiator, __instance.Target, __instance.AttackType)).Result;
            if (cover == Cover.Full)
                __instance.AddBonus(4, SoftCoverFact);
            else if (cover == Cover.Partial)
                __instance.AddBonus(2, SoftCoverPartialFact);
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
