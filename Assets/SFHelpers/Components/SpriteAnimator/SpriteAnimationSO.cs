using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SFH {
    [CreateAssetMenu(fileName = "Sprite Animation", menuName = "SFH/Sprite Animation", order = 1)]
    public class SpriteAnimationSO : ScriptableObject {
        [field: SerializeField, ValidateInput("ValidateFramerate")] public float framerate { get; private set; }
        [SerializeField, ReadOnly] public string frameTime;
        [field: SerializeField] public Order animationOrder { get; private set; } = Order.Forward;
        [SerializeField] private List<SpriteDrawer> sprites = new List<SpriteDrawer>();

        public Sprite GetSprite(int index) {
            return sprites[index].sprite;
        }

        public int GetFramesAmount() => sprites.Count;

        #region Internal

        public enum Order {
            Forward,
            PingPong
        }

        private bool ValidateFramerate(ref string errorMessage, ref InfoMessageType? messageType) {
            if (framerate == 0) {
                errorMessage = "Framerate cannot be equal to 0";
                return false;
            }
            if (framerate < 0) {
                errorMessage = "Framerate cannot be negative";
                return false;
            }
            return true;
        }

        private void OnValidate() {
            frameTime = (1000 / framerate).ToString("0.00") + " ms";
        }

        [System.Serializable]
        private class SpriteDrawer {
            [SerializeField, OnValueChanged("UpdateSprite"), HorizontalGroup(.85f), LabelText("Sprite")]private Sprite spriteField;
            [PreviewField(50, ObjectFieldAlignment.Right), HorizontalGroup(.15f), HideLabel, ReadOnly] public Sprite sprite;
            private void UpdateSprite() {
                sprite = spriteField;
            }
        }

        #endregion

    }
}