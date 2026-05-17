using UnityEngine;

public class NavigationArrow : MonoBehaviour
{
    public Transform target;        // drag your coin/goal here
    public float hideDistance = 1.5f;
    public float orbitRadius = 80f; // pixels from player center

    void Update()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.parent.position;
        float dist = dir.magnitude;

        // Hide when you're close enough
        gameObject.SetActive(dist > hideDistance);

        // Rotate arrow to face the target
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Orbit around the player
        transform.localPosition = dir.normalized * orbitRadius;
    }
}