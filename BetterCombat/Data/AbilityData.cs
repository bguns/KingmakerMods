using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;

namespace BetterCombat.Data
{
    public struct AbilityData
    {
        internal readonly string Guid;
        internal readonly string Name;
        internal readonly string DisplayNameLocalizationKey;
        internal readonly string DisplayNameText;
        internal readonly string DescriptionLocalizationKey;
        internal readonly string DescriptionText;
        internal readonly string IconAssetGuid;
        internal readonly string IconFileName;
        internal readonly AbilityType Type;
        internal readonly AbilityRange Range;
        internal readonly UnitCommand.CommandType ActionType;
        internal readonly bool IsFullRoundAction;
        internal readonly string DurationLocalizationKey;
        internal readonly string DurationLocalizationText;
        internal readonly string SavingThrowLocalizationKey;
        internal readonly string SavingThrowLocalizationText;

        public AbilityData(string guid, string name, string displayNameLocalizationKey, string displayNameText, string descriptionLocalizationKey, string descriptionText, string iconAssetGuid, string iconFileName,
            AbilityType type, AbilityRange range, UnitCommand.CommandType actionType, bool isFullRoundAction, string durationLocalizationKey = null, string durationLocalizationText = null, 
            string savingThrowLocalizationKey = null, string savingThrowLocalizationText = null)
        {
            Guid = guid;
            Name = name;
            DisplayNameLocalizationKey = displayNameLocalizationKey;
            DisplayNameText = displayNameText;
            DescriptionLocalizationKey = descriptionLocalizationKey;
            DescriptionText = descriptionText;
            IconAssetGuid = iconAssetGuid;
            IconFileName = iconFileName;
            Type = type;
            Range = range;
            ActionType = actionType;
            IsFullRoundAction = isFullRoundAction;
            DurationLocalizationKey = durationLocalizationKey;
            DurationLocalizationText = durationLocalizationText;
            SavingThrowLocalizationKey = savingThrowLocalizationKey;
            SavingThrowLocalizationText = savingThrowLocalizationText;
        }
    }
}
