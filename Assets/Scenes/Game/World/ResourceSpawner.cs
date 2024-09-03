using System.Collections.Generic;
using UnityEngine;
using SFH;
using static GridRegions;
using UnityEngine.UIElements;

public class ResourceSpawner : SerializableMonoSingleton<ResourceSpawner> {
    [SerializeField] private float resourceRespawnTime;
    [SerializeField] private float resourceRespawnTimeRandomness;
    [SerializeField] private Dictionary<RegionType, List<ResourceBuildingData>> regionBuildingsLookup = new Dictionary<RegionType, List<ResourceBuildingData>>();
    [SerializeField] private Dictionary<ResourceBuildingData, int> buildingsDesiredCount = new Dictionary<ResourceBuildingData, int>();
    private readonly Dictionary<RegionType, List<Vector2Int>> regionSpawnPoints = new Dictionary<RegionType, List<Vector2Int>>();
    private readonly Dictionary<ResourceBuildingData, int> buildingsCount = new Dictionary<ResourceBuildingData, int>();
    //Resource timers
    private readonly Dictionary<ResourceBuildingData, RegionType> regionPerResource = new Dictionary<ResourceBuildingData,RegionType>();
    private readonly Dictionary<ResourceBuildingData, Timer> resourceTimersLookup = new Dictionary<ResourceBuildingData, Timer>();
    private readonly Dictionary<ResourceBuildingData, bool> resourceNeedSpawnLookup = new Dictionary<ResourceBuildingData, bool>();

    private void HandleRegionUpdate(RegionType region) {
        for (int i = 0; i < regionBuildingsLookup[region].Count; i++) {
            //For each building
            ResourceBuildingData data = regionBuildingsLookup[region][i];
            if (!buildingsCount.ContainsKey(data)) {
                buildingsCount.Add(data, 0);
            }
            int delta = buildingsDesiredCount[data] - buildingsCount[data];
            if (delta > 0) {
                //Spawn building
                if (!resourceNeedSpawnLookup[data]) {
                    resourceTimersLookup[data].SetTime(resourceRespawnTime + (Random.value * 2 - 1) * resourceRespawnTimeRandomness);
                    resourceNeedSpawnLookup[data] = true;
                }
            }
        }
    }

    public void ResourceDestroyed(ResourceBuilding resourceBuilding) {
        ResourceBuildingData data = resourceBuilding.resourceCastedData;
        buildingsCount[data]--;
        Vector2Int position = WorldGrid.inst.GetCellAtWorld((Vector2)resourceBuilding.transform.position - data.offset);
        RegionType region = GridRegions.inst.region[position.x, position.y];
        //Add regionSpawnPoints
        for (int x = 0; x < data.size.x; x++) {
            for (int y = 0; y < data.size.y; y++) {
                Vector2Int actualPosition = position + new Vector2Int(x,y);
                if (!WorldGrid.inst.worldGrid.isPositionValid(actualPosition)) { Debug.LogWarning("Inavlid position when restoring resource position shoud not be possible"); continue; }
                if (regionSpawnPoints.ContainsKey(region)) {
                    regionSpawnPoints[region].Add(actualPosition);
                }
            }
        }
    }

    #region Internal

    private Vector2Int? ReserveRandomPositionForBuilding(RegionType region, ResourceBuildingData data) {
        Vector2Int position = GetRandomPositionInRegion(region);
        List<Vector2Int> occupiedPositions = new List<Vector2Int>();
        for (int x = 0; x < data.size.x; x++) {
            for (int y = 0; y < data.size.y; y++) {
                Vector2Int actualPosition = position + new Vector2Int(x,y);
                if (!WorldGrid.inst.worldGrid.isPositionValid(actualPosition)) return null;
                WorldCell cell = WorldGrid.inst.worldGrid.GetValueAt(actualPosition);
                if (cell.IsOccupied()) return null;
                occupiedPositions.Add(actualPosition);
            }
        }
        for (int i = 0; i < occupiedPositions.Count; i++) {
            regionSpawnPoints[region].Remove(occupiedPositions[i]);
        }
        return position;
    }

    private Vector2Int GetRandomPositionInRegion(RegionType region) {
        Vector2Int position = regionSpawnPoints[region].RandomItem();
        return position;
    }

    private void Update() {
        foreach (KeyValuePair<ResourceBuildingData, Timer> kvp in resourceTimersLookup) {
            if (resourceNeedSpawnLookup[kvp.Key] && kvp.Value.Execute()) {
                ResourceBuildingData data = kvp.Key;
                //Check delta
                Vector2Int? position = ReserveRandomPositionForBuilding(regionPerResource[data], data);
                if (position == null) continue;
                bool success = WorldGrid.inst.TryPlaceBuilding(data, position.Value);
                if (!buildingsCount.ContainsKey(data)) {
                    buildingsCount.Add(data, 0);
                }
                if (success) {
                    buildingsCount[data]++;
                }
                int delta = buildingsDesiredCount[data] - buildingsCount[data];
                if (delta > 0) {
                    kvp.Value.SetTime(resourceRespawnTime + (Random.value * 2 - 1) * resourceRespawnTimeRandomness);
                } else {
                    resourceNeedSpawnLookup[data] = false;
                }
            }
        }
        foreach (KeyValuePair<RegionType, List<ResourceBuildingData>> kvp in regionBuildingsLookup) {
            HandleRegionUpdate(kvp.Key);
        }
    }

    private void Start() {
        //Collect region cells
        foreach (KeyValuePair<RegionType, List<ResourceBuildingData>> kvp in regionBuildingsLookup) {
            regionSpawnPoints.Add(kvp.Key, new List<Vector2Int>());
            for (int i = 0; i < kvp.Value.Count; i++) {
                regionPerResource.Add(kvp.Value[i], kvp.Key);
            }
        }
        for (int x = 0; x < WorldGrid.inst.worldGridWidth; x++) {
            for (int y = 0; y < WorldGrid.inst.worldGridHeight; y++) {
                RegionType regionType = GridRegions.inst.region[x,y];
                if (!regionSpawnPoints.ContainsKey(regionType)) continue;
                regionSpawnPoints[regionType].Add(new Vector2Int(x, y));
            }
        }
        foreach (KeyValuePair<ResourceBuildingData, int> kvp in buildingsDesiredCount) {
            resourceTimersLookup.Add(kvp.Key, new Timer());
            resourceNeedSpawnLookup.Add(kvp.Key, true);
        }

        //Spawn initial resources
        foreach (KeyValuePair<RegionType, List<ResourceBuildingData>> kvp in regionBuildingsLookup) {
            RegionType region = kvp.Key;
            for (int i = 0; i < regionBuildingsLookup[region].Count; i++) {
                //For each building
                ResourceBuildingData data = regionBuildingsLookup[region][i];
                if (!buildingsCount.ContainsKey(data)) {
                    buildingsCount.Add(data, 0);
                }
                int delta = buildingsDesiredCount[data] - buildingsCount[data];
                for (int k = 0; k < delta; k++) {
                    //Spawn building
                    Vector2Int? position = ReserveRandomPositionForBuilding(regionPerResource[data], data);
                    if (position == null) continue;
                    bool success = WorldGrid.inst.TryPlaceBuilding(data, position.Value);
                    if (!buildingsCount.ContainsKey(data)) {
                        buildingsCount.Add(data, 0);
                    }
                    if (success) {
                        buildingsCount[data]++;
                    }
                }
            }
        }
    }

    #endregion

}
