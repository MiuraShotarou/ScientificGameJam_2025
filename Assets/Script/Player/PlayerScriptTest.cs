using UnityEngine;
using System.Collections.Generic;

public class PlayerScriptTest : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float distance;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (isClimbing)
        {
            float climb_H = Input.GetAxisRaw("Horizontal");
            float climb_V = Input.GetAxisRaw("Vertical");

            rb.linearVelocity = new Vector2(climb_H, climb_V * speed);

            UpdateMonkeyOffsets();


            return;
        }
        if (isSwinging)
        {
            float swing_H = Input.GetAxisRaw("Horizontal");
            float swing_V = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(swing_H * speed, swing_V);

            UpdateMonkeyOffsets();
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal != 0)
        {
            facing = horizontal > 0 ? 1 : -1;
        }

        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

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
        }
        if (collision.gameObject.CompareTag("Monkey_1"))
        {
            canJump = true;

            collision.collider.gameObject.layer = LayerMask.NameToLayer("PlayerFollowed");

            Rigidbody2D mrb = collision.collider.GetComponent<Rigidbody2D>();
            if (mrb != null) mrb.isKinematic = true;

            FollowPlayerTest follow = collision.collider.gameObject.AddComponent<FollowPlayerTest>();
            follow.target = this.transform;

            monkeys.Add(follow);
        }

        if (collision.gameObject.CompareTag("Monkey_2"))
        {
            canRun = true;

            collision.collider.gameObject.layer = LayerMask.NameToLayer("PlayerFollowed");

            Rigidbody2D mrb = collision.collider.GetComponent<Rigidbody2D>();
            if (mrb != null) mrb.isKinematic = true;

            FollowPlayerTest follow = collision.collider.gameObject.AddComponent<FollowPlayerTest>();
            follow.target = this.transform;

            monkeys.Add(follow);
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

            monkeys.Add(follow);
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
        }
        if (collision.CompareTag("Swing"))
        {
            isSwinging = false;
            rb.gravityScale = 1f;
        }
    }


    void JupmMove()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJump = true;
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
        Debug.Log("木登りモードに入りました");
        isClimbing = true;
        rb.gravityScale = 0f;  // 重力停止
        rb.linearVelocity = Vector2.zero;
    }

    void Swinging()
    {
        isSwinging = true;
        rb.gravityScale = 0f;  // 重力停止
        rb.linearVelocity = Vector2.zero;
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
    }

    void UpdateMonkeyOffsets()
    {
        for (int i = 0; i < monkeys.Count; i++)
        {
            FollowPlayerTest mk = monkeys[i];
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
