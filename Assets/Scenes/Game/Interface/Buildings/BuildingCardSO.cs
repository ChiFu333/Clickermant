using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BuildCardData", menuName = "Game/BuildingCard")]
public class BuildingCardSO : ScriptableObject {
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public string buildName { get; private set; }
    [Header("Cost")]
    [field: SerializeField] public List<Resource> resCosts { get; private set; }
    [Header("Home")]
    [field: SerializeField] public BuildingData buildData { get; private set; }
}
