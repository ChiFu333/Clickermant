using SFH;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoSingleton<BuildingManager> {
    [SerializeField] private SpriteRenderer ghostPrefab;
    [SerializeField] private BuildingBehaviour hubPrefab;
    [SerializeField] private float ghostSnapSmoothness;
    [field: SerializeField] public List<BuildingData> buildingAvailableFromStart { get; private set; } = new List<BuildingData>();
    [SerializeField] private List<InitialBuilding> initialBuildings = new List<InitialBuilding>();
    [SerializeField] private AudioQuerySO buildSound, cancelSound, breakSound;
    //Internal
    private readonly List<BuildingData> buildingAvailable = new List<BuildingData>();
    private ScreenUtils.ScreenUtilsInterface screen;
    public DestructableBuilding hutBehaviour { get; private set; }
    //Build mode
    private BuildingData selectedBuilding;
    public bool inBuildMode = false;
    public bool inDeconstructionMode = false;
    private GameObject ghost;

    public Vector2 GetSummonPosition() {
        return Vector2.zero;
    }

    public void SelectBuilding(BuildingData building) {
        selectedBuilding = building;
        ghost = null;
        DisplayGhost();
    }

    public void ToggleDeconstructionMode() {
        inDeconstructionMode = !inDeconstructionMode;
        if (inDeconstructionMode) {
            inBuildMode = false;
        }
    }

    private void DisplayGhost() {
        if (ghost == null) {
            ghost = Instantiate(ghostPrefab.gameObject);
            ghost.GetComponent<SpriteRenderer>().sprite = selectedBuilding.buildingPrefab.GetComponent<SpriteRenderer>().sprite;
            ghost.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            ghost.transform.localScale = selectedBuilding.buildingPrefab.transform.localScale;
            ghost.transform.position = screen.WorldMouse();
        }
        Vector2Int mouseGridPosition = GetGridMousePosition()/*WorldGrid.inst.GetCellAtWorld(screen.WorldMouse())*/;
        Vector2 targetPosition = WorldGrid.inst.worldGrid.GetWorldPosition(mouseGridPosition) + selectedBuilding.offset;
        ghost.transform.position = Vector2.Lerp(ghost.transform.position, targetPosition, ghostSnapSmoothness * Time.deltaTime);
    }

    private void HandleBuilding() {
        //Obtain mouse position
        Vector2Int position = GetGridMousePosition();
        bool success = WorldGrid.inst.TryPlaceBuilding(selectedBuilding, position);
        if (success) {
            AudioManager.inst.PlayQuery(buildSound);
            ResourceManager.inst.ConsumeResources(selectedBuilding.cost);
            inBuildMode = false;
            selectedBuilding = null;
        }
    }

    private void HandleBuildingRemoval() {
        Vector2Int position = GetGridMousePosition();
        BuildingBehaviour buildingBehaviour = WorldGrid.inst.GetBuilding(position);
        //Check if building can be removed (it belongs to player & it is not hub)
        if (buildingBehaviour!= null 
            && buildingBehaviour.data is not ResourceBuildingData 
            && buildingBehaviour.data.buildingPrefab != hubPrefab) {
            WorldGrid.inst.TryRemoveBuilding(position);
            AudioManager.inst.PlayQuery(breakSound);
        }
    }

    #region Internal

    private void Update() {
        if ((!inBuildMode && !inDeconstructionMode) || ScreenUtils.GetUIUnderMouse().Count > 0) {
            if (ghost != null) {
                Destroy(ghost);
                ghost = null;
            }
            return;
        }
        if (!inDeconstructionMode) DisplayGhost();
        if (Input.GetMouseButtonDown(0)) {
            if (inDeconstructionMode) {
                HandleBuildingRemoval();
            } else {
                HandleBuilding();
            }
        }
        if (Input.GetMouseButtonDown(1)) {
            AudioManager.inst.PlayQuery(cancelSound);
            inBuildMode = false;
            inDeconstructionMode = false;
        }
    }

    private Vector2Int GetGridMousePosition() {
        Vector2 offset = selectedBuilding != null ? selectedBuilding.offset : Vector2.zero;
        Vector2 worldMouseOffsetted = screen.WorldMouse() - WorldGrid.inst.worldGrid.GetCellSize() / 2 - offset;
        return WorldGrid.inst.worldGrid.GetCellAtWorld(worldMouseOffsetted);
    }

    private void Start() {
        screen = ScreenUtils.inst[Camera.main];
        //Spawn initial buildings
        for (int i = 0; i < initialBuildings.Count; i++) {
            WorldGrid.inst.TryPlaceBuilding(initialBuildings[i].buildingData, initialBuildings[i].cellPosition);
        }
        //Set hut lose condition
        hutBehaviour = (DestructableBuilding)WorldGrid.inst.GetBuilding(new Vector2Int(15,10));
        
        hutBehaviour.deathEvent.AddListener(()=> {
            GameStateManager.inst.GameOver();
        });
    }

    protected override void SingletonAwake() {
        buildingAvailable.AddRange(buildingAvailableFromStart);
    }

    [System.Serializable]
    private class InitialBuilding {
        public BuildingData buildingData;
        public Vector2Int cellPosition;
    }

    #endregion
}
