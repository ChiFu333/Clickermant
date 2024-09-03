using UnityEngine;

[CreateAssetMenu(fileName = "Pathed Unit Data", menuName = "Game/Pathed Unit Data")]
public class PathedUnitData : UnitData {
    [field: SerializeField] public float movementSpeed { get; private set; }
}