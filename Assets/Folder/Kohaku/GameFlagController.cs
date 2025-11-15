using UnityEngine;

public class GameFlagController : MonoBehaviour
{
    // クリアしたかどうかのフラグ（他のスクリプトから読み取れるようにpublic）
    public bool isClear { get; private set; }

    // クリアフラグを立てるメソッド
    public void SetClearFlag()
    {
        if (isClear)
        {
            // すでにクリアしてたら何もしない
            return;
        }

        isClear = true;

        // ひとまずデバッグログで確認
        Debug.Log("Clear!");

        // ここに後で演出とかシーン遷移とか追加できる
        // 例:
        // SceneManager.LoadScene("ResultScene");
        // クリアUI表示 など
    }
}
