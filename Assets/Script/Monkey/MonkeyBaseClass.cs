using UnityEngine;

public class MonkeyBaseClass : MonoBehaviour
{
    public bool isFollowing = false;  // 仲間状態
    public int order = 1;             // 何番目に並ぶか
    public Transform player;          // プレイヤー
    public float distance = 1f;       // プレイヤーからの距離
    public FollowPlayerTest follow;   // 追従スクリプト

    protected virtual void Start()
    {
    }

    public virtual void JoinPlayer(Transform playerTransform)
    {
        isFollowing = true;
        player = playerTransform;

        // Layer を変更（プレイヤーとの当たり判定を消す）
        gameObject.layer = LayerMask.NameToLayer("PlayerFollowed");

        // Follow スクリプトを追加
        follow = gameObject.AddComponent<FollowPlayerTest>();
        follow.target = player;
    }

    public virtual void UpdateFollowOffset(int facing)
    {
        if (follow != null)
        {
            follow.offset = new Vector3(distance * order * facing, 0f, 0f);
        }
    }
}
