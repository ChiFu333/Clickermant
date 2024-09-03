using System.Collections.Generic;
using UnityEngine;

namespace SFH {
    public class GameObjectPool<T> where T : MonoBehaviour, IPoolable {
        private readonly List<T> releasedObjects = new List<T>();
        private readonly List<T> usedObjects = new List<T>();
        private readonly T prefab;
        public GameObjectPool(T objectPrefab) {
            prefab = objectPrefab;
        }

        public T Get() {
            if (releasedObjects.IsEmpty()) {
                //Create new object, move into used list, and return it
                T newObject = GameObject.Instantiate(prefab);
                usedObjects.Add(newObject);
                return newObject;
            } else {
                //Return object from released ones, activate it and move into used list
                T reusedObject = releasedObjects.GetLast();
                releasedObjects.RemoveAt(releasedObjects.Count - 1);
                reusedObject.gameObject.SetActive(true);
                reusedObject.OnReuse();
                usedObjects.Add(reusedObject);
                return reusedObject;
            }
        }

        public void Release(T releasedObject) {
            releasedObject.gameObject.SetActive(false);
            releasedObject.OnRelease();

            if (usedObjects.Contains(releasedObject)) {
                usedObjects.Remove(releasedObject);
            }
            releasedObjects.Add(releasedObject);
        }
    }
}