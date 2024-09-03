using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuManager : MonoBehaviour {
    [SerializeField] private string targetSceneName;

    public void ResetTutorial() {
        PlayerPrefs.SetInt("tutorialComplete", 0);
    }

    public void Play() {
        SceneManager.LoadScene(targetSceneName);
    }

    public void Exit() {
        Application.Quit(0);
    }

    #region Internal

#if UNITY_EDITOR
    [MenuItem("GameObject/SFH/Menu Manager")]
    private static void CreateMenuManager() {
        GameObject pfx = new GameObject("Menu Manager");
        pfx.AddComponent<MenuManager>();
    }
#endif

    #endregion
}
