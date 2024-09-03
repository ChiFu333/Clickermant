using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace SFH {
    [CreateAssetMenu(fileName = "Audio Query", menuName = "SFH/Audio Query", order = 1)]
    public class AudioQuerySO : SerializedScriptableObject {
        public AudioClip clip;
        [field:MinMaxSlider(-1, 1, true)] [field: SerializeField]public Vector2 pitchVariance { get; private set; } = Vector2.zero;

#if UNITY_EDITOR
        private void Awake() {
            if (Selection.assetGUIDs.Length == 0) return;
            string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            AudioClip clipAsset = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clipAsset != null) {
                clip = clipAsset;
            }
        }
#endif
    }
}