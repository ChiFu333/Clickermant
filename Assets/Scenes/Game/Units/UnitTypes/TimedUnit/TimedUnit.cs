using SFH;
using UnityEngine;

public class TimedUnit : PathedUnit, IHubUnit {
    [SerializeField] private AudioQuerySO dieSound;
    private TimedUnitData timedCastedData;
    private readonly Timer timer = new Timer();
    protected HubBuilding hub;

    public void ConnectToHub(HubBuilding _hub) {
        hub = _hub;
    }

    public void DisconnectFromHub() {
        hub = null;
    }

    protected void ResetTimer() {
        timer.SetTime(timedCastedData.lifeTime);
    }

    private void HandleTimer() {
        if (timer.Execute()) {
            if (hub != null) {
                SetMoveToHubState();
                return;
            }
            Die();
        }
    }

    protected virtual void SetMoveToHubState() { }

    protected override void Die() {
        //Remove self from hub
        if (hub != null) hub.unitsHere.Remove(this);
        AudioManager.inst.PlayQuery(dieSound);
        base.Die();
    }

    protected override void Awake() {
        base.Awake();
        timedCastedData = (TimedUnitData)data;
        timer.SetTime(timedCastedData.lifeTime);
    }

    protected override void Update() {
        base.Update();
        HandleTimer();
    }

    public UnitData GetData() {
        return data;
    }
}