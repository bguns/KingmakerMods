using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Compatibility.EldritchArcana
{
    internal class EldritchArcanaPatches
    {
        internal static Type[] AllPatches =
        {
            typeof(Library_ModifyDirtyFighterTrait_Patch)
        };
    }
}
