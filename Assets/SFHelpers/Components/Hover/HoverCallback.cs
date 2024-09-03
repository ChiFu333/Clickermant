using UnityEngine;
using UnityEngine.Events;

namespace SFH {
    public class HoverCallback : MonoBehaviour {
        [field: SerializeField] public UnityEvent<HoverCallback> onEnter { get; private set; } = new UnityEvent<HoverCallback>();
        [field: SerializeField] public UnityEvent<HoverCallback> onExit { get; private set; } = new UnityEvent<HoverCallback>();
        private void OnMouseEnter() {
            onEnter.Invoke(this);
        }

        private void OnMouseExit() {
            onExit.Invoke(this);
        }

        private void OnDestroy() {
            onExit.Invoke(this);
        }
    }
}