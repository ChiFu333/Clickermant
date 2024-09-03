using UnityEngine;
[CreateAssetMenu(fileName = "Resource Data", menuName = "Game/Resource Data")]
public class ResourceData : ScriptableObject {
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public string resourceName { get; private set; }
}
