using UnityEngine;
namespace SFH {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
        public static T inst { get; private set; }
        private void Awake() {
            if (inst != null) {
                Destroy(this);
                return;
            }
            inst = this as T;
            SingletonAwake();
        }

        protected virtual void SingletonAwake() { }
    }
}