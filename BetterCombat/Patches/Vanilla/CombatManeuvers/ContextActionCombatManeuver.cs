using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.CombatManeuvers
{
    public class ContextActionCombatManeuverCTT : ContextActionCombatManeuver
    {

        public static ContextActionCombatManeuverCTT CreateFrom(ContextActionCombatManeuver original, bool doNotProvokeAoO = false)
        {
            var instance = Library.Create<ContextActionCombatManeuverCTT>();
            instance.Type = original.Type;
            instance.OnSuccess = original.OnSuccess;
            instance.ReplaceStat = original.ReplaceStat;
            instance.NewStat = original.NewStat;
            instance.UseKineticistMainStat = original.UseKineticistMainStat;
            instance.UseCastingStat = original.UseCastingStat;
            instance.UseCasterLevelAsBaseAttack = original.UseCasterLevelAsBaseAttack;
            instance.UseBestMentalStat = original.UseBestMentalStat;
            instance.BatteringBlast = original.BatteringBlast;
            instance.DoNotProvokeAoO = doNotProvokeAoO;
            return instance;
        }

        public bool DoNotProvokeAoO;

        public override void RunAction()
        {
            Main.Logger?.Write($"ContextActionCombatManeuverCTT.RunAction, DoNotProvokeAoO = {DoNotProvokeAoO}");
            if (DoNotProvokeAoO && Context.MaybeCaster != null)
            {
                Main.Logger?.Write($"ContextActionCombatManeuverCTT.RunAction, no AoO for {Context.MaybeCaster?.CharacterName}");
                CombatManeuverProvokeAttack.DoNotTriggerAoOForNextCombatManeuver(Context.MaybeCaster);
            }
            base.RunAction();
        }
    }

    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class LibraryScriptableObject_CombatManeuverContextActions_Patch
    {
        static bool initialized = false;

        [Harmony12.HarmonyPrefix]
        static bool Prefix(LibraryScriptableObject __instance)
        {
            initialized = __instance.Initialized();
            return true;
        }

        [Harmony12.HarmonyPostfix]
        static void Postfix(LibraryScriptableObject __instance)
        {
            if (initialized)
                return;

            foreach (var key in CombatManeuverData.abilitiesThatShouldReplaceContextActionCombatManeuver.Keys)
            {
                var ability = __instance.Get<BlueprintAbility>(key);
                var runAction = ability.GetComponent<AbilityEffectRunAction>();
                var combatManeuverAction = (ContextActionCombatManeuver)runAction.Actions.Actions.First(action => action.GetType() == typeof(ContextActionCombatManeuver));
                if (combatManeuverAction == null)
                {
                    Main.Logger?.Error($"{CombatManeuverData.abilitiesThatShouldReplaceContextActionCombatManeuver[key]} ContextActionCombatManeuver not found");
                    continue;
                }

                var newCombatManeuverAction = ContextActionCombatManeuverCTT.CreateFrom(combatManeuverAction, true);
                runAction.Actions.Actions = runAction.Actions.Actions.RemoveFromArray(combatManeuverAction);
                runAction.Actions.Actions = runAction.Actions.Actions.AddToArray(newCombatManeuverAction);
                Main.Logger?.Write($"{CombatManeuverData.abilitiesThatShouldReplaceContextActionCombatManeuver[key]} action patched.");
            }
        }
    }
}
