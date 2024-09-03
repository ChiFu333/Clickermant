using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    [SerializeField] private RectTransform tutorialBox;
    [SerializeField] private TMP_Text tutorialText;

    private void ShowTutorial() {
        tutorialBox.sizeDelta = Vector2.zero;
        tutorialBox.gameObject.SetActive(true);
        tutorialText.SetText("");
        tutorialBox.DOSizeDelta(new Vector2(1100,100),0.5f).SetDelay(3).OnComplete(()=> {
            tutorialText.SetText("Click this funny blue button to earn mana");
            tutorialBox.DOSizeDelta(new Vector2(1300, 100), 0.5f).SetDelay(3).OnComplete(()=>{
                tutorialText.SetText("Summon units and buildings by clicking cards below");
                tutorialBox.DOSizeDelta(new Vector2(1350, 100), 0.5f).SetDelay(3).OnComplete(() => {
                    tutorialText.SetText("Some units consume mana, some need a place for that");
                    tutorialBox.DOSizeDelta(new Vector2(1425, 100), 0.5f).SetDelay(3).OnComplete(() => {
                        tutorialText.SetText("Research new technologies. Create army. Survive 7 waves.");
                        tutorialBox.DOScale(0,0.5f).SetDelay(3.5f);
                    });
                });
            });
        });
    }

    private void Start() {
        
        bool isTutorialComplete = PlayerPrefs.GetInt("tutorialComplete", 0) == 1;
        if (!isTutorialComplete) {
            PlayerPrefs.SetInt("tutorialComplete", 1);
            ShowTutorial();
        } else {
            tutorialBox.gameObject.SetActive(false);
        }
    }
}
