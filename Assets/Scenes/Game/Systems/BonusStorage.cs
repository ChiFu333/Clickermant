using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFH;
using System;

public class BonusStorage : MonoSingleton<BonusStorage>
{
    public Dictionary<string, int> addBonuses = new Dictionary<string, int>()
    {
        {"ManaClick", 0},
        {"AttackUp", 0},
        {"SpeedUp", 0}
    };
    public Dictionary<string, float> multiBonuses = new Dictionary<string, float>()
    {
        {"Mining", 1}
    };
}
