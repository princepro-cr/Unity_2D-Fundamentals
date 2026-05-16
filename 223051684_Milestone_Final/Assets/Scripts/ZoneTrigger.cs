using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public enum ZoneType { Forest, Savanna, Space }

    [Header("Which zone is this?")]
    public ZoneType zoneType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Change lighting
        ZoneLightingController.Instance.TransitionToZone(zoneType);

        // Save checkpoint
        CheckpointManager.Instance.SetCheckpoint(zoneType);
    }
}