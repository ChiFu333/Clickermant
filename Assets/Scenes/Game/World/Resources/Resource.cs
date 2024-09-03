using UnityEngine;

[System.Serializable]
public class Resource {
    [field: SerializeField] public ResourceData data { get; private set; }
    [field: SerializeField] public int count { get; private set; }

    public Resource(ResourceData _data, int _count) {
        data = _data;
        count = _count;
    }
}
