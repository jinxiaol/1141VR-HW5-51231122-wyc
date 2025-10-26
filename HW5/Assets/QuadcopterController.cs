using UnityEngine;
using System.Collections.Generic; // �ɤJ�x�����X�A�Ω�s��w�q�L���ϰ�ID

public class QuadcopterController : MonoBehaviour
{
    // --- �b Unity Inspector ���]�w�����}�ܼ� ---

    [Header("���汱��Ѽ�")]
    public float thrustForce = 100f; // �������i�O (�v�T�ɭ�)
    public float pitchTorque = 5f;   // ������O (�v�T�e��ɱ�)
    public float yawTorque = 5f;     // �����O (�v�T���k��V)

    [Header("�����ɮ�")]
    public AudioClip flyingSound;    // ����ɫ��򼽩񪺭���
    public AudioClip collisionSound; // �����ê���ɪ�����
    public AudioClip passSound;      // ���Q�q�L��ê���ɪ�����

    // --- �{�������ϥΪ����� ---
    private Rigidbody rb;
    private AudioSource audioSource;

    // --- �s�W�G�l�ܤw�g�q�L�� PassZone ---
    // �ΨӦs��w�gĲ�o�L�q�L���Ī� PassZone ���� Instance ID
    private HashSet<int> passedZones = new HashSet<int>();

    void Start()
    {
        // 1. ���o Rigidbody �M AudioSource ����
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // ���F�T�O Rigidbody ��l����v�A�i�H�D�ʳ��
        rb.WakeUp();

        // 2. ������򪺭��歵��
        if (audioSource != null && flyingSound != null)
        {
            audioSource.clip = flyingSound;
            audioSource.loop = true; // �]�w���`������
            audioSource.Play();
        }
    }

    void FixedUpdate()
    {
        // �����ɭ����� (�ϥΪť���M�� Shift ��)
        float liftInput = 0f;
        if (Input.GetKey(KeyCode.Space))      // �ť���G�W��
            liftInput = 1f;
        if (Input.GetKey(KeyCode.LeftShift))  // �� Shift�G�U��
            liftInput = -1f;

        Vector3 verticalThrust = Vector3.up * liftInput * thrustForce;
        rb.AddForce(verticalThrust, ForceMode.Acceleration);

        // ��V�M�e��ɱױ��� (�ϥ� A/D/W/S ��)
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D ��
        float verticalAxis = Input.GetAxis("Vertical");     // W/S ��

        // �I�[���k��V����O (Yaw)
        rb.AddTorque(Vector3.up * horizontalInput * yawTorque, ForceMode.Acceleration);

        // �I�[�e��ɱת���O (Pitch)
        rb.AddTorque(transform.right * verticalAxis * pitchTorque, ForceMode.Acceleration);
    }

    // �B�z�I���ƥ� (��������ê��)
    void OnCollisionEnter(Collision collision)
    {
        // ���]�Ҧ������ê�����]�w�F Tag �� "Obstacle"
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (audioSource != null && collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }
            Debug.Log("�����ê���I");
        }
    }

    // �B�zĲ�o�ƥ� (���Q�q�L���w�� PassZone �ϰ�)
    void OnTriggerEnter(Collider other)
    {
        // �T�OĲ�o������O PassZone 
        if (other.gameObject.CompareTag("PassZone"))
        {
            int zoneID = other.gameObject.GetInstanceID();

            // �ˬd�o�� PassZone �� ID �O�_�w�g�b�ڭ̪��O����
            if (!passedZones.Contains(zoneID))
            {
                // �p�G�O�Ĥ@���q�L
                if (audioSource != null && passSound != null)
                {
                    audioSource.PlayOneShot(passSound);
                }

                // �N�o�� PassZone �� ID �O���U�ӡA�T�O�����|�AĲ�o����
                passedZones.Add(zoneID);
                Debug.Log("���Q�q�L�@�� PassZone�I");
            }
        }
    }
}