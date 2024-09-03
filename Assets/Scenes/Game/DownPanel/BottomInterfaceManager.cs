using SFH;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomInterfaceManager : MonoSingleton<BottomInterfaceManager> {
    [Header("Mode")]
    [SerializeField] private Image modeImage;
    [SerializeField] private Sprite unitSprite;
    [SerializeField] private Sprite buildSprite;
    [Header("Cards")]
    [SerializeField] private int maxCardsInHolder = 3;
    [SerializeField] private UnitCard unitCardPrefab;
    [SerializeField] private BuildingCard buildCardPrefab;
    [SerializeField] private Transform leftCardHolder;
    [SerializeField] private Transform rightCardHolder;
    [Header("Available from start")]
    [SerializeField] private List<UnitData> alreadyAvailableUnits = new List<UnitData>();

    private readonly List<UnitData> availableUnits = new List<UnitData>();
    private readonly List<BuildingData> availableBuildings = new List<BuildingData>();

    private InterfaceMode mode = InterfaceMode.Unit;
    public void ChangeMode() {
        if (mode == InterfaceMode.Unit) {
            mode = InterfaceMode.Build;
            UpdateUIToBuild();
        } else {
            mode = InterfaceMode.Unit;
            UpdateUIToUnit();
        }
    }

    #region Internal

    private void UpdateUIToUnit() {
        UpdateUIBase(unitSprite);
        //Show Unit cards
        for (int i = 0; i < availableUnits.Count; i++) {
            //Create unit card
            UnitCard card = Instantiate(unitCardPrefab.gameObject).GetComponent<UnitCard>();
            //TODO: setup cards information and acllbacks
            card.Setup(availableUnits[i]);
            if (i < maxCardsInHolder) {
                //To left
                card.transform.SetParent(leftCardHolder, false);
            } else {
                //To right
                card.transform.SetParent(rightCardHolder, false);
            }
        }
    }

    private void UpdateUIToBuild() {
        UpdateUIBase(buildSprite);
        //Show build cards
        for (int i = 0; i < availableBuildings.Count; i++) {
            //Create build card
            BuildingCard card = Instantiate(buildCardPrefab.gameObject).GetComponent<BuildingCard>();
            //TODO: setup cards information and acllbacks
            card.Setup(availableBuildings[i]);
            //TODO: setup cards information and acllbacks
            if (i < maxCardsInHolder) {
                //To left
                card.transform.SetParent(leftCardHolder, false);
            } else {
                //To right
                card.transform.SetParent(rightCardHolder, false);
            }
        }
    }

    private void UpdateUIBase(Sprite targetSprite) {
        modeImage.sprite = targetSprite;
        //Clear holders
        for (int i = 0; i < leftCardHolder.childCount; i++) {
            Destroy(leftCardHolder.GetChild(i).gameObject);
        }
        for (int i = 0; i < rightCardHolder.childCount; i++) {
            Destroy(rightCardHolder.GetChild(i).gameObject);
        }
    }

    private enum InterfaceMode {
        Unit,
        Build
    }

    private void Start() {
        availableUnits.AddRange(alreadyAvailableUnits);
        availableBuildings.AddRange(BuildingManager.inst.buildingAvailableFromStart);
        UpdateUIToUnit();
    }

    #endregion
    public void AddNewUnit(UnitData d)
    {
        availableUnits.Add(d);
        if (mode == InterfaceMode.Unit) {
            UpdateUIToUnit();
        } else {
            UpdateUIToBuild();
        }
    }
    public void AddNewBuilding(BuildingData d)
    {
        availableBuildings.Add(d);
        if (mode == InterfaceMode.Unit) {
            UpdateUIToUnit();
        } else {
            UpdateUIToBuild();
        }
    }
}
