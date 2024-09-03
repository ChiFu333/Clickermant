using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildin Data", menuName = "Game/Building Data")]
public class BuildingData : ScriptableObject {
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public Vector2Int size { get; private set; }
    [field: SerializeField] public Vector2 offset { get; private set; }
    [field: SerializeField] public BuildingBehaviour buildingPrefab { get; private set; }
    [field: SerializeField] public List<Resource> cost { get; private set; } = new List<Resource>();
}