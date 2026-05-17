// ============================================================
// Student Number : 223051684
// SpikeProjectile.cs
// No Rigidbody — pure transform movement, guaranteed to fly.
// Notifies GameManager when an enemy is killed.
// ============================================================
using UnityEngine;

public class SpikeProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 3f;

    private float direction = 1f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(float dir)
    {
        direction = dir;
        transform.localScale = new Vector3(dir, 1f, 1f);
    }

    void Update()
    {
        // Move in world space — Space.World means it ignores any parent rotation
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Coin")) return;

        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.AddKill(); // increment kill counter
        }

        Destroy(gameObject);
    }
}