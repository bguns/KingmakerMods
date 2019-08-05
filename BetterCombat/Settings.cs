using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace BetterCombat
{
    public class Settings : UnityModManager.ModSettings
    {
        public bool UseSoftCover = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save<Settings>(this, modEntry);
        }
    }
}
