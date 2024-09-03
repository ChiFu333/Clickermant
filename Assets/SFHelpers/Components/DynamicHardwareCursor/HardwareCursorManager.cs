using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace SFH {
    public class HardwareCursorManager : MonoSingleton<HardwareCursorManager> {
        [SerializeField] private CursorStateSO defaultCursorState;
        private bool isWorld = true;

        private CursorStateSO currentUICursorState;
        //private CursorStateSO currentWorldCursorState;
        private CursorStateSO currentCursorState;

        private HoverCursorChanger currentHoveredChanger;
        private List<HoverCursorChanger> changers = new List<HoverCursorChanger>();
        private ScreenUtils.ScreenUtilsInterface screen;
        private bool requestCursorUpdate;

        public void SetCursor(CursorStateSO newCursorState, bool isWorld) {
            if (newCursorState == null) return;
            if (isWorld) {
                //currentWorldCursorState = newCursorState;
            } else {
                currentUICursorState = newCursorState;
            }
            currentCursorState = newCursorState;
            requestCursorUpdate = true;
        }

        public void ResetCursor() {
            if (defaultCursorState == null) return;
            currentCursorState = defaultCursorState;
            currentUICursorState = defaultCursorState;
            requestCursorUpdate = true;
        }

        #region Internal
        private void UpdateHardwareCursor() {
            requestCursorUpdate = false;
            if (currentCursorState == null) return;
            Cursor.SetCursor(currentCursorState.GetTexture(), currentCursorState.GetHotspot(), CursorMode.Auto);
        }

        private void Update() {
            HandleWorldChangers();
            if (requestCursorUpdate) UpdateHardwareCursor();
        }

        private void HandleWorldChangers() {
            if (!isWorld) return;
            changers.Clear();
            //Find all changers under mouse position
            List<GameObject> objects = screen.GetObjectsUnderMouse();
            for (int i = 0; i < objects.Count; i++) {
                HoverCursorChanger changer = objects[i].GetComponent<HoverCursorChanger>();
                if (changer == null) continue;
                changers.Add(changer);
            }
            //If no currentChanger - look for new
            if (currentHoveredChanger == null) {
                if (changers.Count >= 1) {
                    currentHoveredChanger = changers[0];
                    currentHoveredChanger.SetCursor();
                } else {
                    //Cursor handled by UI system
                    currentCursorState = currentUICursorState;
                    requestCursorUpdate = true;
                }
                return;
            }
            //If not covered but was - reset
            if (!changers.Contains(currentHoveredChanger)) {
                currentHoveredChanger.ResetCursor();
                currentHoveredChanger = null;
            }
        }

        private void Start() {
            screen = ScreenUtils.inst[Camera.main];
            ResetCursor();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/SFH/HardwareCursorManager")]
        private static void CreateHardwareCursorManager() {
            GameObject hcmo = new GameObject("HardwareCursorManager");
            hcmo.AddComponent<HardwareCursorManager>();
        }
#endif
        #endregion
    }
}