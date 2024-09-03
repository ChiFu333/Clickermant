using UnityEngine;

namespace SFH {
    public interface ICameraShaker {
        public void Shake(ShakeParameters parameters, AnimationCurve falloffCurve = null);
        public void Shake(float duration, float amount, bool fade = true, AnimationCurve falloffCurve = null);
    }

    [System.Serializable]
    public class ShakeParameters {
        public float duration;
        public float amount;
        public bool fade;
        public ShakeParameters(float _duration, float _amount, bool _fade = true) {
            duration = _duration;
            amount = _amount;
            fade = _fade;
        }
    }
}