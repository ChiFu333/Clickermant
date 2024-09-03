using SFH;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnitManager : MonoSingleton<UnitManager> {
    [SerializeField] private Transform unitsHolder;
    [SerializeField] private float enemyVisionRadius;
    [SerializeField] private string alliedLayerMask;
    [SerializeField] private string enemyLayerMask;
    [SerializeField] private float randomOffsetRadius;
    [SerializeField] private BuildingData hutData;

    [SerializeField] LayerMask _mask;
    private readonly Pathfinder pathfinder = new Pathfinder();
    private Grid<bool> pathfinderGrid;

    private readonly List<WarriorUnit> enemyWarriors = new List<WarriorUnit>();
    private readonly List<WarriorUnit> alliedWarriors = new List<WarriorUnit>();

    #region Creation and destruction

    public UnitBehaviour CreateUnit(UnitData unitData) {
        UnitBehaviour instance = Instantiate(unitData.prefab.gameObject, unitsHolder, false).GetComponent<UnitBehaviour>();
        if (unitData.fraction == UnitData.Fraction.Player) {
            if (instance is Researcher || instance is HomeWorker) { } else if (instance is IHubUnit unit) HubManager.inst.TryPutInHub(unit);
            if (instance is WarriorUnit warrior) alliedWarriors.Add(warrior);

            instance.gameObject.layer = LayerMask.NameToLayer(alliedLayerMask);

        } else {
            if (instance is WarriorUnit warrior) enemyWarriors.Add(warrior);
            instance.gameObject.layer = LayerMask.NameToLayer(enemyLayerMask);
        }
        //Animate unit
        Vector2 baseScale = instance.transform.localScale;
        instance.transform.localScale = Vector3.zero;
        instance.transform.DOScale(baseScale,0.1f);

        return instance;
    }

    public void DestroyUnit(UnitBehaviour unit) {
        if (unit is WarriorUnit warrior) {
            if (unit.data.fraction == UnitData.Fraction.Player) {
                alliedWarriors.Remove(warrior);
            } else {
                enemyWarriors.Remove(warrior);
                EnemyTriggerSpawner.inst.RemoveUnit(warrior.data);
            }
        }
        Destroy(unit.gameObject);
    }

    #endregion

    public void UpdateEncounter() {
        //Updates encounter on current scene
        //Has list of enemies, list of allies

        //Assigns allies to enemies based on sole numbers
        if (enemyWarriors.Count == 0) return;
        for (int i = 0; i < alliedWarriors.Count; i++) {
            if (alliedWarriors[i].attackTarget == null || alliedWarriors[i].attackTarget.GetObject() == null) {
                WarriorUnit targetEnemy = enemyWarriors.RandomItem();
                alliedWarriors[i].Attack(targetEnemy);
            } else {
                continue;
            }
        }
    }

    private void UpdateEnemyLogic() {
        //Each frame look for targets in range
        //If found warrior - override any attack
        //If found building - override only if there is no warrior
        //If found nothing - attack hut
        for (int i = 0; i < enemyWarriors.Count; i++) {
            WarriorUnit unit = enemyWarriors[i];
            if (IsTargetWarrior(unit)) continue;
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(unit.transform.position, enemyVisionRadius, _mask);
            //There is no warrior target, try find new warrior unit
            for (int k = 0; k < overlaps.Length; k++) {
                if (overlaps[k].TryGetComponent<WarriorUnit>(out var alliedUnit)) {
                    //Target allied unit
                    unit.Attack(alliedUnit);
                    break;
                }
            }
            if (IsTargetWarrior(unit)) continue;
            if (IsTargetBuilding(unit)) continue;
            //There is no warrior units in range, try find building
            for (int k = 0; k < overlaps.Length; k++) {
                if (overlaps[k].TryGetComponent<DestructableBuilding>(out var alliedBuilding)) {
                    //Target allied building
                    unit.Attack(alliedBuilding);
                    break;
                }
            }
            if (IsTargetBuilding(unit)) continue;
            //There is no building, go attack hut
            unit.Attack(BuildingManager.inst.hutBehaviour);
        }
    }

    private bool IsTargetWarrior(WarriorUnit unit) {
        return unit.attackTarget != null && unit.attackTarget.GetObject() != null && unit.attackTarget.GetObject().GetComponent<WarriorUnit>() != null;
    }

    private bool IsTargetBuilding(WarriorUnit unit) {
        return unit.attackTarget != null && unit.attackTarget.GetObject() != null
            && unit.attackTarget.GetObject().GetComponent<DestructableBuilding>() != null
            && unit.attackTarget.GetObject().GetComponent<DestructableBuilding>().data != hutData;
    }

    #region Pathing

    /// <summary>Finds path in world space</summary>
    public List<Vector2> FindPath(UnitBehaviour unit, Vector2 position) {
        //Check if position is on grid
        Vector2Int cellPosition = WorldGrid.inst.GetCellAtWorld(position);
        if (!WorldGrid.inst.worldGrid.isPositionValid(cellPosition)) return null;
        List<Vector2Int> cellPath = FindPath(unit, cellPosition);

        return cellPath.Select(i => {
            Vector2 randomOffset = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1).normalized;
            return WorldGrid.inst.worldGrid.GetWorldPosition(i) + randomOffset * randomOffsetRadius;
        }).ToList();
    }
    /// <summary>Finds path in cell space</summary>
    private List<Vector2Int> FindPath(UnitBehaviour unit, Vector2Int cellPosition) {
        Vector2Int startCellPosition = WorldGrid.inst.GetCellAtWorld(unit.transform.position);
        //Try pathfind from point to point
        List<Vector2Int> gridPath = pathfinder.FindPath(startCellPosition, cellPosition);
        return gridPath;
    }

    public void UpdatePathfinderGrid(Grid<WorldCell> grid) {
        int width = WorldGrid.inst.worldGridWidth;
        int height = WorldGrid.inst.worldGridHeight;
        pathfinderGrid = new Grid<bool>(width, height, grid.GetCellSize());
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                bool isOccupied = grid.GetValueAt(x,y).IsOccupied();
                pathfinderGrid.SetValueAt(x, y, !isOccupied);
            }
        }
        pathfinder.SetGrid(pathfinderGrid);
    }

    #endregion

    #region Internal

    private void Update() {
        if (GameStateManager.inst.gameState == GameStateManager.State.Over) return;
        UpdateEnemyLogic();
        UpdateEncounter();
    }

    private void Start() {
        WorldGrid.inst.gridUpdated.AddListener(UpdatePathfinderGrid);
    }

    #endregion
}
