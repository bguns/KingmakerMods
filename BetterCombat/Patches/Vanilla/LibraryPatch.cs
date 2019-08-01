using BetterCombat.Helpers;
using Kingmaker.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Patches.Vanilla
{

    // This patch is used to set the Main.Library reference. It has a priority of First. This allows other additions to the 
    // blueprint library to use their own patches, referring to Main.Library, and avoids necessitating all library patches
    // to be somehow grouped in one huge patch method.
    //
    // IMPORTANT: LoadDictionary will possibly be run multiple times. m_Initialized will be true subsequent runs, which will
    //  cause the method to return instantly, however, ANY POSTFIX METHODS WILL STILL RUN, and possibly add multiple copies
    //  of the same feature/component etc...
    //
    // Given the above observation, I don't really think that the strategy as described in the first paragraph is all that
    // reliable, and will just use __instance in all LoadDictionary postfixes. This patch remains active because often
    // code needs to fetch things from the library outside of a postfix context
    [Harmony12.HarmonyPatch(typeof(LibraryScriptableObject), nameof(LibraryScriptableObject.LoadDictionary), new Type[0])]
    static class LibraryScriptableObject_LoadDictionary_Patch
    {
        [Harmony12.HarmonyPostfix]
        [Harmony12.HarmonyPriority(Harmony12.Priority.First)]
        static void Postfix(LibraryScriptableObject __instance)
        {
            var self = __instance;
            if (Main.Library != null) return;
            Main.Library = self;
        }
    }
}