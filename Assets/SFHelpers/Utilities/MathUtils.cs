using System;
using UnityEngine;

namespace SFH {
    public static class MathUtils {
        public static float GetRadiansBetweenPositions(Vector2 pointA, Vector2 pointB) {
            Vector2 delta = pointB - pointA;
            return Mathf.Atan2(delta.y, delta.x);
        }

        public static float GetEulerBetweenPositions(Vector2 pointA, Vector2 pointB) {
            return GetRadiansBetweenPositions(pointA, pointB) * Mathf.Rad2Deg;
        }

        #region Lines

        public static bool DoLinesIntersect(Line lineA, Line lineB) {
            return CCW(lineA.A, lineB.A, lineB.B) != CCW(lineA.B, lineB.A, lineB.B) && CCW(lineA.A, lineA.B, lineB.A) != CCW(lineA.A, lineA.B, lineB.B);
        }

        private static bool CCW(Vector2 A, Vector2 B, Vector2 C) {
            return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
        }
            

        public static Vector2? FindLinesIntersection(Line lineA, Line lineB, double tolerance = 0.001) {
            double x1 = lineA.A.x, y1 = lineA.A.y;
            double x2 = lineA.B.x, y2 = lineA.B.y;

            double x3 = lineB.A.x, y3 = lineB.A.y;
            double x4 = lineB.B.x, y4 = lineB.B.y;

            if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance) {
                //Both lines overlap vertically, ambiguous intersection points
                return null;
            }
            if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance) {
                //Both lines overlap horizontally, ambiguous intersection points
                return null;
            }
            if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance) {
                return null;
            }
            if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance) {
                return null;
            }

            double x, y;

            if (Math.Abs(x1 - x2) < tolerance) {
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                x = x1;
                y = c2 + m2 * x1;
            } else if (Math.Abs(x3 - x4) < tolerance) {
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                x = x3;
                y = c1 + m1 * x3;
            } else {
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                x = (c1 - c2) / (m2 - m1);
                y = c2 + m2 * x;

                if (!(Math.Abs(-m1 * x + y - c1) < tolerance
                    && Math.Abs(-m2 * x + y - c2) < tolerance)) {
                    return null;
                }
            }

            if (IsInsideLine(lineA, x, y) &&
                IsInsideLine(lineB, x, y)) {
                return new Vector2((float)x, (float)y);
            }
            return null;
        }

        private static bool IsInsideLine(Line line, double x, double y) {
            return (x >= line.A.x && x <= line.B.x
                        || x >= line.B.x && x <= line.A.x)
                   && (y >= line.A.y && y <= line.B.y
                        || y >= line.B.y && y <= line.A.y);
        }

        #endregion
    }
}