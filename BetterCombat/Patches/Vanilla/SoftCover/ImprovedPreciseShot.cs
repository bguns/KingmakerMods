using BetterCombat.Helpers;
using BetterCombat.Rules;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Controllers.Combat;
using Kingmaker.RuleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla.SoftCover
{
    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    class Library_ModifyImprovedPreciseStrike_Patch
    {
        static LibraryScriptableObject library => Main.Library;

        [Harmony12.HarmonyPostfix]
        static void Postfix()
        {
            var improvedPreciseStrikeFeat = library.Get<BlueprintFeature>("<ImprovedPreciseStrike GUID>");
            improvedPreciseStrikeFeat.AddComponent(Helpers.Library.Create<IgnoreCoverOnRangedAttack>());
        }
    }
}
