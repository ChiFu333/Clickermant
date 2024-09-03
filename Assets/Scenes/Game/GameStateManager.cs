using System.Collections.Generic;
using SFH;
using TMPro;
using UnityEngine;

public class GameStateManager : MonoSingleton<GameStateManager> {
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreeen;
    [SerializeField] private GameObject closePopup;
    [SerializeField] private TMP_Text closeText;
    [SerializeField] private List<string> closePrompts = new List<string>();
    private int closeNum = 0;
    public State gameState { get; private set; } = State.Running;

    public void GameOver() {
        gameOverScreen.SetActive(true);
        gameState = State.Over;
    }

    public void Win() {
        winScreeen.SetActive(true);
        gameState = State.Over;
    }

    public void Menu() {
        SceneChanger.inst.Change("Menu");
    }

    public void TryClose() {
        if (closeNum < closePrompts.Count) {
            closeText.SetText(closePrompts[closeNum]);
            closeNum++;
        } else {
            Menu();
        }
    }

    public enum State {
        Running,
        Over
    }

    public void TogglePopup() {
        closeNum = 0;
        closeText.SetText("Do you really want to exit?");
        closePopup.SetActive(!closePopup.activeSelf);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePopup();
        }
    }
}
