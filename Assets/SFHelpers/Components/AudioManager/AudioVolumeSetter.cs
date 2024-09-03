using UnityEngine;
using UnityEngine.UI;

namespace SFH {
    [RequireComponent(typeof(Slider))]
    public class AudioVolumeSetter : MonoBehaviour {
        [SerializeField] private AudioManager.TargetSource target;
        private Slider slider => GetComponent<Slider>();

        private void SetVolume(float value) {
            AudioManager.inst.SetVolume(target, value);
        }

        private void Awake() {
            slider.onValueChanged.AddListener(SetVolume);
        }
    }
}