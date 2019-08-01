using BetterCombat.Data;
using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.NewFeats
{
    internal static class NewFeatsPatches
    {
        internal static Type[] AllPatches =
        {
            typeof(LowProfile_AddLocalization_Patch),
            typeof(LowProfile_AddFeat_Patch)
        };
    }
}
