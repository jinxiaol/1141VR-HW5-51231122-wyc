using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追蹤目標")]
    public Transform target;

    [Header("視角參數")]
    public Vector3 offset = new Vector3(0f, 3f, -8f);
    public float smoothSpeed = 5f;

    // 【鏡頭跟隨旋轉版本】
    void LateUpdate()
    {
        if (target == null)
            return;

        // 1. 【關鍵修改】：計算攝影機的目標位置
        // 使用 target.rotation * offset 將 offset 向量轉換到目標物件的本地座標系
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // 平滑移動攝影機
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 2. 讓攝影機永遠看著目標
        transform.LookAt(target);
    }
}
