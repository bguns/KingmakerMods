using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Data
{
    public struct FeatData
    {
        internal readonly string Guid;
        internal readonly string Name;
        internal readonly string DisplayNameLocalizationKey;
        internal readonly string DisplayNameText;
        internal readonly string DescriptionLocalizationKey;
        internal readonly string DescriptionText;
        internal readonly string IconAssetGuid;

        public FeatData(string guid, string name, string displayNameLocalizationKey, string displayNameText, string descriptionLocalizationKey, string descriptionText, string iconAssetGuid)
        {
            Guid = guid;
            Name = name;
            DisplayNameLocalizationKey = displayNameLocalizationKey;
            DisplayNameText = displayNameText;
            DescriptionLocalizationKey = descriptionLocalizationKey;
            DescriptionText = descriptionText;
            IconAssetGuid = iconAssetGuid;
        }
    }

    internal class FeatSelection
    {
        internal static readonly string Basic            = "247a4068296e8be42890143f451b4b45";

        // Combat Feats
        internal static readonly string Fighter          = "41c8486641f7d6d4283ca9dae4147a9f";
        internal static readonly string EldritchKnight   = "da03141df23f3fe45b0c7c323a8e5a0e";
        internal static readonly string Magus            = "66befe7b24c42dd458952e3c47c93563";
        internal static readonly string WarDomain        = "79c6421dbdb028c4fa0c31b8eea95f16";
        internal static readonly string CombatTrick      = "c5158a6622d0b694a99efb1d0025d2c1";

        internal static readonly string[] AllCombat = new string[] { Fighter, EldritchKnight, Magus, WarDomain, CombatTrick };
    }
}
