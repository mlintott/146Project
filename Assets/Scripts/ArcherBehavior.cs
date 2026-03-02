// ArcherBehavior.cs
// This file was initially generated with Cursor and edited for use in this exercise.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBehavior : MonoBehaviour
{
    [SerializeField] private float range = 5f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 8f;
    [SerializeField] private float shootDelay = 0.3f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private GameObject player;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float initialAttackDelay = 10f;
    private float lastShotTime;
    private float inRangeStartTime = -1f;

    private Vector3 startPosition;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        lastShotTime = Time.time - cooldown;
        inRangeStartTime = -1f;
        startPosition = transform.position;
    }

    private bool isInRange()
    {
        float distance = calculateDistanceToPlayer();
        return distance <= range;
    }

    private bool cooldownOver()
    {
        return Time.time >= lastShotTime + cooldown;
    }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
        }

        facePlayer();

        bool inRange = isInRange();
        if (inRange)
        {
            if (inRangeStartTime < 0f)
            {
                inRangeStartTime = Time.time;
            }

            bool initialDelayOver = Time.time >= inRangeStartTime + initialAttackDelay;
            if (initialDelayOver && cooldownOver())
            {
                animator.SetBool("IsShooting", true);
                lastShotTime = Time.time;
                StartCoroutine(ShootArrow());
            }
        }
        else
        {
            inRangeStartTime = -1f;
        }
    }

    private float calculateDistanceToPlayer()
    {
        if (player == null) return float.PositiveInfinity;
        return Vector3.Distance(transform.position, player.transform.position);
    }

    private void facePlayer()
    {
        if (spriteRenderer != null && player != null)
        {
            spriteRenderer.flipX = player.transform.position.x < transform.position.x;
        }
    }

    // Called by player melee (existing)
    public bool TryTakeDamage(Vector3 attackerPosition, float attackRange = 1.0f)
    {
        float distance = Vector3.Distance(transform.position, attackerPosition);
        if (distance <= attackRange)
        {
            if (isDead) return false;
            Die();
            return true;
        }
        return false;
    }

    // Option A: called by Line when it touches this archer
    public void KillFromLine()
    {
        if (isDead) return;
        Die();
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();

        DeathEffect deathEffect = GetComponent<DeathEffect>();
        if (deathEffect != null)
        {
            deathEffect.PlayDeathEffect();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ShootArrow()
    {
        yield return new WaitForSeconds(shootDelay);

        if (isDead) yield break;

        if (arrowPrefab != null && player != null && isInRange())
        {
            Vector3 spawnPosition = transform.position;
            Vector3 direction = (player.transform.position - spawnPosition).normalized;
            Quaternion arrowRotation = CalculateRotationFromDirection(direction);

            GameObject arrow = Instantiate(arrowPrefab, spawnPosition, arrowRotation);

            Arrow arrowScript = arrow.GetComponent<Arrow>();
            if (arrowScript != null)
            {
                arrowScript.SetDirection(direction, arrowSpeed);
            }
        }
    }

    private Quaternion CalculateRotationFromDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void ResetArcher()
    {
        isDead = false;
        StopAllCoroutines();

        DeathEffect deathEffect = GetComponent<DeathEffect>();
        if (deathEffect != null)
        {
            deathEffect.ReenableComponents();
        }

        transform.position = startPosition;

        inRangeStartTime = -1f;
        lastShotTime = Time.time - cooldown;
    }
}