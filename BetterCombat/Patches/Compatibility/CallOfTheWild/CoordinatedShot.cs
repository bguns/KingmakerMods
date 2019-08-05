using BetterCombat.Helpers;
using BetterCombat.Patches.Vanilla.Flanking;
using BetterCombat.Rules;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Facts;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using System;

namespace BetterCombat.Patches.Compatibility
{

    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class CoordinatedShotAttackBonus : RuleInitiatorLogicComponent<RuleCalculateAttackBonus>
    {
        public int AttackBonus = 1;
        public int AdditionalFlankBonus = 1;
        public BlueprintUnitFact CoordinatedShotFact;

        public static CoordinatedShotAttackBonus Create(BlueprintUnitFact coordinatedShotFact, int attackBonus = 1, int additionalFlankBonus = 1)
        {
            var instance = Library.Create<CoordinatedShotAttackBonus>();
            instance.AttackBonus = attackBonus;
            instance.AdditionalFlankBonus = additionalFlankBonus;
            instance.CoordinatedShotFact = coordinatedShotFact;
            return instance;
        }

        public override void OnEventAboutToTrigger(RuleCalculateAttackBonus evt)
        {
            if (!evt.Weapon.Blueprint.IsRanged)
                return;

            Func<UnitEntityData, UnitEntityData, UnitEntityData, bool> coordinatedShotFlankingPreconditions =
            (target, flankingPartner1, flankingPartner2) => this.Owner.State.Features.SoloTactics
                                                            || (flankingPartner1 != this.Owner.Unit && flankingPartner1.Descriptor.HasFact(this.CoordinatedShotFact))
                                                            || (flankingPartner2 != this.Owner.Unit && flankingPartner2.Descriptor.HasFact(this.CoordinatedShotFact));

            foreach(UnitEntityData unit in evt.Target.CombatState.EngagedBy)
            {
                if (evt.Target.IsFlankedByUnit(unit, coordinatedShotFlankingPreconditions))
                {
                    evt.AddBonus(AttackBonus + AdditionalFlankBonus, this.Fact);
                    return;
                }
            }

            if (this.Owner.State.Features.SoloTactics)
            {
                evt.AddBonus(AttackBonus, this.Fact);
                return;
            }

            foreach (UnitEntityData unitEntityData in evt.Target.CombatState.EngagedBy)
            {
                if (unitEntityData.Descriptor.HasFact(this.CoordinatedShotFact) && unitEntityData != this.Owner.Unit)
                {
                    evt.AddBonus(AttackBonus, this.Fact);
                    return;
                }
            }
        }

        public override void OnEventDidTrigger(RuleCalculateAttackBonus evt)
        {
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class Library_ModifyCoordinatedShotFeat_Patch
    {
        static string _coordinatedShotFeatId = "6c53965320604cfeb13d283afd147e09";

        static bool initialized = false;

        [Harmony12.HarmonyPrefix]
        [Harmony12.HarmonyPriority(Harmony12.Priority.Low)]
        static bool Prefix(LibraryScriptableObject __instance)
        {
            initialized = __instance.Initialized();
            return true;
        }

        [Harmony12.HarmonyPostfix]
        [Harmony12.HarmonyPriority(Harmony12.Priority.Low)]
        static void Postfix(LibraryScriptableObject __instance)
        {
            if (initialized)
                return;

            var coordinatedShotFeature = __instance.Get<BlueprintFeature>(_coordinatedShotFeatId);
            if (coordinatedShotFeature == null)
            {
                Main.Logger?.Write("Coordinated Shot feature not found");
                return;
            }

            coordinatedShotFeature.SetComponents(coordinatedShotFeature.ComponentsArray.RemoveAllFromArray(component => !(component is PrerequisiteFeature)));
            coordinatedShotFeature.AddComponent(CoordinatedShotAttackBonus.Create(coordinatedShotFeature));

        }
    }
}
