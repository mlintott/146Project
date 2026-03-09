using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Manages overall game state. Listens for PlayerDied event and triggers Game Over.
/// Handles scene reset when the player chooses to restart.
/// 
/// SETUP:
/// 1. Create an empty GameObject called "GameManager" and attach this script.
/// 2. Assign the GameOverUI panel in the Inspector.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Settings")]
    [Tooltip("Delay (seconds) after death before the Game Over screen appears. " +
             "Give your DeathEffect time to finish playing.")]
    [SerializeField] private float gameOverDelay = 1.5f;

    private bool isGameOver = false;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    void Awake()
    {
        // Simple singleton — only one GameManager allowed.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        PlayerHealth.PlayerDied += OnPlayerDied;
    }

    void OnDisable()
    {
        PlayerHealth.PlayerDied -= OnPlayerDied;
    }

    // -------------------------------------------------------------------------
    // Game-state handlers
    // -------------------------------------------------------------------------

    private void OnPlayerDied()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("GameManager: Player died — starting Game Over sequence.");
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay); // Works with timeScale = 0
        Time.timeScale = 0f;
        ShowGameOver();
    }

    private void ShowGameOver()
    {
        Time.timeScale = 0f; // Freeze everything
        if (gameOverUI != null)
            gameOverUI.Show();
        else
            Debug.LogWarning("GameManager: No GameOverUI assigned!");
    }

    /// <summary>
    /// Call this from your Restart button. Reloads the active scene cleanly.
    /// </summary>
    public void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1f; // Safety — make sure time isn't frozen.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Call this from a Quit button.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}