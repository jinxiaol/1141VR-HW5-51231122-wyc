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

        // 1. �p����v�����ؼЦ�m (��m�p��O������)
        Vector3 desiredPosition = target.position + offset;

        // ���Ʋ�����v��
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 2. �i����ק�j�G�ץ� LookAt �禡�A�u�b Y �b�W����

        // �p��ݦV�ؼЪ����ਤ�� (�|����)
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // ����o�ӱ��઺ Euler ����
        Vector3 euler = targetRotation.eulerAngles;

        // �N X �b (����) ���ਤ�ױj��]�� 0
        euler.x = 0;

        // �N Z �b (½�u) ���ਤ�ױj��]�� 0
        euler.z = 0;

        // �N�ץ��L�᪺���ץ��Ʀa���Ω���v��
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), smoothSpeed * Time.deltaTime);
    }
}
