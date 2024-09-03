using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SFH {
    public static class VectorExtensions {
        #region Vector2

        #region MaxMin
        public static Vector2 MaxX(IEnumerable<Vector2> collection) {
            return SelectVector2(collection, (a, b) => a.x > b.x);
        }
        public static Vector2 MaxY(IEnumerable<Vector2> collection) {
            return SelectVector2(collection, (a, b) => a.y > b.y);
        }
        public static Vector2 MinX(IEnumerable<Vector2> collection) {
            return SelectVector2(collection, (a, b) => a.x < b.x);
        }
        public static Vector2 MinY(IEnumerable<Vector2> collection) {
            return SelectVector2(collection, (a, b) => a.y < b.y);
        }
        public static Vector2 SelectVector2(IEnumerable<Vector2> collection, Func<Vector2, Vector2, bool> function) {
            if (collection.Count() == 0) return Vector2.zero;
            Vector2 selectedVector = collection.ElementAt(0);
            for (int i = 1; i < collection.Count(); i++) {
                if (function(collection.ElementAt(i), selectedVector)) selectedVector = collection.ElementAt(i);
            }
            return selectedVector;
        }

        #endregion

        public static Vector2 DirectionTo(this Vector2 vector, Vector2 target) {
            return (target - vector).normalized;
        }

        public static float DistanceTo(this Vector2 vector, Vector2 target) {
            return Vector2.Distance(vector, target);
        }

        public static Vector2Int ToVector2Int(this Vector2 vector) {
            return new Vector2Int((int)vector.x, (int)vector.y);
        }

        public static Vector2 SetX(this Vector2 vector, float x) {
            vector.x = x;
            return vector;
        }

        public static Vector2 SetY(this Vector2 vector, float y) {
            vector.y = y;
            return vector;
        }
        #endregion
        #region Vector3

        #region MaxMin
        public static Vector3 MaxX(IEnumerable<Vector3> collection) {
            return SelectVector3(collection, (a, b) => a.x > b.x);
        }
        public static Vector3 MaxY(IEnumerable<Vector3> collection) {
            return SelectVector3(collection, (a, b) => a.y > b.y);
        }
        public static Vector3 MaxZ(IEnumerable<Vector3> collection) {
            return SelectVector3(collection, (a, b) => a.z > b.z);
        }
        public static Vector3 MinX(IEnumerable<Vector3> collection) {
            return SelectVector3(collection, (a, b) => a.x < b.x);
        }
        public static Vector3 MinY(IEnumerable<Vector3> collection) {
            return SelectVector3(collection, (a, b) => a.y < b.y);
        }
        public static Vector3 MinZ(IEnumerable<Vector3> collection) {
            return SelectVector3(collection, (a, b) => a.z < b.z);
        }

        public static Vector2 SelectVector3(IEnumerable<Vector3> collection, Func<Vector3, Vector3, bool> function) {
            if (collection.Count() == 0) return Vector3.zero;
            Vector3 selectedVector = collection.ElementAt(0);
            for (int i = 1; i < collection.Count(); i++) {
                if (function(collection.ElementAt(i), selectedVector)) selectedVector = collection.ElementAt(i);
            }
            return selectedVector;
        }
        #endregion

        public static Vector3 DirectionTo(this Vector3 vector, Vector3 target) {
            return (target - vector).normalized;
        }

        public static float DistanceTo(this Vector3 vector, Vector3 target) {
            return Vector3.Distance(vector, target);
        }

        public static Vector3Int ToVector3Int(this Vector3 vector) {
            return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
        }

        public static Vector3 SetX(this Vector3 vector, float x) {
            vector.x = x;
            return vector;
        }

        public static Vector3 SetY(this Vector3 vector, float y) {
            vector.y = y;
            return vector;
        }

        public static Vector3 SetZ(this Vector3 vector, float z) {
            vector.z = z;
            return vector;
        }

        #endregion
    }
}
