using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterCombat.Helpers
{
    public static class LoadIcons
    {
        public static string IconsFolder = @"./Mods/CloserToTabletop/Icons/";
        public static Sprite Load(string iconPath)
        {
            var bytes = File.ReadAllBytes(iconPath);
            var texture = new Texture2D(64, 64);
            texture.LoadImage(bytes);
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0, 0));
        }

    }
}
