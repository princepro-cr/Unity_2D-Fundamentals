using UnityEngine;
using TMPro;
// ============================================================
// Student Number : 223051684
// Detects which platform the player is standing on
// and displays its name on the HUD
// ============================================================

public class PlatformLabel : MonoBehaviour
{
    public TextMeshProUGUI labelText; // drag your PlatformLabel UI text here

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player landed on something below them
        if (collision.gameObject.CompareTag("MossPlatform"))
        {
            ShowLabel("Mossy Ground");
        }
        else if (collision.gameObject.CompareTag("WoodPlatform"))
        {
            ShowLabel("Wooden Platform");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Hide label when player leaves the platform
        if (collision.gameObject.CompareTag("MossPlatform") ||
            collision.gameObject.CompareTag("WoodPlatform"))
        {
            HideLabel();
        }
    }

    void ShowLabel(string name)
    {
        labelText.text = name;
        labelText.gameObject.SetActive(true);
    }

    void HideLabel()
    {
        labelText.text = "";
        labelText.gameObject.SetActive(false);
    }
}