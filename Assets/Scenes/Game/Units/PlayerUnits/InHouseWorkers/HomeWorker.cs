using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFH;

public class HomeWorker : TimedUnit {
    [SerializeField] private AudioQuerySO workSound, eatingSound;
    private HomeWorkerData homeWorkerData;

    private Timer workTimer = new Timer();
    private Timer restTimer = new Timer();

    private bool workNotRest = true;
    private bool T1 = true;
    private bool T2 = true;
    private int countAlreadyWorked = 0;
    private void Start() {
        homeWorkerData = (HomeWorkerData)data;
    }
    protected override void Update() {
        base.Update();
        if (workNotRest) {
            if (T1) {
                if (workTimer == null) workTimer = new Timer();
                workTimer.SetTime((homeWorkerData.lifeTime - homeWorkerData.restDuration) / homeWorkerData.workPerWorkCycle);
                T1 = false;
            }
            if (workTimer.Execute()) {
                ManaManager.inst.AddMana(homeWorkerData.manaMaking);
                countAlreadyWorked++;
                if (countAlreadyWorked == homeWorkerData.workPerWorkCycle) {
                    workNotRest = false;
                }
                T1 = true;
                AudioManager.inst.PlayQuery(workSound);
            }
        } else {
            if (T2) {
                if (restTimer == null) restTimer = new Timer();
                restTimer.SetTime(homeWorkerData.restDuration);
                T2 = false;
            }
            if (restTimer.Execute()) {
                //Rest
                T2 = true;
                if (ManaManager.inst.TryConsumeMana(homeWorkerData.restManaConsumption)) {
                    AudioManager.inst.PlayQuery(eatingSound);
                } else {
                    Die();
                }
                countAlreadyWorked = 0;
                workNotRest = true;
            }
        }
    }
}
