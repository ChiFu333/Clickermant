using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SFH;
using System.Runtime.CompilerServices;

public class UnitCard : MonoBehaviour {
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image icon;
    [SerializeField] private AudioQuerySO success, unsuccess;
    private UnitData unitData;
    public void Setup(UnitData _unitData) {
        unitData = _unitData;
        nameText.SetText(unitData.name);
        costText.SetText(unitData.manaCost.ToString());
        icon.sprite = unitData.icon;
    }

    public void Buy() {
        bool T = true;
        if (unitData.prefab is MeleeWarriorUnit warrior)
        {
            if(!HubManager.inst.IsThereEnoughtSpaceForPutInHub(warrior)) 
            {
                T = false;
            }
        }
        if (unitData.prefab is HomeWorker homeWokrer)
        {
            if(!HubManager.inst.IsThereEnoughtSpaceForPutInHub(homeWokrer)) 
            {
                T = false;
            }
        }
        else if(unitData.prefab is Researcher researcher)
        {
            if(!HubManager.inst.IsThereEnoughtSpaceForPutInHub(researcher)) T = false;
        }
        
        if(T && ManaManager.inst.TryConsumeMana(unitData.manaCost)) 
        {
            AudioManager.inst.PlayQuery(success);
            UnitBehaviour unitBehaviour = UnitManager.inst.CreateUnit(unitData);
            if (unitBehaviour is Researcher || unitBehaviour is HomeWorker) HubManager.inst.PutAndPlaceNear((TimedUnit)unitBehaviour);
            else unitBehaviour.transform.position = BuildingManager.inst.GetSummonPosition();
        }
        else
        {
            AudioManager.inst.PlayQuery(unsuccess);
        }
    }
}
