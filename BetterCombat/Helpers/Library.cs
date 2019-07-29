using Kingmaker.Blueprints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterCombat.Helpers
{
    static class Library
    {
        #region Helpers

        public static T Create<T>(Action<T> init = null) where T : ScriptableObject
        {
            var result = ScriptableObject.CreateInstance<T>();
            if (init != null) init(result);
            return result;
        }

        #endregion

        #region Extensions

        #region Library

        private static FastGetter library_get_m_initialized = Harmony.CreateFieldGetter(typeof(LibraryScriptableObject), "m_Initialized");

        public static bool Initialized(this LibraryScriptableObject library)
        {
            return (bool)library_get_m_initialized(library);
        }

        #endregion

        #region Blueprints
        public static T Get<T>(this LibraryScriptableObject library, String assetId) where T : BlueprintScriptableObject
        {
            try
            {
                return (T)library.BlueprintsByAssetId[assetId];
            }
            catch (KeyNotFoundException)
            {
                Main.Logger?.Write($"Key {assetId} not present in library.");
                return null;
            }
        }
        #endregion

        #region Blueprint Components
        public static void AddComponent(this BlueprintScriptableObject obj, BlueprintComponent component)
        {
            obj.SetComponents(obj.ComponentsArray.AddToArray(component));
        }

        public static void RemoveComponent(this BlueprintScriptableObject obj, BlueprintComponent component)
        {
            obj.SetComponents(obj.ComponentsArray.RemoveFromArray(component));
        }

        public static void RemoveAllComponentsOfType<T>(this BlueprintScriptableObject obj) where T : BlueprintComponent
        {
            obj.SetComponents(obj.ComponentsArray.RemoveAllFromArray(e => e.GetType() == typeof(T)));
        }

        public static void SetComponents(this BlueprintScriptableObject obj, params BlueprintComponent[] components)
        {
            // Fix names of components. Generally this doesn't matter, but if they have serialization state,
            // then their name needs to be unique.
            var names = new HashSet<string>();
            foreach (var c in components)
            {
                if (string.IsNullOrEmpty(c.name))
                {
                    c.name = $"${c.GetType().Name}";
                }
                if (!names.Add(c.name))
                {
                    //SaveCompatibility.CheckComponent(obj, c);
                    String name;
                    for (int i = 0; !names.Add(name = $"{c.name}${i}"); i++) ;
                    c.name = name;
                }
                Main.Logger?.Validate(c, obj);
            }

            obj.ComponentsArray = components;
        }
        #endregion

        #endregion
    }
}
