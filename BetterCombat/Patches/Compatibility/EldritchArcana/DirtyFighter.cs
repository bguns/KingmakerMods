using BetterCombat.Helpers;
using BetterCombat.Rules;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.RuleSystem.Rules.Damage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Compatibility.EldritchArcana
{
    [AllowedOn(typeof(BlueprintUnitFact))]
    [AllowMultipleComponents]
    public class DamageBonusAgainstFlankedTarget : RuleInitiatorLogicComponent<RuleCalculateDamage>
    {
        public int Bonus;

        public static DamageBonusAgainstFlankedTarget Create(int bonus)
        {
            var d = Library.Create<DamageBonusAgainstFlankedTarget>();
            d.Bonus = bonus;
            return d;
        }

        public override void OnEventAboutToTrigger(RuleCalculateDamage evt)
        {
            if (evt.Target.IsFlankedByUnit(Owner.Unit) && evt.DamageBundle.Weapon?.Blueprint.IsMelee == true)
            {
                evt.DamageBundle.WeaponDamage?.AddBonusTargetRelated(Bonus);
            }
        }

        public override void OnEventDidTrigger(RuleCalculateDamage evt) { }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class Library_ModifyDirtyFighterTrait_Patch
    {
        static string _dirtyFighterFeatId = "ac47c14063574a0a9ea6927bf637a02a";

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

            var dirtyTrickFeature = __instance.Get<BlueprintFeature>(_dirtyFighterFeatId);
            if (dirtyTrickFeature == null)
            {
                Main.Logger?.Write("Dirty Trick feature not found");
                return;
            }

            Main.Logger?.Write("Patching Dirty Trick");
            dirtyTrickFeature.RemoveAllComponentsOfType<BlueprintComponent>();
            dirtyTrickFeature.AddComponent(DamageBonusAgainstFlankedTarget.Create(1));

        }
    }
}
