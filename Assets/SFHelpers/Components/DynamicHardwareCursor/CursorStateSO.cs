using UnityEngine;

namespace SFH {
    public abstract class CursorStateSO : ScriptableObject {
        public abstract Texture2D GetTexture();
        public abstract Vector2 GetHotspot();
    }
}