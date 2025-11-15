using UnityEngine;
using UnityEngine.UI;
using SoundSystem;

public class SoundTestTitle : MonoBehaviour
{
    [SerializeField] private string titleBGMName = "TitleBGM";
    [SerializeField] private string startSEName = "StartSE";
    [SerializeField] private string mainGameBGMName = "MainGameBGM";

    [SerializeField] private Button startButton;

    private void Start()
    {
        var sm = SoundManager.GetInstance();

        if (sm != null)
        {
            sm.PlayBGMByName(titleBGMName);
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogWarning("StartButton が設定されていません");
        }
    }

    private void OnStartButtonClicked()
    {
        var sm = SoundManager.GetInstance();
        if (sm != null)
        {
            sm.PlaySeByName(startSEName);
            sm.PlayBGMByName(mainGameBGMName);
        }

        // シーン遷移処理は他のスクリプトに任せる
    }
}
