using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameLoad : MonoBehaviour
{
    private string sceneName = "InGame";
    public void OnInGameLoad()
    {
        SceneManager.LoadScene(sceneName);
    }
}
