// ============================================================
// Student Number : 223051684
// CameraFollow.cs
// Smooth SmoothDamp follow + bounds clamping + screen shake
// + dead zone to reduce jitter (Unit 9 advanced technique)
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
    public float deadZoneX = 0.5f;             // Camera won't move if player moves less than this
    public float deadZoneY = 0.3f;

    [Header("Camera Bounds (covers all 3 zones)")]
    public bool useBounds = true;
    public float minX = -10f;
    public float maxX = 200f;                  // ← Extended to cover Forest+Savanna+Space
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
    private Vector3 lastTargetPos;             // Used for dead zone calculation

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
        // Only update target position if player moved beyond dead zone
        // This removes jitter from tiny idle movements
        float deltaX = Mathf.Abs(target.position.x - lastTargetPos.x);
        float deltaY = Mathf.Abs(target.position.y - lastTargetPos.y);

        if (deltaX > deadZoneX || deltaY > deadZoneY)
        {
            lastTargetPos = target.position;
        }

        // --- Build desired position using dead zone position ---
        Vector3 desiredPos = new Vector3(
            lastTargetPos.x + offset.x,
            lastTargetPos.y + offset.y,
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
    // TRIGGER SHAKE
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