// ============================================================
// Student Number : 223051684
// Destroys coin on player contact and notifies GameManager
// Adds a pulsing yellow Point Light 2D for Unit 9 lighting
// Unit 10 : plays collect sound on pickup
// ============================================================

using UnityEngine;
using UnityEngine.Rendering.Universal;   // Required for Light2D

public class CoinScript : MonoBehaviour
{
    // -------------------------------------------------------
    // LIGHT SETTINGS — tweak in Inspector on the Coin prefab
    // -------------------------------------------------------
    [Header("Coin Glow")]
    public float lightIntensity = 1.2f;     // Base brightness of the glow
    public float pulseSpeed = 2.5f;     // How fast the light pulses
    public float pulseAmount = 0.3f;     // How much intensity varies up and down
    public Color glowColor = new Color(1f, 0.92f, 0.3f); // Warm yellow

    // -------------------------------------------------------
    // AUDIO SETTINGS — drag a clip onto the Coin prefab
    // -------------------------------------------------------
    [Header("Audio")]
    public AudioClip collectSound;          // Assign in Inspector (e.g. coin_collect.wav)

    // -------------------------------------------------------
    // PRIVATE
    // -------------------------------------------------------
    private Animator anim;
    private Collider2D col;
    private bool collected = false; // Prevents double collection
    private Light2D coinLight;          // The Point Light 2D on this coin
    private AudioSource audioSource;        // Audio Source component on this prefab

    // -------------------------------------------------------
    // START
    // -------------------------------------------------------
    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();  // Requires Audio Source on prefab

        // --- Create the Point Light 2D at runtime ---
        // This means you do NOT need to manually add a light to every coin
        // in the scene — the prefab handles it automatically.
        GameObject lightObj = new GameObject("CoinGlow");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.zero;

        coinLight = lightObj.AddComponent<Light2D>();
        coinLight.lightType = Light2D.LightType.Point;
        coinLight.color = glowColor;
        coinLight.intensity = lightIntensity;
        coinLight.pointLightOuterRadius = 1.5f;   // How far the glow reaches
        coinLight.pointLightInnerRadius = 0.3f;   // Solid core radius
    }

    // -------------------------------------------------------
    // UPDATE — drives the pulse animation on the light
    // -------------------------------------------------------
    void Update()
    {
        if (coinLight == null || collected) return;

        // Sine wave makes the light breathe up and down smoothly
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        coinLight.intensity = lightIntensity + pulse;
    }

    // -------------------------------------------------------
    // COLLECTION
    // -------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            collected = true;
            col.enabled = false;

            // Fade the light out quickly so it doesn't pop off
            if (coinLight != null)
                StartCoroutine(FadeLight());

            // Play the collect sound at the coin's world position.
            // PlayClipAtPoint spawns a temporary AudioSource so the sound
            // keeps playing even after this GameObject is destroyed.
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            anim.SetTrigger("Collect");
            FindObjectOfType<GameManager>().AddCoin();
            Destroy(gameObject, 0.6f);
        }
    }

    // -------------------------------------------------------
    // FADE LIGHT — smoothly reduces intensity to zero
    // over the same 0.6 s window as the collect animation
    // -------------------------------------------------------
    private System.Collections.IEnumerator FadeLight()
    {
        float startIntensity = coinLight.intensity;
        float elapsed = 0f;
        float fadeDuration = 0.5f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            coinLight.intensity = Mathf.Lerp(startIntensity, 0f, elapsed / fadeDuration);
            yield return null;
        }

        coinLight.intensity = 0f;
    }
}