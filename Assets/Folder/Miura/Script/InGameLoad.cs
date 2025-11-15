using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameLoad : MonoBehaviour
{
    private string sceneName = "TestScene";
    public void OnInGameLoad()
    {
        SceneManager.LoadScene(sceneName);
    }
}
