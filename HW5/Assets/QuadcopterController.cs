using UnityEngine;
using System.Collections.Generic;

public class QuadcopterController : MonoBehaviour
{
    // --- �ѼƳ]�w�G�z�ݭn�b Inspector �վ�o�Ǽƭ� ---
    [Header("���ʳt�׳]�w")]
    public float moveSpeed = 5f;     // �e�i/��h���t��
    public float rotationSpeed = 100f; // ���k����t�� (��/��)
    public float verticalSpeed = 3f;   // �����ɭ��t��

    [Header("�����ɮ�")]
    // �O�����ܡA�Ω�b Inspector �즲����
    public AudioClip flyingSound;
    public AudioClip collisionSound;
    public AudioClip passSound;

    // --- ����P���A ---
    private Rigidbody rb;
    private AudioSource audioSource;
    private HashSet<int> passedZones = new HashSet<int>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // ������򪺭��歵��
        if (audioSource != null && flyingSound != null)
        {
            audioSource.clip = flyingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        // 1. 旋轉控制 (A/D 或左右方向鍵)
        float turnInput = Input.GetAxis("Horizontal");
        float yawRotation = turnInput * rotationSpeed * Time.deltaTime;

        // 左右旋轉：仍然使用 transform.Rotate() 實現原地轉向
        transform.Rotate(Vector3.up, yawRotation);

        // 2. 獲取前進/後退/升降輸入
        float forwardInput = Input.GetAxis("Vertical");
        float liftInput = 0f;
        if (Input.GetKey(KeyCode.Space))
            liftInput = 1f;
        if (Input.GetKey(KeyCode.LeftShift))
            liftInput = -1f;

        // 3. 計算目標速度
        Vector3 targetVelocity = transform.forward * forwardInput * moveSpeed;
        targetVelocity += Vector3.up * liftInput * verticalSpeed;

        // 4. 應用速度到 Rigidbody
        if (rb != null)
        {
            // 將 Rigidbody 的速度設定為我們計算出的目標速度
            rb.linearVelocity = targetVelocity;

            // 為了避免旋轉過快，鎖定角速度 (可選)
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (audioSource != null && collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
            Debug.Log("�����ê���I");
        }
    }

    // �B�zĲ�o�ƥ� (���Q�q�L PassZone �ϰ�)
    // 【新版】允許重複觸發音效

    void OnTriggerEnter(Collider other)
    {
        // 檢查是否為 PassZone
        if (other.gameObject.CompareTag("PassZone"))
        {
            // 直接播放通過音效，不再檢查是否已經通過過
            if (audioSource != null && passSound != null)
            {
                audioSource.PlayOneShot(passSound);
            }

            Debug.Log("通過區域音效播放！");
        }
    }
}