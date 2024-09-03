
public class WorldCell {
    private BuildingBehaviour building;
    private bool isOccupied;

    #region Set

    public void SetBuilding(BuildingBehaviour _building) {
        building = _building;
        isOccupied = true;
    }

    public void RemoveBuilding() {
        building = null;
        isOccupied = false;
    }

    #endregion

    #region Get

    public BuildingBehaviour GetBuilding() => building;

    public bool IsOccupied() {
        return isOccupied;
    }

    public bool IsFree() => !IsOccupied();

    #endregion

    public WorldCell() {
        isOccupied = false;
        building = null;
    }
}