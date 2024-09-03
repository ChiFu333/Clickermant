using UnityEngine;

namespace SFH {
    public class Line {
        public Vector2 start;
        public Vector2 end;
        public Vector2 A { get => start; set { start = value; } }
        public Vector2 B { get => end; set { end = value; } }
        public Line(Vector2 start, Vector2 end) {
            this.start = start;
            this.end = end;
        }
    }
}