using DG.Tweening;
using UnityEngine;

public class ResourceBuilding : BuildingBehaviour, IMineable {
    public ResourceBuildingData resourceCastedData { get; private set; }
    private int workLeft;

    public void Mine(int power) {
        workLeft -= power;
        if (workLeft <= 0) {
            FullyMined();
        } else {
            Vector2 initialScale = transform.localScale;
            transform.DOShakeScale(0.3f, initialScale * .1f,20).OnComplete(() => transform.DOScale(initialScale, 0.1f));
        }
    }

    private void FullyMined() {
        ResourceManager.inst.RemoveResourceBuilding(resourceCastedData.resource.data, this);
        ResourceManager.inst.AddResource(resourceCastedData.resource);
        ResourceSpawner.inst.ResourceDestroyed(this);
        WorldGrid.inst.TryRemoveBuilding(WorldGrid.inst.GetCellAtWorld((Vector2)transform.position - data.offset));
        Destroy(gameObject);
    }

    private void Awake() {
        resourceCastedData = (ResourceBuildingData)data;
        ResourceManager.inst.AddResourceBuilding(resourceCastedData.resource.data, this);
        workLeft = resourceCastedData.workToMine;
        GetComponent<SpriteRenderer>().flipX = Random.value > 0.5f;
    }
}
