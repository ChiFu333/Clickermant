public interface IHubUnit {
    public UnitData GetData();
    public void ConnectToHub(HubBuilding hub);
    public void DisconnectFromHub();
}