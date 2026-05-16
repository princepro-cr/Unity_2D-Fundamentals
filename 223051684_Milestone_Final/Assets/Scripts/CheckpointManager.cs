// ============================================================
// Student Number : 223051684
// CheckpointManager.cs
// Saves the last zone the player reached and respawns them there
// ============================================================

using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Respawn Points — drag empty GameObjects here")]
    public Transform forestRespawn;
    public Transform savannaRespawn;
    public Transform spaceRespawn;

    private Transform currentRespawn;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Always start at forest
        currentRespawn = forestRespawn;
    }

    // Called by ZoneTrigger when player enters a zone
    public void SetCheckpoint(ZoneTrigger.ZoneType zone)
    {
        switch (zone)
        {
            case ZoneTrigger.ZoneType.Forest:
                currentRespawn = forestRespawn;
                break;
            case ZoneTrigger.ZoneType.Savanna:
                currentRespawn = savannaRespawn;
                break;
            case ZoneTrigger.ZoneType.Space:
                currentRespawn = spaceRespawn;
                break;
        }
        Debug.Log("Checkpoint set: " + zone);
    }

    public Vector3 GetRespawnPoint()
    {
        return currentRespawn.position;
    }
}