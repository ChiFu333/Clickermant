#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SFH {

    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : Editor {
        public override void OnInspectorGUI() {
            CameraController cameraController = (CameraController)target;
            cameraController.components = (CameraController.CameraComponent)EditorGUILayout.EnumFlagsField("Components:", cameraController.components);
            if (cameraController.components.HasFlag(CameraController.CameraComponent.Follower)) {
                //Draw follower-related stuff
            }
            if (cameraController.components.HasFlag(CameraController.CameraComponent.Shaker)) {
                //Draw shaker-related stuff
                cameraController.maxShakerOffset = EditorGUILayout.FloatField("Max shaker offset:", cameraController.maxShakerOffset);
                cameraController.shakeFalloffCurve = EditorGUILayout.CurveField("Shake falloff:", cameraController.shakeFalloffCurve);
            }
        }
    }
}
#endif