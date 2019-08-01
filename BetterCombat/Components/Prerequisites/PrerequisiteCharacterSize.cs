using JetBrains.Annotations;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Root;
using Kingmaker.Enums;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCombat.Rules.Prerequisites
{
    class PrerequisiteCharacterSize : Prerequisite
    {
         
        public Size Value { get; set; }
        public bool OrSmaller { get; set; }
        public bool OrLarger { get; set; }

        public override bool Check([CanBeNull] FeatureSelectionState selectionState, [NotNull] UnitDescriptor unit, [NotNull] LevelUpState state)
        {
            return CheckUnit(unit);
        }

        public bool CheckUnit(UnitDescriptor unit)
        {
            if (unit.OriginalSize == Value)
                return true;

            if (OrSmaller && unit.OriginalSize < Value)
                return true;

            if (OrLarger && unit.OriginalSize > Value)
                return true;

            return false;
        }

        public override string GetUIText()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = $"Size: {LocalizedTexts.Instance.Sizes.GetText(Value)}";
            stringBuilder.Append(text);
            if (OrSmaller)
                stringBuilder.Append(" or smaller");
            if (OrLarger)
                stringBuilder.Append(" or larger");
            return stringBuilder.ToString();
        }
    }
}
