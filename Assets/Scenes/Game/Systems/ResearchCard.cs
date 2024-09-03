using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ResearchCard : MonoBehaviour {
    [SerializeField] public ResearchSO researchData { get; private set; }
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text researchName, progressBar;
    [SerializeField] private GameObject toggleBox;
    [SerializeField] private Slider progressSlider;
    private int scienceInMe;
    public bool toggled { get; private set; }

    private void Start() {
        if (researchData != null) Init(researchData);
    }
    public void Init(ResearchSO data) {
        researchData = data;
        icon.sprite = data.icon;
        researchName.text = data.bonusName;
        progressSlider.maxValue = data.researchCost;
        progressBar.text = "0/" + data.researchCost.ToString();
    }
    public void ChangeToggleActive(bool active) {
        toggleBox.SetActive(active);
        toggled = active;
    }

    public float GetProgressPercent() {
        return scienceInMe / (float)researchData.researchCost;
    }
    public void SelectThisCard() {
        ResearchCardsManager.inst.ChangeActiveClickedCard(this);
    }
    public void AddScience(int count) {
        if (scienceInMe + count > researchData.researchCost) {
            scienceInMe = researchData.researchCost;
        } else {
            scienceInMe += count;
        }
        progressSlider.value = scienceInMe;
        progressBar.text = scienceInMe + "/" + researchData.researchCost.ToString();
        if (scienceInMe == researchData.researchCost) {
            ResearchCardsManager.inst.CompleteResearch(this);
        }
    }
}
