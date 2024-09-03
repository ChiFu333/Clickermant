
using UnityEngine;

[CreateAssetMenu(fileName = "Hub Building Data", menuName = "Game/Hub Building Data")]
public class HubBuildingData : DestructableBuildingData {
    [field: SerializeField] public int unitCapacity { get; private set; }
    [field:SerializeField] public UnitData applicableUnits { get; private set; }
}