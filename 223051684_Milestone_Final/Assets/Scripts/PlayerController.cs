// ============================================================
// Student Number : 223051684
// ============================================================
// HOW TO SEE THE MECHANIC IN ACTION:
// 1. Open SampleScene in Assets > Scenes
// 2. Press the PLAY button at the top of the Unity Editor
// 3. Controls:
//       A / D  or  LEFT / RIGHT Arrow  =  Move left and right
//       SPACE                          =  Jump
// 4. Walk into a gold COIN to collect it (Coins counter increases)
// 5. Fall off a platform to lose a life (camera shakes on death)
// 6. Lose all 3 lives and the scene resets automatically
// ============================================================

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // -------------------------------------------------------
    // MOVEMENT SETTINGS
    // -------------------------------------------------------
    [Header("Movement")]
    public float speed = 8f;    // How fast the player runs left and right
    public float jumpForce = 11f;   // How high the player jumps

    [Header("Jump Feel")]
    public float fallMultiplier = 2.5f;  // Makes falling faster so jumps feel snappy
    public float lowJumpMultiplier = 2f;    // Tap = short hop, hold = full jump

    [Header("Legs")]
    public Transform legLeft;    // Drag LegLeft  GameObject here in Inspector
    public Transform legRight;   // Drag LegRight GameObject here in Inspector

    // -------------------------------------------------------
    // AUDIO SETTINGS
    // -------------------------------------------------------
    [Header("Audio")]
    public AudioClip jumpSound;     // Assign in Inspector (e.g. jump.wav)
    public AudioClip hurtSound;     // Assign in Inspector (e.g. hurt.wav)
    public AudioClip landSound;     // Assign in Inspector (e.g. land.wav)

    // -------------------------------------------------------
    // PRIVATE VARIABLES
    // -------------------------------------------------------
    private Rigidbody2D rb;
    private bool onGround = false;
    private bool wasOnGround = false;  // Tracks previous frame ground state for landing SFX
    private float t = 0f;
    private float facing = 1f;
    private Vector3 originalScale;
    private AudioSource audioSource;             // Audio Source component on the Player

    // -------------------------------------------------------
    // START
    // -------------------------------------------------------
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>(); // Requires Audio Source on Player
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

            // PlayOneShot so rapid jumps don't cut each other off
            if (audioSource != null && jumpSound != null)
                audioSource.PlayOneShot(jumpSound);
        }

        // --- Land sound ---
        // Triggers once when the player touches the ground after being airborne
        if (onGround && !wasOnGround)
        {
            if (audioSource != null && landSound != null)
                audioSource.PlayOneShot(landSound);
        }
        wasOnGround = onGround;
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
    // COLLISION DETECTION
    // -------------------------------------------------------
    void OnCollisionEnter2D(Collision2D c) { onGround = true; }
    void OnCollisionExit2D(Collision2D c) { onGround = false; }

    // -------------------------------------------------------
    // PUBLIC — called by GameManager or DeathZone when player is hurt
    // -------------------------------------------------------
    public void PlayHurtSound()
    {
        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound);
    }
}