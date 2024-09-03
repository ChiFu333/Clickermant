using SFH;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyTriggerSpawner : SerializableMonoSingleton<EnemyTriggerSpawner> {
    [SerializeField] private TMP_Text waveTimerText;
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private  Dictionary<WarriorUnitData, int> maxUnitsOnScreen = new Dictionary<WarriorUnitData, int>();
    public Dictionary<WarriorUnitData, int> unitsOnScreen = new Dictionary<WarriorUnitData, int>();
    private readonly Timer waveTimer = new Timer();
    private readonly List<Vector2Int> borderPositions = new List<Vector2Int>();
    private int waveIndex = 0;
    private bool waitTillNoMoreEnemies = false;

    /*

    /*[Header("Ents")]
    [SerializeField] private WarriorUnitData entData;
    [SerializeField] private int entXMaxFromRight;
    [Header("Villagers")]
    [SerializeField] private WarriorUnitData villagerData;
    [SerializeField] private int villagerXMaxFromLeft;
    [SerializeField] private float villagerSpawnCooldown;
    [SerializeField] private int villagersBaseAmount;
    private readonly Timer villagerTimer = new Timer();
    /*public void SpawnEnt() {
        //Find random empty spot on the right part
        for (int i = 0; i < 10; i++) {
            int x = Random.Range(WorldGrid.inst.worldGridWidth - entXMaxFromRight, WorldGrid.inst.worldGridWidth);
            int y = Random.Range(0, WorldGrid.inst.worldGridHeight);
            if (WorldGrid.inst.worldGrid.GetValueAt(x, y).IsFree()) {
                //Spawn ent here
                UnitBehaviour unit = UnitManager.inst.CreateUnit(entData);
                unit.transform.position = WorldGrid.inst.worldGrid.GetWorldPosition(x,y);
                break;
            }
        }
        
    }

    private void SpawnVillagers() {
        //Find random empty spot on the left part
        //Spawn villagers here
        for (int k = 0; k < villagersBaseAmount; k++) {
            for (int i = 0; i < 10; i++) {
                int x = Random.Range(0, villagerXMaxFromLeft);
                int y = Random.Range(0, WorldGrid.inst.worldGridHeight);

                if (WorldGrid.inst.worldGrid.GetValueAt(x, y).IsFree()) {
                    //Spawn ent here
                    UnitBehaviour unit = UnitManager.inst.CreateUnit(villagerData);
                    unit.transform.position = WorldGrid.inst.worldGrid.GetWorldPosition(x, y);
                    break;
                }
            }
        }
    }

    #region Internal

    private void Update() {
        /*if (villagerTimer.Execute()) {
            villagerTimer.SetTime(villagerSpawnCooldown);
            SpawnVillagers();
        }
    }
    private void Start() {
        //villagerTimer.SetTime(villagerSpawnCooldown);
    }

    #endregion*/

    public void RemoveUnit(UnitData unitData) {
        if (unitData is WarriorUnitData warrior) {
            unitsOnScreen[warrior]--;
            unitsOnScreen[warrior] = Mathf.Max(0, unitsOnScreen[warrior]);
        }
    }

    private void CreateWave() {
        Wave wave = waves[waveIndex];
        for (int k = 0; k < wave.unitsCount.Count; k++) {
            UnitCount unit = wave.unitsCount[k];

            if (!unitsOnScreen.ContainsKey(unit.unit)) {
                unitsOnScreen.Add(unit.unit, 0);
            }
            int delta = Mathf.Min(unit.count, maxUnitsOnScreen[unit.unit]) - unitsOnScreen[unit.unit];
            for (int i = 0; i < delta; i++) {
                //Try spawn unit
                UnitBehaviour behaviour = UnitManager.inst.CreateUnit(unit.unit);
                Vector2? position = PickPosition();
                if (!position.HasValue) continue;
                behaviour.transform.position = position.Value;
                unitsOnScreen[unit.unit]++;
            }
        }
    }

    private Vector2? PickPosition() {
        for (int i = 0; i < 10; i++) {
            Vector2Int randomPos = borderPositions.RandomItem();
            if (WorldGrid.inst.worldGrid.GetValueAt(randomPos).IsFree()) return WorldGrid.inst.worldGrid.GetWorldPosition(randomPos);
        }
        return null;
    }

    private void Update() {
        if (GameStateManager.inst.gameState == GameStateManager.State.Over) return;
        if (waitTillNoMoreEnemies) {
            waveTimerText.SetText("0_0");
            int units = 0;
            foreach (KeyValuePair<WarriorUnitData, int> kvp in unitsOnScreen) {
                units += kvp.Value;
            }
            if (units == 0) {
                GameStateManager.inst.Win();
            }
        } else {
            waveTimerText.SetText($"{waveTimer.GetTimeLeft().ToString("0")}");
            if (waveTimer.Execute()) {
                CreateWave();
                waveIndex++;
                if (waveIndex >= waves.Count) {
                    waitTillNoMoreEnemies = true;
                    return;
                }
                waveTimer.SetTime(waves[waveIndex].delayAfterPreviousWave);
            }
        }
    }

    private void Start() {
        //Setup wave 0
        waveTimer.SetTime(waves[0].delayAfterPreviousWave);
        for (int x = 0; x < WorldGrid.inst.worldGridWidth; x++) {
            borderPositions.Add(new Vector2Int(x, 1));
            borderPositions.Add(new Vector2Int(x, WorldGrid.inst.worldGridHeight - 1));
        }
        for (int y = 0; y < WorldGrid.inst.worldGridHeight; y++) {
            borderPositions.Add(new Vector2Int(1, y));
            borderPositions.Add(new Vector2Int(WorldGrid.inst.worldGridWidth - 1, y));
        }
    }


    [System.Serializable]
    private class Wave {
        public float delayAfterPreviousWave;
        public List<UnitCount> unitsCount = new List<UnitCount>();
    }

    [System.Serializable]
    private class UnitCount {
        public WarriorUnitData unit;
        public int count;
    }
}