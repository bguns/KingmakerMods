using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Data
{
    public struct ActivatableAbilityData
    {
        internal readonly string Guid;
        internal readonly string Name;
        internal readonly string DisplayNameLocalizationKey;
        internal readonly string DisplayNameText;
        internal readonly string DescriptionLocalizationKey;
        internal readonly string DescriptionText;
        internal readonly string IconAssetGuid;
        internal readonly string IconFileName;

        public ActivatableAbilityData(string guid, string name, string displayNameLocalizationKey, string displayNameText, string descriptionLocalizationKey, string descriptionText, string iconAssetGuid, string iconFileName)
        {
            Guid = guid;
            Name = name;
            DisplayNameLocalizationKey = displayNameLocalizationKey;
            DisplayNameText = displayNameText;
            DescriptionLocalizationKey = descriptionLocalizationKey;
            DescriptionText = descriptionText;
            IconAssetGuid = iconAssetGuid;
            IconFileName = iconFileName;
        }
    }
}
