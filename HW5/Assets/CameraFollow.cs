using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("�l�ܥؼ�")]
    public Transform target;

    [Header("�����Ѽ�")]
    public Vector3 offset = new Vector3(0f, 3f, -8f);
    public float smoothSpeed = 5f;
    void LateUpdate()
    {
        if (target == null)
            return;
        Vector3 desiredPosition = target.position + target.rotation * offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        transform.LookAt(target);
    }
}
