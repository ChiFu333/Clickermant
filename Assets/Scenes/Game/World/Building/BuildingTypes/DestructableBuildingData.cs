
using UnityEngine;

[CreateAssetMenu(fileName = "Destructable Building Data", menuName = "Game/Destructable Building Data")]
public class DestructableBuildingData : BuildingData {
    [field: SerializeField] public int health { get; private set; }
}