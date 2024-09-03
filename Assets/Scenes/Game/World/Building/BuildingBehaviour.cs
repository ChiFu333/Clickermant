using SFH;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class BuildingBehaviour : MonoBehaviour {
    [field: SerializeField] public BuildingData data { get; private set; }
    public UnityEvent isDestroyed;
    public List<Vector2Int> cellsOccupied = new List<Vector2Int>();

    public Vector2Int? GetClosestCellPosition() {
        Vector2Int cellPosition = WorldGrid.inst.GetCellAtWorld(transform.position);
        float xWidth = (data.size.x + 1) / 2.0f;
        float yHeight = (data.size.y + 1) / 2.0f;
        List<Vector2Int> freePositions = new List<Vector2Int>();
        for (int x = -Mathf.FloorToInt(xWidth); x < Mathf.FloorToInt(xWidth) + 1; x++) {
            for (int y = -Mathf.FloorToInt(yHeight); y < Mathf.FloorToInt(yHeight) + 1; y++) {
                Vector2Int samplePosition = cellPosition + new Vector2Int(x, y);
                if (WorldGrid.inst.worldGrid.isPositionValid(samplePosition)) {
                    if (WorldGrid.inst.worldGrid.GetValueAt(samplePosition).IsFree()) freePositions.Add(samplePosition);
                }
            }
        }
        if (freePositions.Count > 0) {
            return freePositions.RandomItem();
        }
        //Failed
        return null;
    }

    public Vector2? GetClosestPosition() {
        Vector2Int? position = GetClosestCellPosition();
        if (position == null) return null;
        return WorldGrid.inst.worldGrid.GetWorldPosition(position.Value);
    }

    public virtual void Deconstruct() {
        
    }
}