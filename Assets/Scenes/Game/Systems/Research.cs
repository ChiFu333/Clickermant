using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Research
{
    [field: SerializeField] public BonusType type { get; private set; } 
    
    [field: ShowIf("type", BonusType.OpenUnit)] 
    [field: SerializeField] public UnitData unitData { get; private set; }
    [field: ShowIf("type", BonusType.OpenBuilding)] 
    [field: SerializeField] public BuildingData buildingData { get; private set; }
    
    [field: ShowIf("type", BonusType.PlusBonus)] [field: SerializeField] public string plusBonusCode { get; private set; }
    [field: ShowIf("type", BonusType.PlusBonus)] [field: SerializeField] public int plusBonus { get; private set; }
    
    [field: ShowIf("type", BonusType.MultiBonus)] [field: SerializeField] public string multiBonusCode { get; private set; }
    [field: ShowIf("type", BonusType.MultiBonus)] [field: SerializeField] public float multiBonus { get; private set; }
    
    
    public enum BonusType {
        OpenUnit,
        OpenBuilding,
        PlusBonus,
        MultiBonus
    }
}
