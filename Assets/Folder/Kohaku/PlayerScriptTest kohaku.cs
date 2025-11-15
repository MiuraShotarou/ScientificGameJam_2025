using UnityEngine;


public class PlayerScriptTestkohaku : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float distance;
    private bool monkey1 = false;
    private bool monkey2 = false;
    private int facing = 1;

    private Rigidbody2D rb;

    FollowPlayerTest follow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if (horizontal != 0)
        {
            facing = horizontal > 0 ? 1 : -1;
        }

        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);

        if (monkey1)
        {
            JupmMove();
            follow.offset = new Vector3(distance * facing, 0f, 0f);
        }
        if (monkey2)
        {
            RunMove();
            follow.offset = new Vector3(distance * 2 * facing, 0f, 0f);
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monkey_1"))
        {
            monkey1 = true;

            collision.gameObject.layer = LayerMask.NameToLayer("PlayerFollowed");

            follow = collision.gameObject.AddComponent<FollowPlayerTest>();
            follow.target = this.transform;
        }

        if (collision.gameObject.CompareTag("Monkey_2"))
        {
            monkey2 = true;

            collision.gameObject.layer = LayerMask.NameToLayer("PlayerFollowed");

            follow = collision.gameObject.AddComponent<FollowPlayerTest>();
            follow.target = this.transform;
        }

    }

    void JupmMove()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void RunMove()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            speed *= 1.2f;
        }       
    }

    void Reset()
    {
        speed = 5f;
        jumpForce = 5f;
        monkey1 = false;
        monkey2 = false;
    }
}
