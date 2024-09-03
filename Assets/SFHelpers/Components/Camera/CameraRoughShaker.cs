using System.Collections;
using UnityEngine;

namespace SFH {
    public class CameraRoughShaker : ICameraShaker {
        private readonly CameraController controller;
        private readonly float maxOffset;
        public CameraRoughShaker(CameraController _controller, float _maxOffset) {
            controller = _controller;
            maxOffset = _maxOffset;
        }

        public void Shake(ShakeParameters parameters, AnimationCurve falloffCurve = null) {
            Shake(parameters.duration, parameters.amount, parameters.fade, falloffCurve);
        }

        public void Shake(float duration, float amount, bool fade = true, AnimationCurve falloffCurve = null) {
            if (falloffCurve == null) falloffCurve = CameraController.inst.shakeFalloffCurve;
            controller.StartCoroutine(ShakeRoutine(duration, amount, fade, falloffCurve));
        }

        private IEnumerator ShakeRoutine(float duration, float amount, bool fade, AnimationCurve falloffCurve) {
            Timer timer = new Timer(duration);
            while (!timer.Execute()) {
                float fadedAmount = fade ? amount * falloffCurve.Evaluate(timer.GetTimePassed() / duration) : amount;
                float xOffset = maxOffset * fadedAmount * (Random.value * 2 - 1);
                float yOffset = maxOffset * fadedAmount * (Random.value * 2 - 1);
                controller.cam.transform.localPosition = new Vector3(xOffset, yOffset, -10);
                yield return null;
            }
            controller.cam.transform.localPosition = -10 * Vector3.forward;
        }
    }
}