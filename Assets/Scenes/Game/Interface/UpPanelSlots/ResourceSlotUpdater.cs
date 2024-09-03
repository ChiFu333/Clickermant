using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceSlotUpdater : MonoBehaviour {
    [SerializeField] private ResourceData resourceData;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text textCount;
    public void Start() {
        icon.sprite = resourceData.sprite;
        textCount.text = ResourceManager.inst.GetResource(resourceData).ToString();
        ResourceManager.inst.actionsLookup[resourceData].AddListener(UpdateUI);
        UpdateUI();
    }
    private void UpdateUI() {
        int m = ResourceManager.inst.GetResource(resourceData);
        textCount.text = m.ToString();
    }
}

