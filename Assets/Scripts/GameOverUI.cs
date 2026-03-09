using UnityEngine;
using UnityEngine.UI;
using TMPro; // Remove this line if you're using legacy UI Text instead of TextMeshPro

/// <summary>
/// Controls the Game Over screen canvas/panel.
///
/// SETUP:
/// 1. In your scene, create a Canvas (Screen Space — Overlay).
/// 2. Add a Panel child — this is your "Game Over Panel". Assign it below.
/// 3. Inside the panel add:
///      - A TextMeshProUGUI (or Text) element with "GAME OVER" (or any message).
///      - A Button for Restart  → hook its OnClick to GameOverUI.OnRestartClicked
///      - A Button for Quit     → hook its OnClick to GameOverUI.OnQuitClicked  (optional)
/// 4. Attach this script to the Canvas (or the Panel itself).
/// 5. Drag the GameOverUI component into the GameManager's Inspector slot.
///
/// The panel starts hidden and is shown by GameManager after the death delay.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("Panel")]
    [Tooltip("The root Panel GameObject to show/hide.")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Optional - Animated Text")]
    [Tooltip("Optional: assign a TMP label to display a custom message.")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private string gameOverMessage = "GAME OVER";

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton; // optional

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    void Awake()
    {
        // Make sure the panel is hidden at startup.
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Wire up buttons in code as a safety net (you can also do it in the Inspector).
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    // -------------------------------------------------------------------------
    // Public API — called by GameManager
    // -------------------------------------------------------------------------

    /// <summary>
    /// Reveals the Game Over panel. Called by GameManager after the death delay.
    /// </summary>
    public void Show()
    {
        if (messageText != null)
            messageText.text = gameOverMessage;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("GameOverUI: Game Over screen shown.");
    }

    /// <summary>
    /// Hides the Game Over panel (useful if you ever want a manual hide).
    /// </summary>
    public void Hide()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // -------------------------------------------------------------------------
    // Button callbacks — also hookable directly from the Inspector
    // -------------------------------------------------------------------------

    public void OnRestartClicked()
    {
        Hide();
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
        else
            Debug.LogError("GameOverUI: GameManager instance not found!");
    }

    public void OnQuitClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
    }
}