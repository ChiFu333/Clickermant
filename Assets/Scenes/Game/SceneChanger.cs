using SFH;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoSingleton<SceneChanger> {
    public void Change(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
