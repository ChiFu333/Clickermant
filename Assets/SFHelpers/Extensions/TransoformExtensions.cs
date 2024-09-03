using UnityEngine;

namespace SFH {
    public static class TransoformExtensions {
        public static float DistanceTo(this Transform transform, Transform other) {
            return Vector2.Distance(transform.position, other.position);
        }

        public static Vector2 DirectionTo(this Transform transform, Transform other) {
            return transform.position.DirectionTo(other.position);
        }
    }
}