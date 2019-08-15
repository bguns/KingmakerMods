using BetterCombat.Helpers;
using BetterCombat.Rules.CombatManeuvers;
using Kingmaker.RuleSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic.Commands;
using System.Collections.Generic;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{

    [Harmony12.HarmonyPatch(typeof(UnitAttack), "TriggerAttackRule", Harmony12.MethodType.Normal)]
    static class UnitAttack_TriggerAttackRule_CombatManeuverPatch
    {
        private static FastSetter unitattack_set_LastAttackRule = Harmony.CreateSetter<UnitAttack>(nameof(UnitAttack.LastAttackRule));
        private static FastGetter unitattack_get_m_AllAttacks = Harmony.CreateFieldGetter<UnitAttack>("m_AllAttacks");    
       
        static bool Prefix(UnitAttack __instance, AttackHandInfo attack)
        {
            var combatManeuver = Rulebook.Trigger(new RuleCheckCombatManeuverReplaceAttack(__instance.Executor, __instance.Target, attack)).Result;

            if (combatManeuver == CombatManeuver.None)
                return true;


            var lastAttackRule = new RuleAttackWithWeapon(__instance.Executor, __instance.Target, attack.Weapon, attack.AttackBonusPenalty)
            {
                IsRend = __instance.IsRend(__instance.PlannedAttack),
                IsFirstAttack = (attack.AttackNumber == 0),
                IsFullAttack = __instance.IsFullAttack,
                IsCharge = __instance.IsCharge,
                AttackNumber = attack.AttackNumber,
                AttacksCount = ((List<AttackHandInfo>)unitattack_get_m_AllAttacks(__instance)).Count
            };

            unitattack_set_LastAttackRule(__instance, lastAttackRule);

            attack.Target = __instance.Target;
            attack.IsHit = __instance.Executor.TriggerAttackReplacementCombatManeuver(__instance.Target, attack.Weapon, attack.AttackBonusPenalty, combatManeuver);

            return false;
        }
    }
}
