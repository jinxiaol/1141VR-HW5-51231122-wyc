using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("�l�ܥؼ�")]
    public Transform target;

    [Header("�����Ѽ�")]
    public Vector3 offset = new Vector3(0f, 3f, -8f);
    public float smoothSpeed = 5f;

    // �i���Y���H���઩���j
    void LateUpdate()
    {
        if (target == null)
            return;

        // 1. �i����ק�j�G�p����v�����ؼЦ�m
        // �ϥ� target.rotation * offset �N offset �V�q�ഫ��ؼЪ��󪺥��a�y�Шt
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // ���Ʋ�����v��
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 2. ����v���û��ݵۥؼ�
        transform.LookAt(target);
    }
}
