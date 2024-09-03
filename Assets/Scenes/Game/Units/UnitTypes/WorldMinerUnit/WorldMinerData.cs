using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World Miner Data", menuName = "Game/World Miner Data")]
public class WorldMinerData : TimedUnitData {
    [field: SerializeField] public List<ResourceData> targetResourceTypes { get; private set; } = new List<ResourceData>();
    [field: SerializeField] public float mineDistance { get; private set; }
    [field: SerializeField] public int minePower { get; private set; }
    [field: SerializeField] public float mineCooldown { get; private set; }
}