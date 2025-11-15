using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // プレイヤー
    public float smoothSpeed = 5f;
    public Vector3 offset;   // カメラとプレイヤーの距離

void FixedUpdate()
{
    if (target == null) return;

    Vector3 desiredPosition = target.position + offset;
    transform.position = Vector3.Lerp(
        transform.position,
        desiredPosition,
        smoothSpeed * Time.fixedDeltaTime
    );
}
}
