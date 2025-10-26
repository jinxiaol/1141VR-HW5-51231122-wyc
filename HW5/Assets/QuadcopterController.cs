using UnityEngine;
using System.Collections.Generic;

public class QuadcopterController : MonoBehaviour
{
    [Header("飛行控制參數")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float verticalSpeed = 3f;

    [Header("音效叄數設定")]
    public AudioClip flyingSound;
    public AudioClip collisionSound;
    public AudioClip passSound;
    private Rigidbody rb;
    private AudioSource audioSource;
    private HashSet<int> passedZones = new HashSet<int>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && flyingSound != null)
        {
            audioSource.clip = flyingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        float turnInput = Input.GetAxis("Horizontal");
        float yawRotation = turnInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, yawRotation);
        float forwardInput = Input.GetAxis("Vertical");
        float liftInput = 0f;
        if (Input.GetKey(KeyCode.Space))
            liftInput = 1f;
        if (Input.GetKey(KeyCode.LeftShift))
            liftInput = -1f;
        Vector3 targetVelocity = transform.forward * forwardInput * moveSpeed;
        targetVelocity += Vector3.up * liftInput * verticalSpeed;
        if (rb != null)
        {
            rb.linearVelocity = targetVelocity;
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
            Debug.Log("實體碰撞");
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PassZone"))
        {
            if (audioSource != null && passSound != null)
            {
                audioSource.PlayOneShot(passSound);
            }

            Debug.Log("通過區域音效播放！");
        }
    }
}