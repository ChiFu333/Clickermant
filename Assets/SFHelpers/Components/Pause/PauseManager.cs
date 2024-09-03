using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PauseManager : MonoBehaviour {
    [SerializeField] private float defaultTimeScale = 1;
    [SerializeField] private GameObject pauseMenuHolder;
    private bool isPaused = false;
    public bool IsPaused() => isPaused;

    /// <summary>
    /// Opens pause menu and pauses game
    /// </summary>
    public void PauseMenu() {
        Pause();
        if (pauseMenuHolder != null) {
            pauseMenuHolder.SetActive(true);
        }
    }
    /// <summary>
    /// Pauses game by setting Time.timeScale to 0
    /// </summary>
    public void Pause() {
        isPaused = true;
        Time.timeScale = 0;
    }
    /// <summary>
    /// Unpauses game by setting Time.timeScale to default value
    /// </summary>
    public void Unpause() {
        isPaused = false;
        Time.timeScale = defaultTimeScale;
        if (pauseMenuHolder != null) {
            pauseMenuHolder.SetActive(false);
        }
    }

    #region Internal

#if UNITY_EDITOR
    [MenuItem("GameObject/SFH/Pause Manager")]
    private static void CreatePauseManager() {
        GameObject pfx = new GameObject("Pause Manager");
        pfx.AddComponent<PauseManager>();
    }
#endif

    #endregion
}
