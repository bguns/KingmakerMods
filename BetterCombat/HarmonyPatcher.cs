using BetterCombat.Patches.Vanilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat
{
    internal class HarmonyPatcher
    {
        private readonly Harmony12.HarmonyInstance m_harmonyInstance;
        private readonly Dictionary<Type, bool> m_typesPatched = new Dictionary<Type, bool>();
        private readonly List<String> m_failedPatches = new List<String>();
        private readonly List<String> m_failedLoading = new List<String>();

        internal Dictionary<Type, bool> TypesPatched
        {
            get { return m_typesPatched; }
        }

        internal List<String> FailedPatches
        {
            get { return m_failedPatches; }
        }

        internal List<String> FailedLoading
        {
            get { return m_failedLoading; }
        }

        internal HarmonyPatcher(Harmony12.HarmonyInstance harmonyInstance)
        {
            m_harmonyInstance = harmonyInstance;
        }

        internal bool ApplyPatches(Type[] patches, string featureName)
        {
            bool success = true;
            foreach(Type patch in patches)
            {
                success &= ApplyPatch(patch, featureName);
            }
            return success;
        }

        internal bool ApplyPatch(Type type, String featureName)
        {
            try
            {
                if (m_typesPatched.ContainsKey(type)) return m_typesPatched[type];

                var patchInfo = Harmony12.HarmonyMethodExtensions.GetHarmonyMethods(type);
                if (patchInfo == null || patchInfo.Count() == 0)
                {
                    Main.Logger?.Error($"Failed to apply patch {type}: could not find Harmony attributes");
                    m_failedPatches.Add(featureName);
                    m_typesPatched.Add(type, false);
                    return false;
                }
                var processor = new Harmony12.PatchProcessor(m_harmonyInstance, type, Harmony12.HarmonyMethod.Merge(patchInfo));
                var patch = processor.Patch().FirstOrDefault();
                if (patch == null)
                {
                    Main.Logger?.Error($"Failed to apply patch {type}: no dynamic method generated");
                    m_failedPatches.Add(featureName);
                    m_typesPatched.Add(type, false);
                    return false;
                }
                m_typesPatched.Add(type, true);
                return true;
            }
            catch (Exception e)
            {
                Main.Logger?.Error($"Failed to apply patch {type}: {e}");
                m_failedPatches.Add(featureName);
                m_typesPatched.Add(type, false);
                return false;
            }
        }

        internal void ManualPatch(MethodInfo method, Harmony12.HarmonyMethod prefix = null, Harmony12.HarmonyMethod postfix = null)
        {
            m_harmonyInstance.Patch(method, prefix: prefix, postfix: postfix);
        }

        internal void CheckPatchingSuccess()
        {
            // Check to make sure we didn't forget to patch something.
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var infos = Harmony12.HarmonyMethodExtensions.GetHarmonyMethods(type);
                if (infos != null && infos.Count() > 0 && !m_typesPatched.ContainsKey(type))
                {
                    Main.Logger?.Write($"Did not apply patch for {type}");
                }
            }
        }
    }
}
