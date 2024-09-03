using System.Collections.Generic;
using UnityEngine;
using SFH;
public class Researcher : TimedUnit {
    [SerializeField] private AudioQuerySO workSound, eatingSound;
    private ResearcherData researcherData;
    [SerializeField] private List<Resource> listForScience;
    private Timer workTimer = new Timer();
    private Timer restTimer = new Timer();

    private bool workNotRest = true;
    private bool T1 = true;
    private bool T2 = true;
    private int countAlreadyWorked = 0;

    protected override void Update() {
        base.Update();
        if (workNotRest) {
            if (T1) {
                workTimer ??= new Timer();
                workTimer.SetTime((researcherData.lifeTime - researcherData.restDuration) / researcherData.workPerWorkCycle);
                T1 = false;
            }
            if (workTimer.Execute()) {
                TryCreateOneScience();
                countAlreadyWorked++;
                if (countAlreadyWorked == researcherData.workPerWorkCycle) {
                    workNotRest = false;
                }
                T1 = true;
            }
        } else {
            if (T2) {
                restTimer ??= new Timer();
                restTimer.SetTime(researcherData.restDuration);
                T2 = false;
            }
            if (restTimer.Execute()) {
                //Rest
                T2 = true;
                if (ManaManager.inst.TryConsumeMana(researcherData.restManaConsumption)) {
                    AudioManager.inst.PlayQuery(eatingSound);
                } else {
                    Die();
                }
                countAlreadyWorked = 0;
                workNotRest = true;
            }
        }
    }
    private void Start() {
        researcherData = (ResearcherData)data;
    }

    private void TryCreateOneScience() {
        //если есть дерево + камень, то если текущая наука есть - плюс один
        if (ResourceManager.inst.IsEnoughResources(listForScience) && ResearchCardsManager.inst.TryScience()) {
            ResourceManager.inst.ConsumeResources(listForScience);
            AudioManager.inst.PlayQuery(workSound);
            ResearchCardsManager.inst.AddScienceInCurrentCard();
        }
    }
}