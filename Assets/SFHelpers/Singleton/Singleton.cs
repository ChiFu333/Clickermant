
namespace SFH {
    public abstract class Singleton<T> where T : class, new() {
        private static T instance;
        private static readonly object _lock = new object();

        public static T inst {
            get {
                if (instance == null) {
                    lock (_lock) {
                        if (instance == null) {
                            instance = new T();
                        }
                    }
                }
                return instance;
            }
        }
    }
}