using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ResearchData", menuName = "Game/Research Data")]
public class ResearchSO : ScriptableObject
{
    [field: SerializeField] public Sprite icon { get; private set; }
    [field: SerializeField] public string bonusName { get; private set; }
    [field: SerializeField] public int researchCost { get; private set; }
    [field: SerializeField] public List<Research> bonuses { get; private set; }
}
