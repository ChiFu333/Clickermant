using System.Collections.Generic;
using UnityEngine;

public class PathedUnit : UnitBehaviour {
    private const float minimumDistance = 0.025f;
    private PathedUnitData castedData;
    //Pathing
    protected List<Vector2> path;
    protected bool isPathFinished = true;

    private int pathIndex;

    public void MoveTo(Vector2 position) {
        List<Vector2> newPath = UnitManager.inst.FindPath(this, position);
        if (newPath != null && newPath.Count > 0) {
            path = newPath;
            pathIndex = 0;
            isPathFinished = false;
        }
    }

    private void HandlePathing() {
        if (!isPathFinished && path != null) {
            Vector2 direction = (path[pathIndex] - (Vector2)transform.position).normalized;
            float realSpeed = castedData.movementSpeed;
            if(castedData.fraction == UnitData.Fraction.Player)
            {
                realSpeed += BonusStorage.inst.addBonuses["SpeedUp"];
            }
            transform.position += realSpeed * Time.deltaTime * (Vector3)direction;
            //Animate flipping
            if (spriteRenderer != null) {
                spriteRenderer.flipX = Vector2.Dot(direction, Vector2.up) > 0.9f || direction.x < -0.001f;
            }
            //Set next path target
            if (Vector2.Distance(transform.position, path[pathIndex]) <= minimumDistance) {
                if (pathIndex + 1 >= path.Count) {
                    //Pathing is finished
                    isPathFinished = true;
                } else {
                    pathIndex++;
                }
            }
        }
    }

    protected override void Awake() {
        base.Awake();
        castedData = (PathedUnitData)data;
    }

    protected override void Update() {
        base.Update();
        HandlePathing();
    }

    /*private void OnDrawGizmos() {
        Vector2 direction = (path[pathIndex] - (Vector2)transform.position).normalized;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction * 2);
    }*/
}