using UnityEngine;
using SoundSystem;  // ★必須：サウンドマネージャーの名前空間

public class TitleBGMController : MonoBehaviour
{
    [Header("BGM Names (BGMDatabase に登録されている名前)")]
    [SerializeField] private string titleBgmName = "TitleBGM";
    [SerializeField] private string gameBgmName = "GameBGM";

    [Header("Fade Settings")]
    [SerializeField] private float startFadeInTime = 0.5f;  // タイトルBGMのフェードイン
    [SerializeField] private float crossFadeTime = 2.0f;  // タイトル→ゲームBGMのクロスフェード時間

    // タイトルシーンが開いたときにBGMを鳴らす
    private void Start()
    {
        // タイトルBGM再生（必要ならフェード時間を0にして即鳴らしでもOK）
        SoundManager.Instance.PlayBGMByName(titleBgmName, startFadeInTime);
    }

    // 「ゲームスタート」ボタンから呼んでもらう
    public void OnClickStartGame()
    {
        // タイトルBGMをフェードアウトしつつ、GameBGMをフェードイン
        SoundManager.Instance.PlayBGMByName(gameBgmName, crossFadeTime);
    }
}
