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

    // �o���ڭ̦b Update() �B�z���ʩM����A�o�˷|���F�ӡ]�D���z���ʡ^
    void Update()
    {
        // 1. ���౱�� (A/D �Υ��k��V��)
        // ��o A/D �䪺��J�� (�d�� -1 �� 1)
        float turnInput = Input.GetAxis("Horizontal");

        // ���k����G�ϥ� transform.Rotate() ��{��a��V
        float yawRotation = turnInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, yawRotation);

        // 2. �e�i/��h���� (W/S �ΤW�U��V��)
        // ��o W/S �䪺��J�� (�d�� -1 �� 1)
        float forwardInput = Input.GetAxis("Vertical");

        // �e�i/��h�G�ϥ� transform.Translate() �u�۪���ۨ����e��(transform.forward)����
        Vector3 movement = transform.forward * forwardInput * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World); // �����ʻP�@�ɮy�Шt��� (�i��A�� transform.forward ��X�A)

        // 3. �����ɭ����� (Space �W�� / LeftShift �U��)
        float liftInput = 0f;
        if (Input.GetKey(KeyCode.Space))      // �ť���G�W��
            liftInput = 1f;
        if (Input.GetKey(KeyCode.LeftShift))  // �� Shift�G�U��
            liftInput = -1f;

        // �������ʡG�����ק� y �b��m
        transform.position += Vector3.up * liftInput * verticalSpeed * Time.deltaTime;

        // �i���n�j�ѩ�ڭ̥D�n�ϥ� transform �i�沾�ʡA�o�̻ݭn���] Rigidbody ���t��
        // �_�h���z�����i��y���ưʩγt�ײֿn
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // �B�z�I���ƥ� (��������ê��)
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
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PassZone"))
        {
            int zoneID = other.gameObject.GetInstanceID();

            // �u���Ĥ@���i�J PassZone �ɤ~���񭵮�
            if (!passedZones.Contains(zoneID))
            {
                if (audioSource != null && passSound != null)
                {
                    audioSource.PlayOneShot(passSound);
                }

                passedZones.Add(zoneID);
                Debug.Log("���Q�q�L�@�� PassZone�I");
            }
        }
    }
}