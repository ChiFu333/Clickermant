
using UnityEngine;

[CreateAssetMenu(fileName = "Resource Buildin Data", menuName = "Game/Resource Building Data")]
public class ResourceBuildingData : BuildingData {
    [field:SerializeField] public Resource resource { get; private set; }
    [field:SerializeField] public int workToMine { get; private set; }
}