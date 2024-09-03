using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SFH {
    public class UILineRenderer : Graphic {
        public List<UILine> lines { get; private set; } = new List<UILine>();

        private VertexHelper cachedVH;
        protected override void OnPopulateMesh(VertexHelper vh) {
            cachedVH = vh;
            cachedVH.Clear();
            int index = 0;
            foreach (UILine line in lines) {
                DrawLine(ref cachedVH, line.start, line.end, line.thickness, line.color, index);
                index += 4;
            }

        }

        public void AddLine(UILine line) {
            lines.Add(line);
        }

        public void ForceUpdate() {
            SetAllDirty();
        }

        private void DrawLine(Vector2 start, Vector2 end, float thickness, Color color, int index) {
            DrawLine(ref cachedVH, start, end, thickness, color, index);
        }
        private void DrawLine(ref VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color color, int index) {
            DrawAngledLine(ref vh, index, start, end, thickness, CalculateAngle(start, end) + 90f, color);
        }

        private float CalculateAngle(Vector3 start, Vector3 end) {
            return Mathf.Atan2(1f * (end.y - start.y), 1 * (end.x - start.x)) * (180 / Mathf.PI);
        }

        private void DrawAngledLine(ref VertexHelper vh, int index, Vector3 start, Vector3 end, float thickness, float angle, Color color) {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
            vertex.position += start;
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
            vertex.position += start;
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
            vertex.position += end;
            vh.AddVert(vertex);

            vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
            vertex.position += end;
            vh.AddVert(vertex);

            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
        }

        [System.Serializable]
        public class UILine : Line {
            public UILine(Vector2 start, Vector2 end, float thickness, Color color) : base(start, end) {
                this.thickness = thickness;
                this.color = color;
            }
            public float thickness;
            public Color color;
        }
    }
}