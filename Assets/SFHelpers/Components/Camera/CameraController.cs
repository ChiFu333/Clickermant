using UnityEngine;
using UnityEditor;

namespace SFH {
    public class CameraController : MonoSingleton<CameraController> {
        [SerializeField] public CameraComponent components;
        public CameraFollower follower { get; private set; }
        //Structure should be: CameraController (this) - Camera (child of this)
        public Camera cam => transform.GetComponentInChildren<Camera>();
        public ICameraShaker shaker { get; private set; }
        public float maxShakerOffset;
        public AnimationCurve shakeFalloffCurve = AnimationCurve.Linear(0,0,1,1);

        protected override void SingletonAwake() {
            if (components.HasFlag(CameraComponent.Follower)) {
                follower = new CameraFollower(this);
            }
            if (components.HasFlag(CameraComponent.Shaker)) {
                shaker = new CameraRoughShaker(this, maxShakerOffset);

            }
        }

        #region Internal

        private void Update() {
            if (components == CameraComponent.Follower) {
                follower.Update();
            }
        }


        [System.Flags]
        public enum CameraComponent {
            Nothing = 0,
            Follower = 1,
            Shaker = 2
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/SFH/Camera Controller")]
        private static void CreateCameraController() {
            Camera targetCamera = Camera.main;
            CameraController[] cameraControllers = GameObject.FindObjectsByType<CameraController>(FindObjectsSortMode.None);
            for (int i = 0; i < cameraControllers.Length; i++) {
                if (cameraControllers[i].transform.GetChild(0).GetComponent<Camera>() == targetCamera) {
                    Debug.LogWarning("Trying to create camera controller, but it already exists");
                    return;
                }
            }

            GameObject contrl = new GameObject("CameraController");
            targetCamera.gameObject.transform.SetParent(contrl.transform);
            contrl.AddComponent<CameraController>();
        }
#endif

        #endregion
    }
}