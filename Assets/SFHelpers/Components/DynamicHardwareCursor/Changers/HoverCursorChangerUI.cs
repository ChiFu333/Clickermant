using UnityEngine;
using UnityEngine.EventSystems;

namespace SFH {
    public class HoverCursorChangerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        [SerializeField] private CursorStateSO cursor;
        [SerializeField] private bool isTemporal = true;
        public void OnPointerEnter(PointerEventData pointerEventData) {
            HardwareCursorManager.inst.SetCursor(cursor, false);
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            if (isTemporal) {
                HardwareCursorManager.inst.ResetCursor();
            }
        }
    }
}