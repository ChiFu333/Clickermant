using UnityEngine;

namespace SFH {
    public class CameraFollower {
        private readonly CameraController controller;
        private Transform target;

        public CameraFollower(CameraController _controller) {
            controller = _controller;
        }

        public void SetTarget(Transform _target) {
            target = _target;
        }

        public void Update() {
            Vector3 targetPosition = target.position.SetZ(-10);
            controller.transform.position = targetPosition;
        }
    }
}