using UnityEngine;

public class TestControllerKohaku : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float moveSpeed = 5f;    // ���ړ��̑���
    public float jumpForce = 5f;    // �W�����v�̋���

    [Header("�ڒn����")]
    public Transform groundCheck;   // �����̃`�F�b�N�ʒu
    public float groundCheckRadius = 0.1f; // �ڒn����̉~�̔��a
    public LayerMask groundLayer;   // �n�ʂ̃��C���[

    private Rigidbody2D rb;
    private float inputX;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // �L�[�{�[�h�̍��E���́iA,D / ��,���j
        inputX = Input.GetAxisRaw("Horizontal");

        // �X�y�[�X�L�[�ŃW�����v�i�ڒn���Ă���Ƃ������j
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        // ���ړ��i���x�𒼐ڏ��������j
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

        // �n�ʂɑ����t���Ă��邩����
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
        // ��U�c�̑��x�����Z�b�g���Ă��������ɗ͂�������
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // �V�[����Őڒn����̉~��������悤�ɃM�Y���`��i�m�F�p�j
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
