using UnityEngine;
using SFH;
public class PanelChanger : SerializableMonoSingleton<PanelChanger> {
    [SerializeField] private GameObject[] panels;
    [SerializeField] private GameObject buildPanel;
    public void ChangeActive(GameObject ui) {
        ui.SetActive(!ui.activeSelf);
    }
    public void EnterBuildMode()
    {
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
            buildPanel.SetActive(true);
        }
    }
    public void ExitEnterMode()
    {
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(true);
            buildPanel.SetActive(false);
        }
    }
}
