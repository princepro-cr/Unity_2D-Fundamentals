// ============================================================
// Student Number : 223051684
// Respawns player at last checkpoint and triggers camera shake
// ============================================================

using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private float damageCooldown = 1f;
    private float lastDamageTime = -999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                lastDamageTime = Time.time;

                // ✅ Get respawn position from checkpoint
                Vector3 respawn = CheckpointManager.Instance.GetRespawnPoint();

                // ✅ Reset velocity so player doesn't fly through floor
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }

                // ✅ Move player to respawn point
                other.transform.position = respawn;

                // Camera shake on death
                CameraFollow camFollow = Camera.main.GetComponent<CameraFollow>();
                if (camFollow != null)
                    camFollow.TriggerShake();

                // Lose a life
                FindObjectOfType<GameManager>().LoseLife();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (CheckpointManager.Instance != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(
                CheckpointManager.Instance.GetRespawnPoint(),
                0.25f
            );
        }
    }
}