using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Data", menuName = "Game/Unit Data")]
public class UnitData : SerializedScriptableObject {
    [field: SerializeField] public Sprite icon;
    [field: SerializeField] public int maxHealth { get; private set; }
    [field: SerializeField] public Fraction fraction { get; private set; }
    [field: SerializeField] public UnitBehaviour prefab { get; private set; }
    [field: SerializeField] public int manaCost { get; private set; }
    [field: SerializeField] public List<Resource> cost { get; private set; } = new List<Resource>();

    public enum Fraction {
        Player,
        Enemy
    }
}