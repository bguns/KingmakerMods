using BetterCombat.Data;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Localization;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
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

        private static readonly FastSetter blueprintScriptableObject_set_AssetId = Harmony.CreateFieldSetter<BlueprintScriptableObject>("m_AssetGuid");

        public static T Create<T>(Action<T> init = null) where T : ScriptableObject
        {
            var result = ScriptableObject.CreateInstance<T>();
            if (init != null) init(result);
            return result;
        }

        public static BlueprintFeature CreateFeat(FeatData featData)
        {
            var feat = Create<BlueprintFeature>();
            feat.name = featData.Name;
            blueprintScriptableObject_set_AssetId(feat, featData.Guid);
            feat.SetName(Localization.CreateString(featData.DisplayNameLocalizationKey, featData.DisplayNameText));
            feat.SetDescription(Localization.CreateString(featData.DescriptionLocalizationKey, featData.DescriptionText));
            feat.SetIcon(featData.IconAssetGuid);
            feat.Groups = new FeatureGroup[] { FeatureGroup.Feat };
            return feat;
        }

        static readonly FastSetter blueprintAbility_set_m_IsFullRoundAction = Harmony.CreateFieldSetter<BlueprintAbility>("m_IsFullRoundAction");

        public static BlueprintAbility CreateAbility(AbilityData abilityData)
        {
            var ability = Create<BlueprintAbility>();
            ability.name = abilityData.Name;
            blueprintScriptableObject_set_AssetId(ability, abilityData.Guid);
            ability.SetName(Localization.CreateString(abilityData.DisplayNameLocalizationKey, abilityData.DisplayNameText));
            ability.SetDescription(Localization.CreateString(abilityData.DescriptionLocalizationKey, abilityData.DescriptionText));
            if (abilityData.IconAssetGuid != null)
                ability.SetIcon(abilityData.IconAssetGuid);
            else if (abilityData.IconFileName != null)
                ability.SetIcon(LoadIcons.Load(LoadIcons.IconsFolder + abilityData.IconFileName));
            ability.Type = abilityData.Type;
            ability.Range = abilityData.Range;
            ability.ActionType = abilityData.ActionType;
            blueprintAbility_set_m_IsFullRoundAction(ability, abilityData.IsFullRoundAction);

            if (abilityData.DurationLocalizationKey == null)
                ability.LocalizedDuration = new LocalizedString();
            else
                ability.LocalizedDuration = Localization.CreateString(abilityData.DurationLocalizationKey, abilityData.DurationLocalizationText);

            if (abilityData.SavingThrowLocalizationKey == null)
                ability.LocalizedSavingThrow = new LocalizedString();
            else
                ability.LocalizedSavingThrow = Localization.CreateString(abilityData.SavingThrowLocalizationKey, abilityData.SavingThrowLocalizationText);

            return ability;
        }

        public static BlueprintActivatableAbility CreateActivatableAbility(ActivatableAbilityData activatableAbilityData)
        {
            var ability = Create<BlueprintActivatableAbility>();
            ability.name = activatableAbilityData.Name;
            blueprintScriptableObject_set_AssetId(ability, activatableAbilityData.Guid);
            ability.SetName(Localization.CreateString(activatableAbilityData.DisplayNameLocalizationKey, activatableAbilityData.DisplayNameText));
            ability.SetDescription(Localization.CreateString(activatableAbilityData.DescriptionLocalizationKey, activatableAbilityData.DescriptionText));
            if (activatableAbilityData.IconAssetGuid != null)
                ability.SetIcon(activatableAbilityData.IconAssetGuid);
            else if (activatableAbilityData.IconFileName != null)
                ability.SetIcon(LoadIcons.Load(LoadIcons.IconsFolder + activatableAbilityData.IconFileName));
            return ability;
        }

        

        #endregion

        #region Extensions

        #region Library

        private static FastGetter library_get_m_initialized = Harmony.CreateFieldGetter(typeof(LibraryScriptableObject), "m_Initialized");

        public static bool Initialized(this LibraryScriptableObject library)
        {
            return (bool)library_get_m_initialized(library);
        }

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

        public static void AddFeatToFeatureGroup(this LibraryScriptableObject library, BlueprintFeature feat, string featureGroupId)
        {
            var featGroup = library.Get<BlueprintFeatureSelection>(featureGroupId);
            var allFeats = featGroup.AllFeatures.ToList();
            allFeats.Add(feat);
            featGroup.AllFeatures = allFeats.ToArray();

            if (featGroup.Group == FeatureGroup.None)
            {
                var features = featGroup.Features.ToList();
                features.Add(feat);
                featGroup.Features = features.ToArray();
            }
        }

        #endregion

        #region Blueprints
        static readonly FastSetter blueprintUnitFact_set_Description = Harmony.CreateFieldSetter<BlueprintUnitFact>("m_Description");
        static readonly FastSetter blueprintUnitFact_set_Icon = Harmony.CreateFieldSetter<BlueprintUnitFact>("m_Icon");
        static readonly FastSetter blueprintUnitFact_set_DisplayName = Harmony.CreateFieldSetter<BlueprintUnitFact>("m_DisplayName");
        static readonly FastGetter blueprintUnitFact_get_Description = Harmony.CreateFieldGetter<BlueprintUnitFact>("m_Description");
        static readonly FastGetter blueprintUnitFact_get_DisplayName = Harmony.CreateFieldGetter<BlueprintUnitFact>("m_DisplayName");

        public static LocalizedString GetName(this BlueprintUnitFact fact) => (LocalizedString)blueprintUnitFact_get_DisplayName(fact);

        public static void SetName(this BlueprintUnitFact fact, LocalizedString name)
        {
            blueprintUnitFact_set_DisplayName(fact, name);
        }

        public static LocalizedString GetDescription(this BlueprintUnitFact fact) => (LocalizedString)blueprintUnitFact_get_Description(fact);

        public static void SetDescription(this BlueprintUnitFact fact, LocalizedString description)
        {
            blueprintUnitFact_set_Description(fact, description);
        }

        public static void SetIcon(this BlueprintUnitFact fact, string iconAssetGuid)
        {
            var iconFact = Main.Library?.Get<BlueprintUnitFact>(iconAssetGuid);
            if (iconFact == null)
            {
                Main.Logger?.Error($"SetIcon: asset of type {typeof(BlueprintUnitFact).Name} with guid {iconAssetGuid} not found in library.");
                return;
            }
            fact.SetIcon(iconFact.Icon);
        }

        public static void SetIcon(this BlueprintUnitFact fact, Sprite icon)
        {
            blueprintUnitFact_set_Icon(fact, icon);
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
