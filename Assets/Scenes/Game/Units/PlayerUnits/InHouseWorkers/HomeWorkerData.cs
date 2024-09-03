using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Home Worker Data", menuName = "Game/Home Worker Data")]
public class HomeWorkerData : TimedUnitData
{
    [field: SerializeField] public int manaMaking;
    [field: SerializeField] public int workPerWorkCycle;
}
