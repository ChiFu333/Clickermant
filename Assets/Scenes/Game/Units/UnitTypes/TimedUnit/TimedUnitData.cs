using UnityEngine;

[CreateAssetMenu(fileName = "Timed Unit Data", menuName = "Game/Timed Unit Data")]
public class TimedUnitData : PathedUnitData {
    [Header("Timed unit")]
    [field: SerializeField] public float lifeTime { get; private set; }
    [field:SerializeField] public float restDuration { get; private set; }
    [field: SerializeField] public int restManaConsumption { get; private set; }
}