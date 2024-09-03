using System.Collections.Generic;
using UnityEngine;
using SFH;
using UnityEngine.Events;

public class ResourceManager : SerializableMonoSingleton<ResourceManager> {

    #region Resource management

    [SerializeField] private Dictionary<ResourceData, int> resources = new Dictionary<ResourceData, int>();
    [HideInInspector] public Dictionary<ResourceData, UnityEvent> actionsLookup = new Dictionary<ResourceData, UnityEvent>();
    [SerializeField] private int woodAddedThreshold;
    private int woodAdded = 0;


    public int GetResource(ResourceData resource) {
        return resources[resource];
    }
    public void AddResource(Resource resource) {
        resources[resource.data] += resource.count;
        if (resource.count > 0) { 
            woodAdded += resource.count;
            if (woodAdded >= woodAddedThreshold) {
                //Spawn ents
                woodAdded = 0;
                //EnemyTriggerSpawner.inst.SpawnEnt();
            }
        }

        actionsLookup[resource.data]?.Invoke();
    }

    public bool IsEnoughResources(List<Resource> cost) {
        for (int i = 0; i < cost.Count; i++) {
            if (GetResource(cost[i].data) - cost[i].count < 0) return false;
        }
        return true;
    }

    public void ConsumeResources(List<Resource> cost) {
        for (int i = 0; i < cost.Count; i++) {
            AddResource(new Resource(cost[i].data, -cost[i].count));
        }
    }

    #endregion

    #region Resource buildings

    private readonly Dictionary<ResourceData, List<ResourceBuilding>> availableResourceBuildings = new Dictionary<ResourceData, List<ResourceBuilding>>();

    public void AddResourceBuilding(ResourceData resource, ResourceBuilding building) {
        availableResourceBuildings[resource].Add(building);
    }

    public void RemoveResourceBuilding(ResourceData resource, ResourceBuilding building) {
        availableResourceBuildings[resource].Remove(building);
    }

    public void ReturnResourceBuilding(ResourceBuilding building) {
        availableResourceBuildings[building.resourceCastedData.resource.data].Add(building);
    }

    public ResourceBuilding GetResourceBuilding(ResourceData resource) {
        List<ResourceBuilding> resourceBuildings = availableResourceBuildings[resource];
        if (resourceBuildings == null || resourceBuildings.Count == 0) return null;
        ResourceBuilding selectedBuilding = resourceBuildings.RandomItem();
        resourceBuildings.Remove(selectedBuilding);
        return selectedBuilding;
    }

    #endregion

    protected override void SingletonAwake() {
        foreach (KeyValuePair<ResourceData, int> kvp in resources) {
            //Construct actions lookup
            actionsLookup.Add(kvp.Key, new UnityEvent());
            //Create resource building lookup
            availableResourceBuildings.Add(kvp.Key, new List<ResourceBuilding>());
        }
    }
}