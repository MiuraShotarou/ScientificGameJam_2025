using UnityEngine;
using SoundSystem;

public class PlayBGMOnStart:
MonoBehaviour
{
    [SerializeField]
    private string bgmName = "GameBGM";

    void Start()
    {
        SoundManager.Instance.PlayBGMByName(bgmName);
    }
}
