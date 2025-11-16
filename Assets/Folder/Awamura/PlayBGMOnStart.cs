using UnityEngine;
using SoundSystem;   // ← これが必要だったはず

public class PlayBGMOnStart : MonoBehaviour
{
    [SerializeField] private string bgmName = "GameBGM";

    void Start()
    {
        SoundManager.Instance.PlayBGMByName(bgmName);
    }
}
