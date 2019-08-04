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
        public static Vector2 NearestPointOnSegmentToPoint(Vector2 start, Vector2 end, Vector2 point)
        {
            Vector2 line = end - start;
            float length = line.magnitude;
            line.Normalize();

            Vector2 v = point - start;
            float d = Vector2.Dot(v, line);
            d = Mathf.Clamp(d, 0f, length);
            return start + line * d;
        }

        public static int TangentPointsOnCircleFromPoint(Vector2 point, Vector2 circleCenter, float radius, out Vector2 tangent1, out Vector2 tangent2)
        {
            if (Vector2.Distance(point, circleCenter) <= radius)
            {
                tangent1 = Vector2.zero;
                tangent2 = Vector2.zero;
                return 0;
            }
            Vector2 midPoint = (point + circleCenter) / 2.0f;
            float midPointCircleRadius = Vector2.Distance(point, circleCenter) / 2.0f;

            return CirclesIntersect(circleCenter, radius, midPoint, midPointCircleRadius, out tangent1, out tangent2);
            
        }

        // See http://paulbourke.net/geometry/circlesphere/ , "Intersection of two circles", for explanations of variables
        public static int CirclesIntersect(Vector2 p0, float r0, Vector2 p1, float r1, out Vector2 t1, out Vector2 t2)
        {
            float d = Vector2.Distance(p1, p0);
            if (d > r0 + r1 || d < Math.Abs(r0 - r1) || (d < Mathf.Epsilon && Math.Abs(r0 - r1) < Mathf.Epsilon))
            {
                t1 = Vector2.zero;
                t2 = Vector2.zero;
                return 0;
            }

            float a = ((r0 * r0) - (r1 * r1) + (d * d)) / (2.0f * d);
            float h = (float)Math.Sqrt((r0 * r0) - (a * a));

            Vector2 p2 = p0 + a * (p1 - p0) / d;

            float t1x = p2.x + h * (p1.y - p0.y) / d;
            float t1y = p2.y - h * (p1.x - p0.x) / d;

            float t2x = p2.x - h * (p1.y - p0.y) / d;
            float t2y = p2.y + h * (p1.x - p0.x) / d;

            t1 = new Vector2(t1x, t1y);
            t2 = new Vector2(t2x, t2y);

            return 2;
        }

        public static Vector2[] FindEquidistantPointsOnArc(Vector2 startPoint, Vector2 endPoint, Vector2 circleCenter, float radius, int numberOfPoints)
        {
            Vector2[] result = new Vector2[numberOfPoints];
            result[0] = startPoint;
            result[numberOfPoints - 1] = endPoint;

            float angleBetweenPoints = AngleBetweenPoints(circleCenter, startPoint, endPoint) / (numberOfPoints - 1);

            Vector2 currentDirection = (startPoint - circleCenter).normalized;
            Vector2 endDirection = (endPoint - circleCenter).normalized;

            for (int i = 1; i < numberOfPoints - 1; i++)
            {
                currentDirection = Vector3.RotateTowards(currentDirection.To3D(), endDirection.To3D(), angleBetweenPoints * Mathf.Deg2Rad, 0.0f).To2D();
                result[i] = circleCenter + currentDirection * radius;
            }

            return result;
        }
    }
}
