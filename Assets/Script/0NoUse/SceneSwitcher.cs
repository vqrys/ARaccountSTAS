using UnityEngine;
using UnityEngine.SceneManagement;

public class SafeSceneSwitch : MonoBehaviour
{
    public string sceneName;

    public void LoadScene()
    {
        StartCoroutine(SwitchScene());
    }

    System.Collections.IEnumerator SwitchScene()
    {
        // Wait one frame so Vuforia can release camera
        yield return null;
        SceneManager.LoadScene(sceneName);
    }
}
