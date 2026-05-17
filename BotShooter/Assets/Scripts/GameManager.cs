using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
// ============================================================
// Student Number : 223051684
// Tracks coins, lives and kills, updates HUD, shows
// Win/GameOver screens. Stats text managed manually in Unity.
// ============================================================
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coins = 0;
    public int lives = 3;
    public int kills = 0;
    private int totalCoins;

    [Header("HUD")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI killText;

    [Header("Win Screen")]
    public GameObject winScreen;
    public TextMeshProUGUI winCoinsText;     // e.g. "Coins: 5 / 8"
    public TextMeshProUGUI winKillsText;     // e.g. "Kills: 3"

    [Header("Game Over Screen")]
    public GameObject gameOverScreen;
    public TextMeshProUGUI gameOverCoinsText;
    public TextMeshProUGUI gameOverKillsText;

    [Header("SFX")]
    public AudioClip coinSFX;
    public AudioClip loseLifeSFX;
    public AudioClip dieSFX;
    public AudioClip winSFX;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Start()
    {
        totalCoins = FindObjectsOfType<CoinScript>().Length;
        Time.timeScale = 1f;
        UpdateUI();
        ZoneLightingController.Instance.TransitionToZone(ZoneTrigger.ZoneType.Forest);
    }

    public void AddCoin()
    {
        coins++;
        UpdateUI();
        PlaySFX(coinSFX);
        if (coins >= totalCoins)
            ShowWin();
    }

    public void AddKill()
    {
        kills++;
        UpdateUI();
    }

    public void LoseLife()
    {
        lives--;
        UpdateUI();

        CameraFollow cf = Camera.main.GetComponent<CameraFollow>();
        if (cf != null) cf.TriggerShake();

        if (lives <= 0)
        {
            PlaySFX(dieSFX);
            ShowGameOver();
        }
        else
        {
            PlaySFX(loseLifeSFX);
        }
    }

    void UpdateUI()
    {
        coinText.text = "Coins: " + coins;
        livesText.text = "Lives: " + lives;
        if (killText != null)
            killText.text = "Kills: " + kills;
    }

    void ShowWin()
    {
        PlaySFX(winSFX);

        // Update win screen stats if assigned
        if (winCoinsText != null)
            winCoinsText.text = "Coins: " + coins + " / " + totalCoins;
        if (winKillsText != null)
            winKillsText.text = "Kills: " + kills;

        winScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    void ShowGameOver()
    {
        // Update game over stats if assigned
        if (gameOverCoinsText != null)
            gameOverCoinsText.text = "Coins: " + coins + " / " + totalCoins;
        if (gameOverKillsText != null)
            gameOverKillsText.text = "Kills: " + kills;

        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void RestartGame()
    {
        Instance = null;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}