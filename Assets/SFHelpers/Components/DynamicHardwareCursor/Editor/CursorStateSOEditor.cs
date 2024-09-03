#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace SFH {
    [CustomEditor(typeof(StaticCursorSO), true)]
    [CanEditMultipleObjects]
    public class CursorStateSOEditor : Editor {
        private StaticCursorSO cursorState { get { return (target as StaticCursorSO); } }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            if (cursorState.GetTexture() != null) {
                Texture2D renderedTexture = new Texture2D(cursorState.GetTexture().width, cursorState.GetTexture().height, TextureFormat.RGBA32, 1, true);
  
                Graphics.CopyTexture(cursorState.GetTexture(), renderedTexture);
                return renderedTexture;
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

    }
}
#endif