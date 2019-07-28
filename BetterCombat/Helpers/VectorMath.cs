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
        public static float AngleBetweenPoints(Vector2 corner, Vector2 point1, Vector2 point2)
        {

            if (corner == null || point1 == null || point2 == null || corner == point1 || corner == point2 || point1 == point2)
                return 0.0f;

            var position1 = point1 - corner;
            var position2 = point2 - corner;

            return Vector2.Angle(position1, position2);
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
