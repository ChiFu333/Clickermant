using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace SFH {
    [RequireComponent(typeof(Image)), ExecuteInEditMode]
    public class SDFImage : MonoBehaviour {
        [SerializeField] private float outlineThickness;
        [SerializeField] private Color outlineColor;

        [InfoBox("There is no SDF texture", InfoMessageType.Error,VisibleIf = "NoSDFTexture")]
        [SerializeField] private Sprite sprite;
        private Image image => GetComponent<Image>();
        private Material mat;
        private Material material;

        private void Setup() {
            material = Object.Instantiate(Resources.Load<Material>("SDFOutline"));
            mat = Object.Instantiate(material);
            if (sprite == null) return;
            image.sprite = sprite;
            //Update material

            SecondarySpriteTexture? sdfTexture = GetSDFTexture();
            if (sdfTexture == null) {
                //Show error
            } else {
                mat.SetTexture("_SDF", sdfTexture.Value.texture);
            }
            mat.SetFloat("_OutlineThickness", outlineThickness);
            mat.SetColor("_OutlineColor", outlineColor);
            image.material = mat;
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