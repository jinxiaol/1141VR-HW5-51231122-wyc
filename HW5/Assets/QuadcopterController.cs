using UnityEngine;
using System.Collections.Generic; // 導入泛型集合，用於存放已通過的區域ID

public class QuadcopterController : MonoBehaviour
{
    // --- 在 Unity Inspector 中設定的公開變數 ---

    [Header("飛行控制參數")]
    public float thrustForce = 100f; // 垂直推進力 (影響升降)
    public float pitchTorque = 5f;   // 俯仰扭力 (影響前後傾斜)
    public float yawTorque = 5f;     // 偏航扭力 (影響左右轉向)

    [Header("音效檔案")]
    public AudioClip flyingSound;    // 飛行時持續播放的音效
    public AudioClip collisionSound; // 撞到障礙物時的音效
    public AudioClip passSound;      // 順利通過障礙物時的音效

    // --- 程式內部使用的元件 ---
    private Rigidbody rb;
    private AudioSource audioSource;

    // --- 新增：追蹤已經通過的 PassZone ---
    // 用來存放已經觸發過通過音效的 PassZone 物件的 Instance ID
    private HashSet<int> passedZones = new HashSet<int>();

    void Start()
    {
        // 1. 取得 Rigidbody 和 AudioSource 元件
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // 為了確保 Rigidbody 初始不休眠，可以主動喚醒
        rb.WakeUp();

        // 2. 播放持續的飛行音效
        if (audioSource != null && flyingSound != null)
        {
            audioSource.clip = flyingSound;
            audioSource.loop = true; // 設定為循環播放
            audioSource.Play();
        }
    }

    void FixedUpdate()
    {
        // 垂直升降控制 (使用空白鍵和左 Shift 鍵)
        float liftInput = 0f;
        if (Input.GetKey(KeyCode.Space))      // 空白鍵：上升
            liftInput = 1f;
        if (Input.GetKey(KeyCode.LeftShift))  // 左 Shift：下降
            liftInput = -1f;

        Vector3 verticalThrust = Vector3.up * liftInput * thrustForce;
        rb.AddForce(verticalThrust, ForceMode.Acceleration);

        // 轉向和前後傾斜控制 (使用 A/D/W/S 鍵)
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D 鍵
        float verticalAxis = Input.GetAxis("Vertical");     // W/S 鍵

        // 施加左右轉向的扭力 (Yaw)
        rb.AddTorque(Vector3.up * horizontalInput * yawTorque, ForceMode.Acceleration);

        // 施加前後傾斜的扭力 (Pitch)
        rb.AddTorque(transform.right * verticalAxis * pitchTorque, ForceMode.Acceleration);
    }

    // 處理碰撞事件 (撞到實體障礙物)
    void OnCollisionEnter(Collision collision)
    {
        // 假設所有實體障礙物都設定了 Tag 為 "Obstacle"
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (audioSource != null && collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
            Debug.Log("撞到障礙物！");
        }
    }

    // 處理觸發事件 (順利通過指定的 PassZone 區域)
    void OnTriggerEnter(Collider other)
    {
        // 確保觸發的物件是 PassZone 
        if (other.gameObject.CompareTag("PassZone"))
        {
            int zoneID = other.gameObject.GetInstanceID();

            // 檢查這個 PassZone 的 ID 是否已經在我們的記錄中
            if (!passedZones.Contains(zoneID))
            {
                // 如果是第一次通過
                if (audioSource != null && passSound != null)
                {
                    audioSource.PlayOneShot(passSound);
                }

                // 將這個 PassZone 的 ID 記錄下來，確保它不會再觸發音效
                passedZones.Add(zoneID);
                Debug.Log("順利通過一個 PassZone！");
            }
        }
    }
}