using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private string gameOverMessage  = "GAME OVER";
    [SerializeField] private string subtitleMessage  = "";

    [Header("Button")]
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI restartButtonText;
    [SerializeField] private Image restartButtonImage;

    [Header("Decorative")]
    [SerializeField] private Image dividerLine;
    [SerializeField] private Image sheikahEye;

    [Header("Timing")]
    [SerializeField] private float overlayFadeDuration = 1.2f;
    [SerializeField] private float textRevealDuration  = 0.5f;
    [SerializeField] private float buttonRevealDelay   = 0.4f;

    // ── Palette ───────────────────────────────────────────────────────────────
    static readonly Color Orange    = new Color(1.00f, 0.60f, 0.10f, 1f);
    static readonly Color Gold      = new Color(0.90f, 0.75f, 0.30f, 1f);
    static readonly Color BtnRed    = new Color(1.00f, 0.15f, 0.15f, 1f);
    static readonly Color WarmWhite = new Color(0.92f, 0.92f, 0.88f, 0.70f);
    static readonly Color PanelBG   = new Color(0.03f, 0.03f, 0.05f, 0.93f);

    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        ApplyStyle();
    }

    void ApplyStyle()
    {
        // Panel backdrop
        var panelImg = gameOverPanel != null ? gameOverPanel.GetComponent<Image>() : null;
        if (panelImg != null) panelImg.color = PanelBG;

        // "GAME OVER" — big, spaced, orange
        if (gameOverText != null)
        {
            gameOverText.color            = Orange;
            gameOverText.characterSpacing = 20f;
            gameOverText.fontStyle        = FontStyles.Bold;
        }

        // Subtitle — small, italic, warm white
        if (subtitleText != null)
        {
            subtitleText.color            = WarmWhite;
            subtitleText.characterSpacing = 5f;
            subtitleText.fontStyle        = FontStyles.Italic;
        }

        // Gold divider line
        if (dividerLine != null) dividerLine.color = Gold;

        // Sheikah eye
        if (sheikahEye != null) sheikahEye.color = Orange;

        // Button background
        if (restartButtonImage != null)
            restartButtonImage.color = new Color(BtnRed.r, BtnRed.g, BtnRed.b, 0.15f);

        // Button text — RED
        if (restartButtonText != null)
        {
            restartButtonText.color            = BtnRed;
            restartButtonText.characterSpacing = 12f;
            restartButtonText.fontStyle        = FontStyles.Bold;
        }
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void Show()
    {
        gameOverPanel.SetActive(true);

        if (panelCanvasGroup != null) panelCanvasGroup.alpha = 0f;
        SetTMPAlpha(gameOverText,  0f);
        SetTMPAlpha(subtitleText,  0f);
        SetImgAlpha(dividerLine,   0f);
        SetImgAlpha(sheikahEye,    0f);
        if (restartButton != null) restartButton.gameObject.SetActive(false);

        StartCoroutine(RevealSequence());
    }

    public void Hide()
    {
        StopAllCoroutines();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    // ── Reveal sequence ───────────────────────────────────────────────────────

    IEnumerator RevealSequence()
    {
        yield return StartCoroutine(FadeCG(panelCanvasGroup, 0f, 1f, overlayFadeDuration));

        if (sheikahEye != null)
            yield return StartCoroutine(FadeImg(sheikahEye, 0f, 1f, 0.35f));

        if (dividerLine != null)
            yield return StartCoroutine(FadeImg(dividerLine, 0f, 1f, 0.25f));

        if (gameOverText != null)
        {
            gameOverText.text = gameOverMessage;
            yield return StartCoroutine(FadeTMP(gameOverText, 0f, 1f, textRevealDuration));
        }

        yield return new WaitForSecondsRealtime(0.15f);
        if (subtitleText != null && !string.IsNullOrEmpty(subtitleMessage))
        {
            subtitleText.text = subtitleMessage;
            yield return StartCoroutine(FadeTMP(subtitleText, 0f, 0.7f, 0.4f));
        }

        yield return new WaitForSecondsRealtime(buttonRevealDelay);
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            StartCoroutine(PulseButton());
        }
    }

    IEnumerator PulseButton()
    {
        while (restartButton != null && restartButton.gameObject.activeInHierarchy)
        {
            float t = (Mathf.Sin(Time.unscaledTime * 2.2f) + 1f) * 0.5f;
            if (restartButtonText != null)
                restartButtonText.color = Color.Lerp(
                    new Color(BtnRed.r, BtnRed.g, BtnRed.b, 0.45f), BtnRed, t);
            yield return null;
        }
    }

    // ── Button callbacks ──────────────────────────────────────────────────────

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
        GameManager.Instance?.QuitGame();
    }

    // ── Fade helpers ──────────────────────────────────────────────────────────

    IEnumerator FadeCG(CanvasGroup cg, float from, float to, float dur)
    {
        if (cg == null) yield break;
        for (float e = 0; e < dur; e += Time.unscaledDeltaTime)
        {
            cg.alpha = Mathf.Lerp(from, to, e / dur);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator FadeTMP(TextMeshProUGUI t, float from, float to, float dur)
    {
        if (t == null) yield break;
        Color c = t.color;
        for (float e = 0; e < dur; e += Time.unscaledDeltaTime)
        {
            t.color = new Color(c.r, c.g, c.b, Mathf.Lerp(from, to, e / dur));
            yield return null;
        }
        t.color = new Color(c.r, c.g, c.b, to);
    }

    IEnumerator FadeImg(Image img, float from, float to, float dur)
    {
        if (img == null) yield break;
        Color c = img.color;
        for (float e = 0; e < dur; e += Time.unscaledDeltaTime)
        {
            img.color = new Color(c.r, c.g, c.b, Mathf.Lerp(from, to, e / dur));
            yield return null;
        }
        img.color = new Color(c.r, c.g, c.b, to);
    }

    void SetTMPAlpha(TextMeshProUGUI t, float a)
    {
        if (t == null) return;
        Color c = t.color; t.color = new Color(c.r, c.g, c.b, a);
    }

    void SetImgAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color; img.color = new Color(c.r, c.g, c.b, a);
    }
}