using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace BetterCombat
{
    internal class Log
    {
        private readonly UnityModManager.ModEntry.ModLogger m_logger;

        private readonly StringBuilder str = new StringBuilder();

        internal Log(UnityModManager.ModEntry.ModLogger logger)
        {
            m_logger = logger;
        }

        internal void Flush()
        {
            if (str.Length == 0)
                return;

            m_logger.Log(str.ToString());
            str.Clear();
        }

        internal void Error(String message)
        {
            str.AppendLine(message);
            Flush();
        }

        internal void Error(Exception e)
        {
            str.AppendLine(e.ToString());
            Flush();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal void Write(String message)
        {
            Append(message);
            Flush();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal void Append(String message)
        {
            str.AppendLine(message);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal void Validate(BlueprintComponent c, BlueprintScriptableObject parent)
        {
            c.Validate(validation);
            if (validation.HasErrors)
            {
                Append($"Error: component `{c.name}` of `{parent.name}` failed validation:");
                foreach (var e in validation.Errors) Append($"  {e}");
                ((List<ValidationContext.Error>)validation.ErrorsAdvanced).Clear();
                Flush();
            }
        }

        static ValidationContext validation = new ValidationContext();
    }
}
