using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Main Menu for "The Art of Magic" — bright, whimsical, sparkly.
///
/// UNITY SETUP:
/// 1. Create a NEW SCENE called "MainMenu" and add it to Build Settings
///    (File → Build Settings → Add Open Scenes). Put it at index 0.
///    Your gameplay scene should be index 1.
///
/// 2. Build this hierarchy in the new scene:
///   Canvas (Screen Space - Overlay)
///   └── MenuPanel                  (Image — can be transparent, bg handled by camera)
///       ├── TitleText              (TMP — "The Art of Magic")
///       ├── SubtitleText           (TMP — small tagline)
///       ├── StartButton            (Button)
///       │   └── StartButtonText    (TMP — "START")
///       └── SparkleParent          (Empty GameObject — holds sparkle images)
///           ├── Sparkle1           (Image — star/diamond sprite, white)
///           ├── Sparkle2           (Image — star/diamond sprite, white)
///           └── Sparkle3           (Image — star/diamond sprite, white)
///
/// 3. Set your Camera background color to a deep indigo/violet:
///    R:30 G:18 B:60  (hex #1E1239)
///
/// 4. Attach this script to the Canvas and assign all references below.
/// </summary>
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

    [Header("Sparkles (optional — assign any small star/diamond images)")]
    [SerializeField] private RectTransform[] sparkles;

    [Header("Scene")]
    [Tooltip("Build index of your gameplay scene.")]
    [SerializeField] private int gameSceneIndex = 1;

    [Header("Timing")]
    [SerializeField] private float titleRevealDuration = 0.8f;
    [SerializeField] private float subtitleRevealDelay = 0.4f;
    [SerializeField] private float buttonRevealDelay   = 0.7f;

    // ── Whimsical palette ─────────────────────────────────────────────────────
    static readonly Color MagentaPink  = new Color(1.00f, 0.35f, 0.75f, 1f); // #FF59BF
    static readonly Color SkyBlue      = new Color(0.35f, 0.85f, 1.00f, 1f); // #59D9FF
    static readonly Color SunYellow    = new Color(1.00f, 0.90f, 0.30f, 1f); // #FFE64D
    static readonly Color MintGreen    = new Color(0.40f, 1.00f, 0.75f, 1f); // #66FFBF
    static readonly Color PureWhite    = new Color(1.00f, 1.00f, 1.00f, 1f);
    static readonly Color ButtonBG     = new Color(1.00f, 0.35f, 0.75f, 0.25f);

    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        ApplyStyle();

        // Hide everything — reveal in sequence
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

    // ── Styling ───────────────────────────────────────────────────────────────

    void ApplyStyle()
    {
        // Title — big, bold, rainbow-gradient feel via pink
        if (titleText != null)
        {
            titleText.text            = "The Art of Magic";
            titleText.color           = MagentaPink;
            titleText.characterSpacing = 5f;
            titleText.fontStyle       = FontStyles.Bold;
        }

        // Subtitle — sky blue, small, italic
        if (subtitleText != null)
        {
            subtitleText.text      = subtitle;
            subtitleText.color     = new Color(SkyBlue.r, SkyBlue.g, SkyBlue.b, 0.85f);
            subtitleText.fontStyle = FontStyles.Italic;
        }

        // Button — pink glow background, white text
        if (startButtonImage != null)
            startButtonImage.color = ButtonBG;

        if (startButtonText != null)
        {
            startButtonText.text            = "START";
            startButtonText.color           = PureWhite;
            startButtonText.characterSpacing = 14f;
            startButtonText.fontStyle       = FontStyles.Bold;
        }

        // Sparkles — cycle through fun colors
        Color[] sparkleColors = { SunYellow, MintGreen, SkyBlue, MagentaPink, PureWhite };
        if (sparkles != null)
        {
            for (int i = 0; i < sparkles.Length; i++)
            {
                var img = sparkles[i].GetComponent<Image>();
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
        Vector3 startScale = Vector3.one * 0.5f;
        Vector3 endScale   = Vector3.one;
        t.transform.localScale = startScale;

        for (float e = 0; e < dur; e += Time.deltaTime)
        {
            float progress = e / dur;
            // Overshoot bounce
            float scale = Mathf.LerpUnclamped(0.5f, 1f,
                EaseOutBack(progress));
            t.transform.localScale = Vector3.one * scale;
            SetTMPAlpha(t, Mathf.Clamp01(progress * 2f));
            yield return null;
        }

        t.transform.localScale = endScale;
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
        // Gentle scale + color pulse to invite the player to press
        while (startButton != null && startButton.gameObject.activeInHierarchy)
        {
            float t = (Mathf.Sin(Time.time * 2.5f) + 1f) * 0.5f;

            // Pulse scale slightly
            float scale = Mathf.Lerp(0.97f, 1.03f, t);
            startButton.transform.localScale = Vector3.one * scale;

            // Pulse text color between white and yellow
            if (startButtonText != null)
                startButtonText.color = Color.Lerp(PureWhite, SunYellow, t);

            yield return null;
        }
    }

    IEnumerator AnimateSparkles()
    {
        if (sparkles == null) yield break;

        // Pop each sparkle in with a little spin
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

        // Fade + spin in
        float dur = 0.4f;
        for (float e = 0; e < dur; e += Time.deltaTime)
        {
            float t = e / dur;
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, t);
            s.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 180f, t));
            yield return null;
        }

        // Continuously twinkle (scale pulse)
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
        // Quick flash/fade out before loading
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