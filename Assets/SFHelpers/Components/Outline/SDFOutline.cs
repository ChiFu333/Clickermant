using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;

namespace SFH {
    [RequireComponent(typeof(SpriteRenderer)), ExecuteInEditMode]
    public class SDFOutline : MonoBehaviour {
        [SerializeField] private float outlineThickness;
        [SerializeField] private Color outlineColor;

        [InfoBox("There is no SDF texture", InfoMessageType.Error,VisibleIf = "NoSDFTexture")]
        [SerializeField] private Sprite sprite;
        private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
        private Material material;

        private void Setup() {
            material = Object.Instantiate(Resources.Load<Material>("SDFOutline"));
            spriteRenderer.sprite = sprite;
            spriteRenderer.material = material;
            //Update mpb
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            spriteRenderer.GetPropertyBlock(mpb);

            mpb.SetColor("_OutlineColor", outlineColor);
            mpb.SetFloat("_OutlineThickness", outlineThickness);

            spriteRenderer.SetPropertyBlock(mpb);
        }

        private SecondarySpriteTexture? GetSDFTexture() {
            SecondarySpriteTexture[] texes = new SecondarySpriteTexture[sprite.GetSecondaryTextureCount()];
            sprite.GetSecondaryTextures(texes);
            for (int i = 0; i < texes.Length; i++) {
                if (texes[i].name == "_SDF") return texes[i];
            }
            return null;
        }

        private bool NoSDFTexture() {
            return GetSDFTexture() == null;
        }

        #region Internal

        private void OnEnable() {
            Setup();
        }

        private void OnValidate() {
            Setup();
        }

#if UNITY_EDITOR
        [Button("Open SDF editor")]
        private void OpenSDFEditor() {
            SpriteGenerator sg = SpriteGenerator.OpenWindow();
            sg.SetSpriteAsset(sprite.texture);
        }
#endif

        #endregion

    }
}