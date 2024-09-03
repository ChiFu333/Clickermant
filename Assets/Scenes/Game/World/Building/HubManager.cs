using System.Collections.Generic;
using UnityEngine;
using SFH;
public class HubManager : SerializableMonoSingleton<HubManager> {
    [SerializeField] private List<UnitData> hubsForUnits = new List<UnitData>();
    private readonly Dictionary<UnitData, List<HubBuilding>> hubsAvailable = new Dictionary<UnitData, List<HubBuilding>>();

    public void RegisterHub(HubBuilding hub) {
        hubsAvailable[hub.hubCastedData.applicableUnits].Add(hub);
    }

    public void UnregisterHub(HubBuilding hub) {
        hubsAvailable[hub.hubCastedData.applicableUnits].Remove(hub);
    }

    public void TryPutInHub(IHubUnit unit) {
        List<HubBuilding> hubs = hubsAvailable[unit.GetData()];
        for (int i = 0; i < hubs.Count; i++) {
            if (hubs[i].getFreePlacesCount() > 0) {
                hubs[i].unitsHere.Add(unit);
                unit.ConnectToHub(hubs[i]);
                return;
            }
        }
    }
    public bool IsThereEnoughtSpaceForPutInHub(UnitBehaviour unit) {
        List<HubBuilding> hubs = hubsAvailable[unit.data];
        for (int i = 0; i < hubs.Count; i++) {
            if (hubs[i].getFreePlacesCount() > 0) {
                return true;
            }
        }
        return false;
    }
    public void PutAndPlaceNear(TimedUnit unit)
    {
        List<HubBuilding> hubs = hubsAvailable[unit.data];
        for (int i = 0; i < hubs.Count; i++) {
            if (hubs[i].getFreePlacesCount() > 0) {
                hubs[i].unitsHere.Add(unit);
                unit.ConnectToHub(hubs[i]);
                hubs[i].GetComponent<StandingHutPlaces>().TryPutUnit(unit);
                return;
            }
        }
    }
    protected override void SingletonAwake() {
        foreach (UnitData kvp in hubsForUnits) {
            hubsAvailable.Add(kvp, new List<HubBuilding>());
        }
    }
}
