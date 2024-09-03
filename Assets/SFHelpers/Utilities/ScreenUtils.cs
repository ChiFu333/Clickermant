using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SFH {
    public class ScreenUtils : Singleton<ScreenUtils> {
        #region Internal
        public ScreenUtilsInterface this[Camera cam] {
            get {
                if (!camInterfaceLookup.ContainsKey(cam)) {
                    camInterfaceLookup.Add(cam, new ScreenUtilsInterface(cam));
                }
                return camInterfaceLookup[cam];
            }
        }
        private readonly Dictionary<Camera, ScreenUtilsInterface> camInterfaceLookup = new Dictionary<Camera, ScreenUtilsInterface>();
        #endregion

        #region UI
        public static List<RaycastResult> GetUIUnderMouse() {
            PointerEventData pointerData = new PointerEventData(EventSystem.current) { pointerId = -1 };
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            Debug.Assert(EventSystem.current != null, "Please setup EventSystem for this to work");
            EventSystem.current.RaycastAll(pointerData, results);
            return results;
        }

        public static bool IsMouseOverUI() {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            Debug.Assert(EventSystem.current != null,"Please setup EventSystem for this to work");
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            int count = raycastResults.Count;
            //There goes implementation of ignorable component detection
            return count > 0;
        }
        #endregion

        public class ScreenUtilsInterface {
            private readonly Camera cam;
            public ScreenUtilsInterface(Camera _cam) {
                cam = _cam;
            }

            public Vector2 WorldMouse() {
                return cam.ScreenToWorldPoint(Input.mousePosition);
            }

            public Vector2 WorldPosition(Vector2 position) {
                return cam.ScreenToWorldPoint(position);
            }

            public Vector2 WorldBottomLeft() {
                return WorldPosition(new Vector2(0,0));
            }

            public Vector2 WorldTopRight() {
                return WorldPosition(new Vector2(Screen.width, Screen.height));
            }

            public bool IsPositionOnScreen(Vector2 position) {
                Vector2 ScreenPosition = cam.WorldToScreenPoint(position);
                return ScreenPosition.x > 0 && ScreenPosition.x < cam.pixelWidth &&
                       ScreenPosition.y > 0 && ScreenPosition.y < cam.pixelHeight;
            }

            public List<GameObject> GetObjectsUnderMouse() {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin,ray.direction);
                return hits.Select(i => i.collider.gameObject).ToList();
            }
            
        }
    }
}