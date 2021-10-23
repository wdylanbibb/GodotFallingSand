using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;


namespace LineSimplification
{
    public class RamerDouglasPeucker
    {
        static float PerpendicularDistance(Vector2 pt, Vector2 lineStart, Vector2 lineEnd) {
            Vector2 d = lineEnd - lineStart;
 
            // Normalize
            float mag = Mathf.Sqrt(d.x * d.x + d.y * d.y);
            if (mag > 0.0) {
                d = d/mag;
            }

            Vector2 pv = pt - lineStart;

            // Get dot product (project pv onto normalized direction)
            float pvdot = d.x * pv.x + d.y * pv.y;
 
            // Scale line direction vector and subtract it from pv
            Vector2 a = pv - pvdot * d;
 
            return Mathf.Sqrt(a.x * a.x + a.y * a.y);
        }
 
        public static Array<Vector2> Decimate(Array<Vector2> pointList, float epsilon) {
            if (pointList.Count < 2) {
                throw new ArgumentOutOfRangeException("Not enough points to simplify");
            }
 
            // Find the point with the maximum distance from line between the start and end
            float dmax = 0.0f;
            int index = 0;
            int end = pointList.Count - 1;
            for (int i = 1; i < end; ++i) {
                float d = PerpendicularDistance(pointList[i], pointList[0], pointList[end]);
                if (d > dmax) {
                    index = i;
                    dmax = d;
                }
            }
 
            // If max distance is greater than epsilon, recursively simplify
            if (dmax > epsilon) {
                Array<Vector2> firstLine = new Array<Vector2>(pointList.Take(index + 1).ToList());
                Array<Vector2> lastLine = new Array<Vector2>(pointList.Skip(index).ToList());
                Array<Vector2> recResults1 = Decimate(firstLine, epsilon);
                Array<Vector2> recResults2 = Decimate(lastLine, epsilon);
 
                // build the result list
                List<Vector2> output = new List<Vector2>();
                output.AddRange(recResults1.Take(recResults1.Count - 1));
                output.AddRange(recResults2);
                if (output.Count < 2) GD.PrintErr("Problem Assembling Output");
                return new Array<Vector2>(output);
            }
            else {
                // Just return start and end points
                Array<Vector2> output = new Array<Vector2>();
                output.Add(pointList[0]);
                output.Add(pointList[pointList.Count - 1]);
                return output;
            }
        }
    }
}

