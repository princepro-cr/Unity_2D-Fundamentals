// ============================================================
// Student Number : 223051684
// CameraFollow.cs
// Smooth SmoothDamp follow + bounds clamping + screen shake
// + dead zone to reduce jitter (Unit 9 advanced technique)
// + look ahead in movement direction
// + look down when falling so player can see landing
// ============================================================

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // -------------------------------------------------------
    // INSPECTOR SETTINGS
    // -------------------------------------------------------
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public float smoothTime = 0.25f;
    public Vector2 offset = new Vector2(0f, 1f);

    [Header("Dead Zone (reduces jitter on small movements)")]
    public float deadZoneX = 0.5f;
    public float deadZoneY = 0.3f;

    [Header("Look Ahead")]
    public float lookAheadX = 2.5f;        // how far ahead horizontally to look
    public float lookAheadSmooth = 0.4f;   // how smoothly look ahead shifts

    [Header("Look Down on Fall")]
    public float fallLookDown = 2.5f;      // how far down to shift when falling
    public float fallThreshold = -4f;      // vertical speed that triggers look down
    public float lookDownSmooth = 0.3f;    // how smoothly look down kicks in

    [Header("Camera Bounds (covers all 3 zones)")]
    public bool useBounds = true;
    public float minX = -10f;
    public float maxX = 200f;
    public float minY = -5f;
    public float maxY = 15f;

    [Header("Camera Shake")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.18f;

    // -------------------------------------------------------
    // PRIVATE
    // -------------------------------------------------------
    private Vector3 velocity = Vector3.zero;
    private float shakeTimer = 0f;
    private Vector3 shakeOffset = Vector3.zero;
    private Camera cam;
    private float camHalfHeight;
    private float camHalfWidth;
    private Vector3 lastTargetPos;

    // Look ahead
    private float currentLookAheadX = 0f;
    private float lookAheadVelocity = 0f;

    // Look down
    private float currentLookDown = 0f;
    private float lookDownVelocity = 0f;

    // Rigidbody reference for reading velocity
    private Rigidbody2D targetRb;

    // -------------------------------------------------------
    // START
    // -------------------------------------------------------
    void Start()
    {
        cam = GetComponent<Camera>();

        if (cam != null && cam.orthographic)
        {
            camHalfHeight = cam.orthographicSize;
            camHalfWidth = camHalfHeight * cam.aspect;
        }

        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
            lastTargetPos = target.position;

            Vector3 startPos = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                transform.position.z
            );
            transform.position = ClampToBounds(startPos);
        }
    }

    // -------------------------------------------------------
    // LATE UPDATE
    // -------------------------------------------------------
    void LateUpdate()
    {
        if (target == null) return;

        // --- Dead Zone Check ---
        float deltaX = Mathf.Abs(target.position.x - lastTargetPos.x);
        float deltaY = Mathf.Abs(target.position.y - lastTargetPos.y);
        if (deltaX > deadZoneX || deltaY > deadZoneY)
            lastTargetPos = target.position;

        // --- Look Ahead ---
        // Shift camera forward in the direction the player is moving horizontally
        float moveDir = Input.GetAxisRaw("Horizontal");
        float targetLookAheadX = moveDir * lookAheadX;
        currentLookAheadX = Mathf.SmoothDamp(
            currentLookAheadX,
            targetLookAheadX,
            ref lookAheadVelocity,
            lookAheadSmooth
        );

        // --- Look Down on Fall ---
        // When player is falling fast, shift camera downward so they can see the landing
        float verticalVelocity = targetRb != null ? targetRb.linearVelocity.y : 0f;
        float targetLookDown = verticalVelocity < fallThreshold ? -fallLookDown : 0f;
        currentLookDown = Mathf.SmoothDamp(
            currentLookDown,
            targetLookDown,
            ref lookDownVelocity,
            lookDownSmooth
        );

        // --- Build desired position ---
        Vector3 desiredPos = new Vector3(
            lastTargetPos.x + offset.x + currentLookAheadX,
            lastTargetPos.y + offset.y + currentLookDown,
            transform.position.z
        );

        // --- SmoothDamp follow ---
        Vector3 smoothedPos = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            smoothTime
        );

        // --- Clamp to level bounds ---
        smoothedPos = ClampToBounds(smoothedPos);

        // --- Camera Shake ---
        if (shakeTimer > 0f)
        {
            shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            shakeOffset = Vector3.Lerp(shakeOffset, Vector3.zero, Time.deltaTime * 20f);
        }

        transform.position = smoothedPos + shakeOffset;
    }

    // -------------------------------------------------------
    // CLAMP TO BOUNDS
    // -------------------------------------------------------
    Vector3 ClampToBounds(Vector3 pos)
    {
        if (!useBounds) return pos;
        pos.x = Mathf.Clamp(pos.x, minX + camHalfWidth, maxX - camHalfWidth);
        pos.y = Mathf.Clamp(pos.y, minY + camHalfHeight, maxY - camHalfHeight);
        return pos;
    }

    // -------------------------------------------------------
    // TRIGGER SHAKE — called by GameManager on death
    // -------------------------------------------------------
    public void TriggerShake()
    {
        shakeTimer = shakeDuration;
    }

    // -------------------------------------------------------
    // SCENE VIEW GIZMO — yellow box shows camera bounds
    // -------------------------------------------------------
    void OnDrawGizmosSelected()
    {
        if (!useBounds) return;
        Gizmos.color = Color.yellow;
        Vector3 centre = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(centre, size);
    }
}