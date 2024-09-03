using DG.Tweening;
using SFH;
using UnityEngine;

public class MeleeWarriorUnit : WarriorUnit, IHubUnit {
    private const float attackDistanceEpsilon = 0.01f;

    private MeleeWarriorData meleeCastedData;
    private readonly Timer attackTimer = new Timer();
    private State state = State.Idle;
    private Vector2 targetPosition;
    private Vector2 previousTargetPosition;
    private Vector2 hubPosition;
    private Vector2? position = null;
    protected HubBuilding hub;
    private bool IsPositionDynamic = false, isTPSet = false, isHubPosSet = false;

    private void HandleStateMachine() {
        if (attackTarget != null && attackTarget.GetObject() != null) {
            SetPosition();
        }
        switch (state) {
            case State.Idle:
                if (attackTarget != null && attackTarget.GetObject() != null) { state = State.Approaching; isPathFinished = true; break; }
                if (hub != null) {
                    if (!isHubPosSet) {
                        position = hub.GetClosestPosition();
                        isHubPosSet = true;
                    }
                    if (position != null) {
                        hubPosition = position.Value;
                        if (isPathFinished && Vector2.Distance(transform.position, hubPosition) > meleeCastedData.attackDistance) {
                            MoveTo(hubPosition);
                        }
                    }
                }
                break;
            case State.Approaching:
                if (attackTarget == null || attackTarget.GetObject() == null) { state = State.Idle; break; }
                if (Vector2.Distance(transform.position, targetPosition) < meleeCastedData.attackDistance) {
                    state = State.Attacking;
                } else {
                    //Move to target
                    if (isPathFinished) {
                        MoveTo(targetPosition);
                        previousTargetPosition = targetPosition;
                        attackTimer.SetTime(meleeCastedData.cooldown);
                    }
                }
                break;
            case State.Attacking:
                if (attackTarget == null || attackTarget.GetObject() == null) { state = State.Idle; break; }
                if (Vector2.Distance(transform.position, targetPosition) > meleeCastedData.attackDistance + attackDistanceEpsilon) {
                    state = State.Approaching;
                } else {
                    HandleAttack();
                }
                break;
        }
    }

    private void HandleAttack() {
        if (attackTimer.Execute()) {
            attackTimer.SetTime(meleeCastedData.cooldown);
            Vector2 initialPosition = transform.position;
            Vector2 targetPosition = initialPosition + ((Vector2)attackTarget.GetObject().transform.position - initialPosition).normalized * 0.25f;
            transform.DOJump(targetPosition, 0.25f, 1, 0.15f).SetEase(Ease.InFlash).SetLink(gameObject).OnComplete(() => {
                if (attackTarget != null && attackTarget.GetObject() != null) attackTarget.Damage(meleeCastedData.damage);
                transform.DOJump(initialPosition, 0.25f, 1, 0.15f).SetEase(Ease.InFlash).SetLink(gameObject);
            });
            AudioManager.inst.PlayQuery(meleeCastedData.fightSound);
        }
    }

    private Vector2 GetTargetPosition() {
        if (attackTarget.GetObject().TryGetComponent<DestructableBuilding>(out var building)) {
            IsPositionDynamic = false;
            return building.GetClosestPosition().Value;
        } else {
            IsPositionDynamic = true;
            Vector2? pos = WorldGrid.inst.GetClosestEmptyToPosition(attackTarget.GetObject().transform.position);
            if (pos == null) {
                attackTarget = null;
                return transform.position;
            }
            return pos.Value;
        }
    }

    #region Internal

    protected override void Die() {
        if (hub != null) {
            /*bool i = */hub.unitsHere.Remove(this);
            /*if (!i){
                Debug.Log("Unit was not found in hub");
                hub.unitsHere.Print();
            }*/
            
        }
        base.Die();
    }

    private void SetPosition() {
        if (IsPositionDynamic) {
            Vector2 tpos = GetTargetPosition();
            if (Vector2.Distance(previousTargetPosition, tpos) > meleeCastedData.attackDistance) {
                targetPosition = tpos;
            }
            
        } else {
            if (!isTPSet) {
                targetPosition = GetTargetPosition();
                isTPSet = true;
            }
        }
        if (targetPosition != previousTargetPosition) {
            isPathFinished = true;
        }
    }

    public override void Attack(IDamagable target) {
        if (target == attackTarget) return;
        isTPSet = false;
        attackTarget = target;
        state = State.Idle;
    }

    protected override void Update() {
        base.Update();
        if (GameStateManager.inst.gameState == GameStateManager.State.Over) return;
        HandleStateMachine();
    }

    protected override void Awake() {
        base.Awake();
        meleeCastedData = (MeleeWarriorData)data;
    }

    public UnitData GetData() {
        return data;
    }

    public void ConnectToHub(HubBuilding _hub) {
        hub = _hub;
    }

    public void DisconnectFromHub() {
        hub = null;
    }

    private enum State {
        Idle,
        Approaching,
        Attacking
    }

    #endregion
}