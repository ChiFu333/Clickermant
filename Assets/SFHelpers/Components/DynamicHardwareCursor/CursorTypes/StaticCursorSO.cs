using UnityEngine;

namespace SFH {
    [CreateAssetMenu(fileName = "Static cursor", menuName = "SFH/Cursors/Static cursor", order = 1)]
    public class StaticCursorSO : CursorStateSO {
        [field:SerializeField] private Texture2D sprite;
        [field:SerializeField] private Vector2 hotSpot = Vector2.zero;
        public override Texture2D GetTexture() {
            return sprite;
        }
        public override Vector2 GetHotspot() {
            return hotSpot;
        }
    }
}