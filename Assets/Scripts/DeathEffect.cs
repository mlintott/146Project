using UnityEngine;
using System.Collections;

/// <summary>
/// Plays a "poof" death effect on the Player — scales up and fades out,
/// then destroys (or hides) the GameObject.
///
/// SETUP:
/// 1. Add this component to your Player GameObject.
/// 2. Your Player's SpriteRenderer will be used automatically.
///    If your visuals are on a child object, assign it manually in the Inspector.
/// 3. Tune the feel with the fields below — no other setup needed.
/// </summary>
public class DeathEffect : MonoBehaviour
{
    [Header("Poof Settings")]
    [Tooltip("How long the poof animation takes in seconds.")]
    [SerializeField] private float duration = 0.6f;

    [Tooltip("How much the sprite scales up during the poof (1 = no scale, 2 = doubles in size).")]
    [SerializeField] private float scaleMultiplier = 2.0f;

    [Tooltip("If true, destroys the GameObject after the effect. If false, just hides it (safer for reset).")]
    [SerializeField] private bool destroyAfter = false;

    [Header("Optional")]
    [Tooltip("Leave empty to auto-find on this GameObject or its children.")]
    [SerializeField] private SpriteRenderer targetRenderer;

    private Vector3 originalScale;

    void Awake()
    {
        // Auto-find the SpriteRenderer if not manually assigned.
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>();

        if (targetRenderer == null)
            Debug.LogWarning("DeathEffect: No SpriteRenderer found on " + gameObject.name);

        originalScale = transform.localScale;
    }

    /// <summary>
    /// Called by PlayerHealth.OnPlayerDeath() — kicks off the poof coroutine.
    /// </summary>
    public void PlayDeathEffect()
    {
        StartCoroutine(PoofRoutine());
    }

    private IEnumerator PoofRoutine()
    {
        if (targetRenderer == null) yield break;

        float elapsed = 0f;
        Vector3 targetScale = originalScale * scaleMultiplier;
        Color startColor = targetRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Scale up
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            // Fade out
            targetRenderer.color = Color.Lerp(startColor, endColor, t);

            elapsed += Time.unscaledDeltaTime; // Works even when timeScale = 0
            yield return null;
        }

        // Snap to final state
        transform.localScale = targetScale;
        targetRenderer.color = endColor;

        if (destroyAfter)
            Destroy(gameObject);
        else
            gameObject.SetActive(false); // Hidden but still resettable
    }

    /// <summary>
    /// Resets the player's visual state — call this on restart before re-enabling.
    /// </summary>
    public void ResetEffect()
    {
        transform.localScale = originalScale;

        if (targetRenderer != null)
        {
            Color c = targetRenderer.color;
            targetRenderer.color = new Color(c.r, c.g, c.b, 1f);
        }

        gameObject.SetActive(true);
    }
}