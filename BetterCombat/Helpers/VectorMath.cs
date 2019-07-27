using Kingmaker.EntitySystem.Entities;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterCombat.Helpers
{
    public static class VectorMath
    {
        public static float AngleBetweenUnits(UnitEntityData cornerUnit, UnitEntityData unit1, UnitEntityData unit2)
        {

            if (cornerUnit == null || unit1 == null || unit2 == null || cornerUnit == unit1 || cornerUnit == unit2 || unit1 == unit2)
                return 0.0f;

            var position1 = unit1.Position - cornerUnit.Position;
            var position2 = unit2.Position - cornerUnit.Position;

            return Vector2.Angle(position1.To2D(), position2.To2D());
        }

        // Taken from:
        // https://forum.unity.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/#post-2199249
        // Also interesting: https://en.wikipedia.org/wiki/Scalar_projection
        public static Vector2 NearestPointOnLineToPoint(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
        {
            Vector2 line = lineEnd - lineStart;
            float length = line.magnitude;
            line.Normalize();

            Vector2 v = point - lineStart;
            float d = Vector2.Dot(v, line);
            d = Mathf.Clamp(d, 0f, length);
            return lineStart + line * d;
        }
    }
}
