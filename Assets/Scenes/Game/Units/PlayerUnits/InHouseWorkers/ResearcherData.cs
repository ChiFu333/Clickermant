using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Researcher Data", menuName = "Game/Researcher Data")]
public class ResearcherData : TimedUnitData
{
    [field: SerializeField] public int workPerWorkCycle;
}
