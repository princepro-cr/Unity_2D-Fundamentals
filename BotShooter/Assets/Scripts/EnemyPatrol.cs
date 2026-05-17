// ============================================================
// Student Number : 223051684
// Enemy patrols between two points and damages player on contact
// Adds a red Point Light 2D for Unit 9 danger/tension lighting
// ============================================================

using UnityEngine;
using UnityEngine.Rendering.Universal;   // Required for Light2D

public class EnemyPatrol : MonoBehaviour
{
    // -------------------------------------------------------
    // PATROL SETTINGS
    // -------------------------------------------------------
    [Header("Patrol")]
    public float speed = 2f;
    public float leftBound = -5f;      // Exposed so you can set per-enemy in Inspector
    public float rightBound = 5f;      // instead of the hardcoded ±5 from before

    // -------------------------------------------------------
    // DANGER LIGHT SETTINGS
    // -------------------------------------------------------
    [Header("Danger Light")]
    public float lightIntensity = 1.0f;
    public float pulseSpeed = 3.5f;    // Slightly faster pulse than coins = more tension
    public float pulseAmount = 0.35f;
    public Color dangerColor = new Color(0.9f, 0.15f, 0.1f); // Deep red

    // -------------------------------------------------------
    // PRIVATE
    // -------------------------------------------------------
    private bool movingRight = true;
    private float damageCooldown = 1f;
    private float lastDamageTime = -999f;
    private Light2D enemyLight;

    // -------------------------------------------------------
    // START
    // -------------------------------------------------------
    void Start()
    {
        // --- Create the danger Point Light 2D at runtime ---
        GameObject lightObj = new GameObject("EnemyDangerLight");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.zero;

        enemyLight = lightObj.AddComponent<Light2D>();
        enemyLight.lightType = Light2D.LightType.Point;
        enemyLight.color = dangerColor;
        enemyLight.intensity = lightIntensity;
        enemyLight.pointLightOuterRadius = 2.0f;   // Larger radius than coins — more threatening
        enemyLight.pointLightInnerRadius = 0.4f;
    }

    // -------------------------------------------------------
    // UPDATE
    // -------------------------------------------------------
    void Update()
    {
        // --- Patrol movement ---
        float dir = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * speed * Time.deltaTime * dir);

        if (transform.position.x > rightBound) movingRight = false;
        if (transform.position.x < leftBound) movingRight = true;

        // --- Pulse the danger light ---
        if (enemyLight == null) return;
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        enemyLight.intensity = lightIntensity + pulse;
    }

    // -------------------------------------------------------
    // COLLISION — damages player on contact
    // -------------------------------------------------------
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                lastDamageTime = Time.time;

                // Flash the light bright red on hit for impact feedback
                if (enemyLight != null)
                    StartCoroutine(HitFlash());

                FindObjectOfType<GameManager>().LoseLife();
            }
        }
    }

    // -------------------------------------------------------
    // HIT FLASH — spikes intensity on player contact then returns
    // -------------------------------------------------------
    private System.Collections.IEnumerator HitFlash()
    {
        enemyLight.intensity = 3.5f;        // Spike bright on hit
        yield return new WaitForSeconds(0.08f);
        enemyLight.intensity = lightIntensity; // Return to normal
    }
}