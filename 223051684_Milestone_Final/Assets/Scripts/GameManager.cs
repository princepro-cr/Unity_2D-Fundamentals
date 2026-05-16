using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
// ============================================================
// Student Number : 223051684
// Tracks coins and lives, updates HUD, shows Win/GameOver screens
// ============================================================
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coins = 0;
    public int lives = 3;
    private int totalCoins;

    [Header("HUD")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI livesText;
    public GameObject winScreen;
    public GameObject gameOverScreen;

    [Header("SFX")]
    public AudioClip coinSFX;       // plays when a coin is collected
    public AudioClip loseLifeSFX;   // plays when the player loses a life
    public AudioClip dieSFX;        // plays on game over (0 lives left)
    public AudioClip winSFX;        // plays when all coins collected

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D — not positional
    }

    void Start()
    {
        totalCoins = FindObjectsOfType<CoinScript>().Length;
        Time.timeScale = 1f;
        UpdateUI();

        // Start forest music immediately
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

    public void LoseLife()
    {
        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            PlaySFX(dieSFX);      // death sound on final life
            ShowGameOver();
        }
        else
        {
            PlaySFX(loseLifeSFX); // hurt sound when still alive
        }
    }

    void UpdateUI()
    {
        coinText.text = "Coins: " + coins;
        livesText.text = "Lives: " + lives;
    }

    void ShowWin()
    {
        PlaySFX(winSFX);
        winScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    void ShowGameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    // Central SFX helper — null-safe so missing clips never crash
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