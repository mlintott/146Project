using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxLives = 3;

    [Header("Death Timing")]
    [Tooltip("How long to wait after death before firing PlayerDied (should match your DeathEffect duration).")]
    [SerializeField] private float deathEffectDuration = 0.6f;

    private int currentLives;
    private bool isInvulnerable = false;

    // Event for when lives change (useful for UI updates)
    public static Action<int> LivesChanged;

    // Event for when player dies — fired AFTER the death effect finishes
    public static Action PlayerDied;

    // Public properties
    public int CurrentLives => currentLives;
    public int MaxLives => maxLives;
    public bool IsDead => currentLives <= 0;

    void Start()
    {
        currentLives = maxLives;
        NotifyLivesChanged();
    }

    /// <summary>
    /// Makes the player take damage, reducing lives by the specified amount.
    /// </summary>
    public bool TakeDamage(int damage = 1)
    {
        if (IsDead) return true;
        if (isInvulnerable) return false;

        currentLives = Mathf.Max(0, currentLives - damage);
        NotifyLivesChanged();
        Debug.Log($"Player took {damage} damage! Lives remaining: {currentLives}");

        if (IsDead)
        {
            OnPlayerDeath();
            return true;
        }

        return false;
    }

    private void OnPlayerDeath()
    {
        Debug.Log("Player died! Playing death effect...");

        // Disable movement and drawing immediately
        MoveScript moveScript = GetComponent<MoveScript>();
        if (moveScript != null) moveScript.enabled = false;

        DrawManager drawManager = GetComponent<DrawManager>();
        if (drawManager != null) drawManager.enabled = false;

        // Play the poof, then fire PlayerDied after it finishes
        DeathEffect deathEffect = GetComponent<DeathEffect>();
        if (deathEffect != null)
        {
            deathEffect.PlayDeathEffect();
            StartCoroutine(FirePlayerDiedAfterEffect(deathEffectDuration));
        }
        else
        {
            // No DeathEffect component — fire immediately
            FirePlayerDied();
        }
    }

    private IEnumerator FirePlayerDiedAfterEffect(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Unscaled so it works if time gets frozen
        FirePlayerDied();
    }

    private void FirePlayerDied()
    {
        Debug.Log("Player died! Firing PlayerDied event.");
        PlayerDied?.Invoke();
    }

    // -------------------------------------------------------------------------
    // Health utilities
    // -------------------------------------------------------------------------

    public void RestoreFullHealth()
    {
        currentLives = maxLives;
        NotifyLivesChanged();
    }

    public void AddLives(int amount)
    {
        currentLives = Mathf.Min(maxLives, currentLives + amount);
        NotifyLivesChanged();
    }

    public void ResetHealth()
    {
        isInvulnerable = false;
        RestoreFullHealth();
    }

    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
        Debug.Log($"Player invulnerability set to: {invulnerable}");
    }

    private void NotifyLivesChanged()
    {
        LivesChanged?.Invoke(currentLives);
    }
}