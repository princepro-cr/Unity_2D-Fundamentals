// ============================================================
// Student Number : 223051684
// ZoneLightingController — handles lighting AND music crossfade
// per zone. Attach to a Manager GameObject in the scene.
// ============================================================
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ZoneLightingController : MonoBehaviour
{
    public static ZoneLightingController Instance;

    [Header("Global Light")]
    public Light2D globalLight;

    [Header("Transition Speed")]
    public float transitionDuration = 2f;

    [Header("Zone Settings")]
    public ZoneSettings[] zones;

    // ── Two Audio Sources for crossfading ──────────────────────
    // Unity can't smoothly swap a single AudioSource, so we use
    // two and ping-pong between them (like a DJ mixer).
    private AudioSource musicSourceA;
    private AudioSource musicSourceB;
    private bool usingA = true;          // which source is currently "active"

    private Coroutine currentTransition;

    [System.Serializable]
    public class ZoneSettings
    {
        public ZoneTrigger.ZoneType zoneType;

        [Header("Lighting")]
        public Color globalLightColor;
        [Range(0f, 1f)]
        public float globalLightIntensity;
        public Color cameraBackgroundColor;

        [Header("Music")]
        public AudioClip musicClip;          // drag your mp3/wav here in Inspector
        [Range(0f, 1f)]
        public float musicVolume = 0.5f;     // per-zone volume so forest/space feel right
    }

    // ── Lifecycle ───────────────────────────────────────────────
    private void Awake()
    {
        Instance = this;

        // Create both audio sources on this GameObject at runtime —
        // no need to add them manually in the Inspector.
        musicSourceA = gameObject.AddComponent<AudioSource>();
        musicSourceB = gameObject.AddComponent<AudioSource>();

        ConfigureSource(musicSourceA);
        ConfigureSource(musicSourceB);
    }

    private void ConfigureSource(AudioSource src)
    {
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f;   // 2D sound — not positional
        src.volume = 0f;
    }

    // ── Public API (called by ZoneTrigger) ──────────────────────
    public void TransitionToZone(ZoneTrigger.ZoneType zone)
    {
        foreach (ZoneSettings z in zones)
        {
            if (z.zoneType == zone)
            {
                if (currentTransition != null)
                    StopCoroutine(currentTransition);
                currentTransition = StartCoroutine(DoTransition(z));
                return;
            }
        }
    }

    // ── Transition coroutine ────────────────────────────────────
    private IEnumerator DoTransition(ZoneSettings target)
    {
        float elapsed = 0f;

        // Lighting start values
        Color startLightColor = globalLight.color;
        float startIntensity = globalLight.intensity;
        Color startBgColor = Camera.main.backgroundColor;

        // Music: figure out which source is outgoing and which is incoming
        AudioSource outgoing = usingA ? musicSourceA : musicSourceB;
        AudioSource incoming = usingA ? musicSourceB : musicSourceA;
        usingA = !usingA;

        float startOutVol = outgoing.volume;

        // Only swap clip if the incoming zone has a different track
        if (target.musicClip != null && incoming.clip != target.musicClip)
        {
            incoming.clip = target.musicClip;
            incoming.volume = 0f;
            incoming.Play();
        }

        // Crossfade everything simultaneously
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            // Lighting
            globalLight.color = Color.Lerp(startLightColor, target.globalLightColor, t);
            globalLight.intensity = Mathf.Lerp(startIntensity, target.globalLightIntensity, t);
            Camera.main.backgroundColor = Color.Lerp(startBgColor, target.cameraBackgroundColor, t);

            // Music volumes
            outgoing.volume = Mathf.Lerp(startOutVol, 0f, t);
            incoming.volume = Mathf.Lerp(0f, target.musicVolume, t);

            yield return null;
        }

        // Snap final values
        globalLight.color = target.globalLightColor;
        globalLight.intensity = target.globalLightIntensity;
        Camera.main.backgroundColor = target.cameraBackgroundColor;

        outgoing.volume = 0f;
        outgoing.Stop();           // free up the outgoing source
        incoming.volume = target.musicVolume;
    }
}