using UnityEngine;
using System.Collections.Generic;

public class PlayerScriptTestkohaku : MonoBehaviour
{
    [Header("InGameManager.cs")]
    [SerializeField] InGameManager inGameManager;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float distance;

    // ★ Animator の追加
    [SerializeField] private Animator animator;

    private bool canJump = false;
    private bool isJump = true;
    private bool canRun = false;
    private bool canClimb = false;
    private bool isClimbing = false;
    private bool canSwing = false;
    private bool isSwinging = false;

    private int facing = 1;

    private Rigidbody2D rb;

    private List<FollowPlayerTest> monkeys = new List<FollowPlayerTest>();
    private List<FollowPlayerTest> Monkeys {get{return monkeys;}
        set { monkeys = value; inGameManager.ReduceVirus();} 
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Reset();
    }

    void Update()
    {
        // ==========================
        //  Climbing 中
        // ==========================
        if (isClimbing)
        {
            float climb_H = Input.GetAxisRaw("Horizontal");
            float climb_V = Input.GetAxisRaw("Vertical");

            rb.linearVelocity = new Vector2(climb_H, climb_V * speed);

            // ★ Climb 中の Move 判定
            bool isMove = Mathf.Abs(climb_H) > 0.01f || Mathf.Abs(climb_V) > 0.01f;
            animator.SetBool("Move", isMove);


            // ★ 左右反転
            if (climb_H > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (climb_H < 0)
                transform.localScale = new Vector3(-1, 1, 1);

            UpdateMonkeyOffsets();

            return;
        }

        // ==========================
        //  Swinging 中
        // ==========================
        if (isSwinging)
        {
            float swing_H = Input.GetAxisRaw("Horizontal");
            float swing_V = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(swing_H * speed, swing_V);


            bool isMove = Mathf.Abs(swing_H) > 0.01f || Mathf.Abs(swing_V) > 0.01f;
            animator.SetBool("Move", isMove);


            // ★ 左右反転
            if (swing_H > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (swing_H < 0)
                transform.localScale = new Vector3(-1, 1, 1);

            UpdateMonkeyOffsets();

            return;
        }

        // ==========================
        //  通常移動
        // ==========================
        float horizontal = Input.GetAxisRaw("Horizontal");

        // ★ 左右反転処理
        if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (horizontal != 0)
        {
            facing = horizontal > 0 ? 1 : -1;
        }

        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        // ★ Move アニメ
        if (animator != null)
        {
            animator.SetBool("Move", Mathf.Abs(horizontal) > 0.01f);
        }

        // ジャンプ
        if (canJump && !isJump)
        {
            JupmMove();
        }
        if (canRun)
        {
            RunMove();
        }

        UpdateMonkeyOffsets();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJump = false;

            // ★ 着地 → Jamp false
            if (animator != null)
            {
                animator.SetBool("Jamp", false);
            }
        }

        if (collision.gameObject.CompareTag("Monkey_1"))
        {
            canJump = true;

            collision.collider.gameObject.layer = LayerMask.NameToLayer("PlayerFollowed");

            Rigidbody2D mrb = collision.collider.GetComponent<Rigidbody2D>();
            if (mrb != null) mrb.isKinematic = true;

            FollowPlayerTest follow = collision.collider.gameObject.AddComponent<FollowPlayerTest>();
            follow.target = this.transform;

            Monkeys.Add(follow);
        }

        if (collision.gameObject.CompareTag("Monkey_2"))
        {
            canRun = true;

            collision.collider.gameObject.layer = LayerMask.NameToLayer("PlayerFollowed");

            Rigidbody2D mrb = collision.collider.GetComponent<Rigidbody2D>();
            if (mrb != null) mrb.isKinematic = true;

            FollowPlayerTest follow = collision.collider.gameObject.AddComponent<FollowPlayerTest>();
            follow.target = this.transform;

            Monkeys.Add(follow);
        }

        if (collision.gameObject.CompareTag("Monkey_3"))
        {
            canClimb = true;
            canSwing = true;

            collision.collider.gameObject.layer = LayerMask.NameToLayer("PlayerFollowed");

            Rigidbody2D mrb = collision.collider.GetComponent<Rigidbody2D>();
            if (mrb != null) mrb.isKinematic = true;

            FollowPlayerTest follow = collision.collider.gameObject.AddComponent<FollowPlayerTest>();
            follow.target = this.transform;

            Monkeys.Add(follow);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tree") && canClimb)
        {
            Climbing();
        }
        if (collision.CompareTag("Swing") && canSwing)
        {
            Swinging();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tree"))
        {
            isClimbing = false;
            rb.gravityScale = 1f;

            animator.SetBool("Climb", false);
            animator.SetBool("Move", false);

        }

        if (collision.CompareTag("Swing"))
        {
            isSwinging = false;
            rb.gravityScale = 1f;

            if (animator != null)
            {
                animator.SetBool("Move", false);
            }
        }
    }

    void JupmMove()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJump = true;

            // ★ Jamp = true

            animator.SetBool("Jamp", true);

        }
    }

    void RunMove()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 8f;
        }
    }

    void Climbing()
    {
        isClimbing = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        // ★ 登り中フラグ

        animator.SetBool("Climb", true);
        animator.SetBool("Jamp", false);
        animator.SetBool("Move", false);

    }

    void Swinging()
    {
        isSwinging = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("Jamp", false);
        animator.SetBool("Move", false);

    }

    void Reset()
    {
        speed = 5f;
        jumpForce = 5f;
        canJump = false;
        canRun = false;
        canClimb = false;
        isClimbing = false;
        canSwing = false;
        isSwinging = false;


        animator.SetBool("Jamp", false);
        animator.SetBool("Move", false);
        animator.SetBool("Climb", false);

    }

    void UpdateMonkeyOffsets()
    {
        for (int i = 0; i < Monkeys.Count; i++)
        {
            FollowPlayerTest mk = Monkeys[i];
            if (mk == null) continue;

            float index = i + 1.5f;

            if (isClimbing)
            {
                Debug.Log("木登り中");
                mk.offset = new Vector3(0f, distance * index, 0f);
            }
            else
            {
                float offsetX = facing == 1
                    ? distance * index
                    : -distance * index;

                mk.offset = new Vector3(offsetX, 0.2f, 0f);
            }
        }
    }
}
