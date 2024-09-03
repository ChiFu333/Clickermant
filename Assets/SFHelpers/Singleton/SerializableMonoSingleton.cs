using Sirenix.OdinInspector;

namespace SFH {
    public abstract class SerializableMonoSingleton<T> : SerializedMonoBehaviour where T : SerializableMonoSingleton<T> {
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