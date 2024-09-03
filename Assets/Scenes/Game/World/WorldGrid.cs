using DG.Tweening;
using SFH;
using UnityEngine;
using UnityEngine.Events;

public class WorldGrid : MonoSingleton<WorldGrid> {
    [field: SerializeField] public int worldGridWidth { get; private set; } = 32;
    [field: SerializeField] public int worldGridHeight { get; private set; } = 20;
    [SerializeField] private float cellSize;
    [SerializeField] private Transform gridObjectHolder;
    [SerializeField] private bool showDebug;
    public Grid<WorldCell> worldGrid { get; private set; }
    public UnityEvent<Grid<WorldCell>> gridUpdated { get; private set; } = new UnityEvent<Grid<WorldCell>>();

    #region Try modify
    public bool TryPlaceBuilding(BuildingData buildingData, Vector2Int position) {
        if (!worldGrid.isBoxValid(position, buildingData.size)) { Debug.Log("Position is invalid"); return false; } //Position is invalid
        //Check if position is not occupied
        for (int x = 0; x < buildingData.size.x; x++) {
            for (int y = 0; y < buildingData.size.y; y++) {
                if (!worldGrid.GetValueAt(position + new Vector2Int(x, y)).IsFree()) return false;
            }
        }
        //Debug.Log(buildingData.buildingPrefab);
        Build(buildingData.buildingPrefab, position);
        return true;
    }

    public bool TryRemoveBuilding(Vector2Int position) {
        BuildingBehaviour building = worldGrid.GetValueAt(position).GetBuilding();
        if (building == null) return false;
        if (!worldGrid.isBoxValid(position, building.data.size)) { Debug.Log("Position is invalid"); return false; }
        RemoveBuilding(position, building);
        return true;
    }
    #endregion

    #region Application 

    public BuildingBehaviour GetBuilding(Vector2Int position) {
        return worldGrid.GetValueAt(position).GetBuilding();
    }

    public Vector2? GetClosestEmptyToPosition(Vector2 positon) {
        Vector2Int pos = GetCellAtWorld(positon);
        if (worldGrid.GetValueAt(pos).IsFree()) return positon;
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                Vector2Int nPos = pos + new Vector2Int(x,y);
                if (worldGrid.GetValueAt(nPos).IsFree()) return worldGrid.GetWorldPosition(nPos);
            }
        }
        return null;
    }

    private void Build(BuildingBehaviour buildingPrefab, Vector2Int position) {
        //Create actual prefab
        BuildingBehaviour building = Instantiate(buildingPrefab.gameObject, gridObjectHolder, false).GetComponent<BuildingBehaviour>();
        building.transform.position = worldGrid.GetWorldPosition(position) + building.data.offset;
        if (building is not ResourceBuilding) {
            building.gameObject.layer = LayerMask.NameToLayer("Allied");
        }
        //Set cells to building
        BuildingData buildingData = building.data;
        for (int x = 0; x < buildingData.size.x; x++) {
            for (int y = 0; y < buildingData.size.y; y++) {
                Vector2Int pos = position + new Vector2Int(x, y);
                WorldCell cell = worldGrid.GetValueAt(pos);
                building.cellsOccupied.Add(pos);
                cell.SetBuilding(building);
            }
        }
        //AddHut
        if (building.gameObject.TryGetComponent<HubBuilding>(out var hb)) {
            HubManager.inst.RegisterHub(hb);
        }
        //Animate creation
        Vector2 baseScale = building.transform.localScale;
        building.transform.localScale = Vector2.zero;
        building.transform.DOScale(baseScale, 0.3f).SetEase(Ease.InOutBack).SetLink(building.gameObject);
        //Invoke event
        gridUpdated.Invoke(worldGrid);
    }

    private void RemoveBuilding(Vector2Int position, BuildingBehaviour building) {
        for (int i = 0; i < building.cellsOccupied.Count; i++) {
            WorldCell cell = worldGrid.GetValueAt(building.cellsOccupied[i]);
            cell.RemoveBuilding();
        }
        //Destroy world building
        building.Deconstruct();
        Destroy(building.gameObject);
        gridUpdated.Invoke(worldGrid);
    }

    #endregion

    #region Essentials

    public Vector2Int GetCellAtWorld(Vector2 position) {
        return worldGrid.GetCellAtWorld(position - worldGrid.GetCellSize() / 2);
    }

    #endregion

    #region Internal

    private Vector2 GetCellSize() {
        return Vector2.one * cellSize;
    }

    protected override void SingletonAwake() {
        worldGrid = new Grid<WorldCell>(worldGridWidth, worldGridHeight, GetCellSize());
        //Compute offset
        float gridWidth = worldGrid.GetCellSize().x * worldGridWidth;
        float gridHeight = worldGrid.GetCellSize().y * worldGridHeight;
        Vector2 gridOffset = -new Vector2(gridWidth / 2 - worldGrid.GetCellSize().x / 2,gridHeight / 2 - worldGrid.GetCellSize().y / 2);
        worldGrid.SetOrigin(gridOffset);
        //Set default values
        for (int x = 0; x < worldGridWidth; x++) {
            for (int y = 0; y < worldGridHeight; y++) {
                worldGrid.SetValueAt(x, y, new WorldCell());
            }
        }
    }

    private void Start() {
        gridUpdated.Invoke(worldGrid);
    }

    private void OnDrawGizmos() {
        if (showDebug && worldGrid != null) DrawGizmos();
    }

    private void DrawGizmos() {
        for (int x = 0; x < worldGridWidth; x++) {
            for (int y = 0; y < worldGridHeight; y++) {
                if (worldGrid.GetValueAt(x, y).IsOccupied()) {
                    Gizmos.color = Color.red;
                } else {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawCube(worldGrid.GetWorldPosition(x, y), worldGrid.GetCellSize() / 2.0f);
            }
        }
    }

    #endregion 
}
