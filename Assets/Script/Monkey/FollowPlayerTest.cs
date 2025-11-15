using UnityEngine;

public class FollowPlayerTest : MonoBehaviour
{
    public Transform target;  // プレイヤー
    public float followSpeed = 5f;
    public Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            target.position + offset,
            followSpeed * Time.deltaTime   
        );
    }

}
