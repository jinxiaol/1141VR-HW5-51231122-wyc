using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追蹤目標")]
    public Transform target;

    [Header("視角參數")]
    public Vector3 offset = new Vector3(0f, 3f, -8f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null)
            return;

        // 1. 計算攝影機的目標位置 (位置計算保持不變)
        Vector3 desiredPosition = target.position + offset;

        // 平滑移動攝影機
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 2. 【關鍵修改】：修正 LookAt 函式，只在 Y 軸上旋轉

        // 計算看向目標的旋轉角度 (四元數)
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // 獲取這個旋轉的 Euler 角度
        Vector3 euler = targetRotation.eulerAngles;

        // 將 X 軸 (俯仰) 旋轉角度強制設為 0
        euler.x = 0;

        // 將 Z 軸 (翻滾) 旋轉角度強制設為 0
        euler.z = 0;

        // 將修正過後的角度平滑地應用於攝影機
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), smoothSpeed * Time.deltaTime);
    }
}
