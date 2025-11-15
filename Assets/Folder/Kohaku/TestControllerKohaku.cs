using UnityEngine;

public class TestControllerKohaku : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;    // 横移動の速さ
    public float jumpForce = 5f;    // ジャンプの強さ

    [Header("接地判定")]
    public Transform groundCheck;           // 足元のチェック位置
    public float groundCheckRadius = 0.1f;  // 接地判定の円の半径
    public LayerMask groundLayer;           // 地面のレイヤー

    private Rigidbody2D rb;
    private Animator anim;

    private float inputX;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // ★ Animatorを取得
    }

    private void Update()
    {
        // キーボードの左右入力（A,D / ←,→）
        inputX = Input.GetAxisRaw("Horizontal");

        // スペースキーでジャンプ（接地しているときだけ）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // アニメーション更新（移動入力に応じて）
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        // 横移動（速度を直接書き換え）
        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

        // 地面に足が付いているか判定
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        // 一旦縦の速度をリセットしてから上向きに力を加える
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void UpdateAnimation()
    {
        if (anim == null) return;

        // 横移動しているかどうか（移動アニメ）
        bool isMoving = Mathf.Abs(inputX) > 0.01f;
        anim.SetBool("Move", isMoving);

        // ジャンプ中かどうか（ジャンプアニメ）
        bool isJumping = !isGrounded;
        anim.SetBool("Jamp", isJumping);

        // ★★★ 左右反転処理 ★★★
        if (inputX > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (inputX < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

}
