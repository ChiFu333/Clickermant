using SFH;
using UnityEngine;
using DG.Tweening;

public class WorldMinerUnit : TimedUnit {
    [SerializeField] private AudioQuerySO miningSound, eatingSound;
    private WorldMinerData minerCastedData;
    private State state = State.Idle;
    private ResourceBuilding targetResourceBuilding;
    private readonly Timer mineTimer = new Timer();
    private readonly Timer restTimer = new Timer();
    private bool isHubPosSet = false, isResourcePosSet = false;
    private Vector2? restPosition, closestPosition;

    //Implement this state machine:
    //If "Working": Based on resource manager, locate random resource in world, set path to it, mine it, repeat until timer ends. -> "Resting"
    //If "Resting": If assigned, go to assigned building, wait till enough free mana, consume mana, wail some time. -> "Working"
    //If "Resting": If not assigned desintegrate

    private void HandleStateMachine() {
        switch (state) {
            case State.Idle:
                isPathFinished = true;
                state = State.Working;
                break;
            case State.Working:
                //Get suitable resource
                if (targetResourceBuilding == null) {
                    targetResourceBuilding = ResourceManager.inst.GetResourceBuilding(minerCastedData.targetResourceTypes.RandomItem());
                    if (targetResourceBuilding != null) { isPathFinished = true; isResourcePosSet = false; }
                }
                if (targetResourceBuilding == null) {
                    state = State.Idle;
                    break; 
                }
                if (!isResourcePosSet) {
                    closestPosition = targetResourceBuilding.GetClosestPosition();
                    isResourcePosSet = true;
                }
                
                if (closestPosition == null) {
                    ResetState();
                    break;
                }
                float distanceToTarget = Vector2.Distance(transform.position, closestPosition.Value);
                //Walk towards it
                if (isPathFinished && distanceToTarget > minerCastedData.mineDistance) {
                    MoveTo(closestPosition.Value);
                    if (path == null) {
                        //Target is unreachable, pick another one
                        ResetState();
                        break;
                    }
                    mineTimer.SetTime(minerCastedData.mineCooldown);
                }
                //Mine
                if (distanceToTarget < minerCastedData.mineDistance) {
                    if (mineTimer.Execute()) {
                        Mine();
                    }
                }
                break;
            case State.Resting:
                //Goto rest position and rest for some time
                if (!isHubPosSet) {
                    restPosition = hub.GetClosestPosition();
                    isHubPosSet = true;
                }
                if (restPosition == null) {
                    Die();
                    break;
                }
                float distanceToHub = Vector2.Distance(transform.position, restPosition.Value);
                if (isPathFinished && distanceToHub > minerCastedData.mineDistance) {
                    MoveTo(restPosition.Value);
                    restTimer.SetTime(minerCastedData.restDuration);
                    if (path == null) {
                        Die();
                        break;
                    }
                    
                }
                if (distanceToHub < minerCastedData.mineDistance * 2) {
                    if (restTimer.Execute()) {
                        //Rest
                        AudioManager.inst.PlayQuery(eatingSound);
                        if (ManaManager.inst.TryConsumeMana(minerCastedData.restManaConsumption)) {
                            ResetTimer();
                            ResetState();
                            state = State.Idle;
                        } else {
                            Die();
                        }
                    }
                }
                break;
        }
    }

    private void Mine() {
        mineTimer.SetTime(minerCastedData.mineCooldown / BonusStorage.inst.multiBonuses["Mining"]);
        Vector2 initialPosition = transform.position;
        Vector2 targetPosition = initialPosition + ((Vector2)targetResourceBuilding.transform.position - initialPosition).normalized * 0.25f;
        transform.DOJump(targetPosition, 0.25f, 1, 0.15f).SetEase(Ease.InFlash).SetLink(gameObject).OnComplete(()=> {
            targetResourceBuilding.Mine(minerCastedData.minePower);
            AudioManager.inst.PlayQuery(miningSound);
            transform.DOJump(initialPosition, 0.25f, 1, 0.15f).SetEase(Ease.InFlash).SetLink(gameObject);
        });
    }

    protected override void SetMoveToHubState() {
        state = State.Resting;
    }

    private void ResetState() {
        if (targetResourceBuilding != null) ResourceManager.inst.ReturnResourceBuilding(targetResourceBuilding);
        targetResourceBuilding = null;
        isPathFinished = false;
        isResourcePosSet = false;
    }

    protected override void Die() {
        base.Die();
        if (targetResourceBuilding != null) ResourceManager.inst.ReturnResourceBuilding(targetResourceBuilding);
    }

    #region Internal

    protected override void Update() {
        base.Update();
        HandleStateMachine();
    }

    protected override void Awake() {
        base.Awake();
        minerCastedData = (WorldMinerData)data;
    }

    private enum State {
        Idle,
        Working,
        Resting
    }

    #endregion
}