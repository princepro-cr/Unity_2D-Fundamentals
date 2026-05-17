// ============================================================
// Student Number : 223051684
// PlayerController.cs
// Ninja character — same class name so all existing scripts
// (DeathZone, GameManager) work without any changes.
// Added: left-click spike throw.
// ============================================================
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // -------------------------------------------------------
    // MOVEMENT
    // -------------------------------------------------------
    [Header("Movement")]
    public float speed = 8f;
    public float jumpForce = 11f;

    [Header("Jump Feel")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Legs")]
    public Transform legLeft;
    public Transform legRight;

    // -------------------------------------------------------
    // SPIKE THROW
    // -------------------------------------------------------
    [Header("Spike Throw")]
    public GameObject spikePrefab;          // drag Spike prefab here in Inspector
    public Transform throwPoint;            // empty child GameObject at hand position
    public float throwCooldown = 0.4f;      // seconds between throws

    private float lastThrowTime = -999f;

    // -------------------------------------------------------
    // AUDIO
    // -------------------------------------------------------
    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip hurtSound;
    public AudioClip landSound;
    public AudioClip throwSound;            // whoosh/swipe when spike is thrown

    // -------------------------------------------------------
    // PRIVATE
    // -------------------------------------------------------
    private Rigidbody2D rb;
    private bool onGround = false;
    private bool wasOnGround = false;
    private float t = 0f;
    private float facing = 1f;
    private Vector3 originalScale;
    private AudioSource audioSource;

    // -------------------------------------------------------
    // START
    // -------------------------------------------------------
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;
    }

    // -------------------------------------------------------
    // UPDATE
    // -------------------------------------------------------
    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // --- Direction flip ---
        if (move > 0f) facing = 1f;
        if (move < 0f) facing = -1f;
        transform.localScale = new Vector3(
            originalScale.x * facing,
            originalScale.y,
            originalScale.z
        );

        // --- Leg swing animation ---
        bool walking = Mathf.Abs(move) > 0.05f && onGround;
        if (walking) t += Time.deltaTime * 9f;
        else t = Mathf.MoveTowards(t, 0f, Time.deltaTime * 12f);

        float swing = Mathf.Sin(t) * 22f;
        float leftSwing = swing * facing;
        float rightSwing = -swing * facing;

        if (legLeft) legLeft.localRotation = Quaternion.Euler(0, 0, leftSwing);
        if (legRight) legRight.localRotation = Quaternion.Euler(0, 0, rightSwing);

        // --- Jump ---
        if (Input.GetButtonDown("Jump") && onGround)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            if (audioSource && jumpSound) audioSource.PlayOneShot(jumpSound);
        }

        // --- Land sound ---
        if (onGround && !wasOnGround)
            if (audioSource && landSound) audioSource.PlayOneShot(landSound);
        wasOnGround = onGround;

        // --- Spike throw — F key ---
        if (Input.GetKeyDown(KeyCode.F) && Time.time - lastThrowTime >= throwCooldown)
            ThrowSpike();
    }

    // -------------------------------------------------------
    // FIXED UPDATE — gravity tweaks
    // -------------------------------------------------------
    void FixedUpdate()
    {
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }

    // -------------------------------------------------------
    // THROW SPIKE
    // -------------------------------------------------------
    void ThrowSpike()
    {
        if (spikePrefab == null) return;

        lastThrowTime = Time.time;

        Vector3 spawnPos = throwPoint != null ? throwPoint.position : transform.position;
        GameObject spike = Instantiate(spikePrefab, spawnPos, Quaternion.identity);
        spike.GetComponent<SpikeProjectile>().SetDirection(facing);

        if (audioSource && throwSound) audioSource.PlayOneShot(throwSound);
    }

    // -------------------------------------------------------
    // COLLISION
    // -------------------------------------------------------
    void OnCollisionEnter2D(Collision2D c) { onGround = true; }
    void OnCollisionExit2D(Collision2D c) { onGround = false; }

    // -------------------------------------------------------
    // PUBLIC — called by GameManager / DeathZone
    // -------------------------------------------------------
    public void PlayHurtSound()
    {
        if (audioSource && hurtSound) audioSource.PlayOneShot(hurtSound);
    }
}