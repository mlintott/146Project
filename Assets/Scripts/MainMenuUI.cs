using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private string subtitle = "A magical adventure awaits...";

    [Header("Button")]
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private Image startButtonImage;

    [Header("Sparkles (optional)")]
    [SerializeField] private RectTransform[] sparkles;

    [Header("Scene")]
    [Tooltip("Build index of your gameplay scene.")]
    [SerializeField] private int gameSceneIndex = 1;

    [Header("Timing")]
    [SerializeField] private float titleRevealDuration = 0.8f;
    [SerializeField] private float subtitleRevealDelay = 0.4f;
    [SerializeField] private float buttonRevealDelay   = 0.7f;

    // ── Palette ───────────────────────────────────────────────────────────────
    static readonly Color MagentaPink  = new Color(1.00f, 0.35f, 0.75f, 1f);
    static readonly Color SkyBlue      = new Color(0.35f, 0.85f, 1.00f, 1f);
    static readonly Color SunYellow    = new Color(1.00f, 0.90f, 0.30f, 1f);
    static readonly Color MintGreen    = new Color(0.40f, 1.00f, 0.75f, 1f);
    static readonly Color PureWhite    = new Color(1.00f, 1.00f, 1.00f, 1f);
    static readonly Color DeepPurple   = new Color(0.25f, 0.05f, 0.45f, 1f); // #400073
    static readonly Color ButtonBG     = new Color(1.00f, 1.00f, 1.00f, 1f); // White button

    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        ApplyStyle();

        SetTMPAlpha(titleText,    0f);
        SetTMPAlpha(subtitleText, 0f);
        if (startButton != null) startButton.gameObject.SetActive(false);
        HideSparkles();

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
    }

    void Start()
    {
        StartCoroutine(RevealSequence());
    }

    void ApplyStyle()
    {
        // Title — big, bold, magenta pink
        if (titleText != null)
        {
            titleText.text             = "The Art of Magic";
            titleText.color            = MagentaPink;
            titleText.characterSpacing = 5f;
            titleText.fontStyle        = FontStyles.Bold;
        }

        // Subtitle — sky blue, small, italic
        if (subtitleText != null)
        {
            subtitleText.text      = subtitle;
            subtitleText.color     = new Color(SkyBlue.r, SkyBlue.g, SkyBlue.b, 0.85f);
            subtitleText.fontStyle = FontStyles.Italic;
        }

        // Button background — white
        if (startButtonImage != null)
            startButtonImage.color = ButtonBG;

        // Button text — deep purple (readable on white)
        if (startButtonText != null)
        {
            startButtonText.text             = "START";
            startButtonText.color            = DeepPurple;
            startButtonText.characterSpacing = 14f;
            startButtonText.fontStyle        = FontStyles.Bold;
        }

        // Sparkles — cycle through fun colors
        Color[] sparkleColors = { SunYellow, MintGreen, SkyBlue, MagentaPink, PureWhite };
        if (sparkles != null)
        {
            for (int i = 0; i < sparkles.Length; i++)
            {
                var img = sparkles[i] != null ? sparkles[i].GetComponent<Image>() : null;
                if (img != null)
                    img.color = sparkleColors[i % sparkleColors.Length];
            }
        }
    }

    // ── Reveal sequence ───────────────────────────────────────────────────────

    IEnumerator RevealSequence()
    {
        yield return new WaitForSeconds(0.3f);

        // 1 — Title bounces in
        if (titleText != null)
            yield return StartCoroutine(BounceInTitle(titleText, titleRevealDuration));

        // 2 — Subtitle fades in
        yield return new WaitForSeconds(subtitleRevealDelay);
        if (subtitleText != null)
            yield return StartCoroutine(FadeTMP(subtitleText, 0f, 0.85f, 0.5f));

        // 3 — Sparkles pop in
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(AnimateSparkles());

        // 4 — Start button pulses in
        yield return new WaitForSeconds(buttonRevealDelay);
        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);
            StartCoroutine(PulseButton());
        }
    }

    IEnumerator BounceInTitle(TextMeshProUGUI t, float dur)
    {
        t.transform.localScale = Vector3.one * 0.5f;
        for (float e = 0; e < dur; e += Time.deltaTime)
        {
            float progress = e / dur;
            float scale = Mathf.LerpUnclamped(0.5f, 1f, EaseOutBack(progress));
            t.transform.localScale = Vector3.one * scale;
            SetTMPAlpha(t, Mathf.Clamp01(progress * 2f));
            yield return null;
        }
        t.transform.localScale = Vector3.one;
        SetTMPAlpha(t, 1f);
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    IEnumerator PulseButton()
    {
        while (startButton != null && startButton.gameObject.activeInHierarchy)
        {
            float t = (Mathf.Sin(Time.time * 2.5f) + 1f) * 0.5f;
            float scale = Mathf.Lerp(0.97f, 1.03f, t);
            startButton.transform.localScale = Vector3.one * scale;

            // Pulse between deep purple and a slightly lighter purple
            if (startButtonText != null)
                startButtonText.color = Color.Lerp(
                    DeepPurple,
                    new Color(0.45f, 0.10f, 0.70f, 1f), t);

            yield return null;
        }
    }

    IEnumerator AnimateSparkles()
    {
        if (sparkles == null) yield break;
        foreach (var s in sparkles)
        {
            if (s == null) continue;
            StartCoroutine(SpinSparkle(s));
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SpinSparkle(RectTransform s)
    {
        var img = s.GetComponent<Image>();
        if (img == null) yield break;

        float dur = 0.4f;
        for (float e = 0; e < dur; e += Time.deltaTime)
        {
            float t = e / dur;
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, t);
            s.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 180f, t));
            yield return null;
        }

        float offset = Random.Range(0f, Mathf.PI * 2f);
        while (true)
        {
            float sc = 0.85f + 0.2f * Mathf.Sin(Time.time * 3f + offset);
            s.localScale = Vector3.one * sc;
            yield return null;
        }
    }

    void HideSparkles()
    {
        if (sparkles == null) return;
        foreach (var s in sparkles)
        {
            if (s == null) continue;
            var img = s.GetComponent<Image>();
            if (img != null) SetImgAlpha(img, 0f);
        }
    }

    // ── Button callback ───────────────────────────────────────────────────────

    void OnStartClicked()
    {
        StartCoroutine(TransitionToGame());
    }

    IEnumerator TransitionToGame()
    {
        if (startButtonText != null)
            startButtonText.text = "✨ Let's go! ✨";
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(gameSceneIndex);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    IEnumerator FadeTMP(TextMeshProUGUI t, float from, float to, float dur)
    {
        if (t == null) yield break;
        Color c = t.color;
        for (float e = 0; e < dur; e += Time.deltaTime)
        {
            t.color = new Color(c.r, c.g, c.b, Mathf.Lerp(from, to, e / dur));
            yield return null;
        }
        t.color = new Color(c.r, c.g, c.b, to);
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