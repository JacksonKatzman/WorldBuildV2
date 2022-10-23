using System.Collections.Generic;
using UnityEngine;

namespace Game.GameMath
{
	public static class SpacialMath
    {
        public static Polygon JarvisMarch(List<Vector2> points)
        {
            if (points.Count < 3)
            {
                return null;
            }

            var result = new HashSet<Vector2>();
            int leftMostIndex = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[leftMostIndex].x > points[i].x)
                    leftMostIndex = i;
            }
            result.Add(points[leftMostIndex]);
            List<Vector2> collinearPoints = new List<Vector2>();
            Vector2 current = points[leftMostIndex];
            while (true)
            {
                Vector2 nextTarget = points[0];
                for (int i = 1; i < points.Count; i++)
                {
                    if (points[i] == current)
                        continue;
                    float x1, x2, y1, y2;
                    x1 = current.x - nextTarget.x;
                    x2 = current.x - points[i].x;

                    y1 = current.y - nextTarget.y;
                    y2 = current.y - points[i].y;

                    float val = (y2 * x1) - (y1 * x2);
                    if (val > 0)
                    {
                        nextTarget = points[i];
                        collinearPoints = new List<Vector2>();
                    }
                    else if (val == 0)
                    {
                        if (Vector2.Distance(current, nextTarget) < Vector2.Distance(current, points[i]))
                        {
                            collinearPoints.Add(nextTarget);
                            nextTarget = points[i];
                        }
                        else
                            collinearPoints.Add(points[i]);
                    }
                }

                foreach (Vector2 t in collinearPoints)
                    result.Add(t);
                if (nextTarget == points[leftMostIndex])
                    break;
                result.Add(nextTarget);
                current = nextTarget;
            }

            List<Vector2> convertedResult = new List<Vector2>();
            foreach (Vector2 point in result)
            {
                convertedResult.Add(point);
            }
            var polygon = new Polygon(convertedResult);

            return polygon;
        }
    }
}