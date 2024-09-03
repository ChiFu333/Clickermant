using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SFH;

public class BuildingCard : MonoBehaviour {
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text[] costTexts = new TMP_Text[3];
    [SerializeField] private Image[] costIcons = new Image[3];
    [SerializeField] private Image icon;
    [SerializeField] private AudioQuerySO success, unsuccess;
    private BuildingData buildingData;
    public void Setup(BuildingData _buildingData) {
        buildingData = _buildingData;
        nameText.SetText(buildingData.name);
        icon.sprite = buildingData.icon;
        for (int i = 0; i < 3; i++) {
            if (i < buildingData.cost.Count) {
                costTexts[i].text = buildingData.cost[i].count.ToString();
                costIcons[i].sprite = buildingData.cost[i].data.sprite;
            } else {
                costIcons[i].gameObject.SetActive(false);
            }
        }
    }

    public void Build() {
        //Check if enough resources
        //If so talk to building manager and set building type and enter build mode
        if (ResourceManager.inst.IsEnoughResources(buildingData.cost)) {
            BuildingManager.inst.SelectBuilding(buildingData);
            BuildingManager.inst.inBuildMode = true;
            BuildingManager.inst.inDeconstructionMode = false;
            AudioManager.inst.PlayQuery(success);
        } else {
            AudioManager.inst.PlayQuery(unsuccess);
        }
    }
}
