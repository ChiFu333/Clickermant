using System.Collections.Generic;

public class HubBuilding : DestructableBuilding {
     public List<IHubUnit> unitsHere = new List<IHubUnit>();
     public HubBuildingData hubCastedData;

    public int getFreePlacesCount() {
        return hubCastedData.unitCapacity - unitsHere.Count;
    }

    protected override void Awake() {
        base.Awake();
        hubCastedData = (HubBuildingData)data;
    }

    public override void Die() {
        RemoveHub();
        base.Die();
    }

    public override void Deconstruct() {
        RemoveHub();
        base.Deconstruct();
    }

    private void RemoveHub() {
        HubManager.inst.UnregisterHub(this);
        for (int i = 0; i < unitsHere.Count; i++) {
            unitsHere[i].DisconnectFromHub();
            if (unitsHere[i] is WarriorUnit warrior) {
                warrior.ForceDie();
            }
        }

    }

}
