using System.Collections;
using UnityEngine;

public class ArcherBehavior : MonoBehaviour
{
    [SerializeField] private float range = 5f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 8f;
    [SerializeField] private float shootDelay = 0.3f;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float initialAttackDelay = 0f;
    [SerializeField] private bool invertFacing = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private GameObject player;

    private float lastShotTime;
    private float inRangeStartTime = -1f;

    private Vector3 startPosition;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log($"[Archer] Start, player found: {player != null}");

        lastShotTime = Time.time - cooldown;
        inRangeStartTime = -1f;
        startPosition = transform.position;

        if (animator != null) animator.SetBool("IsShooting", false);
    }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
        }

        FacePlayer();

        if (!IsInRange())
        {
            inRangeStartTime = -1f;
            if (animator != null) animator.SetBool("IsShooting", false);
            return;
        }

        if (inRangeStartTime < 0f)
            inRangeStartTime = Time.time;

        bool initialDelayOver = Time.time >= inRangeStartTime + initialAttackDelay;
        if (initialDelayOver && CooldownOver())
        {
            lastShotTime = Time.time;
            if (animator != null) animator.SetBool("IsShooting", true);
            StartCoroutine(ShootArrow());
        }
    }

    private bool IsInRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    private bool CooldownOver()
    {
        return Time.time >= lastShotTime + cooldown;
    }

    private void FacePlayer()
    {
        if (spriteRenderer == null || player == null) return;

        bool shouldFlip = player.transform.position.x < transform.position.x;
        if (invertFacing) shouldFlip = !shouldFlip;

        spriteRenderer.flipX = shouldFlip;
    }

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
        if (deathEffect != null) deathEffect.PlayDeathEffect();
        else Destroy(gameObject);
    }

    private IEnumerator ShootArrow()
    {
        yield return new WaitForSeconds(shootDelay);

        if (animator != null) animator.SetBool("IsShooting", false);
        if (isDead) { Debug.Log("[Archer] isDead, bailing"); yield break; }

        if (arrowPrefab == null) { Debug.Log("[Archer] arrowPrefab is null!"); yield break; }
        if (player == null) { Debug.Log("[Archer] player is null!"); yield break; }
        if (!IsInRange()) { Debug.Log("[Archer] not in range, bailing"); yield break; }

        Vector3 spawnPosition = transform.position;
        Vector3 direction = (player.transform.position - spawnPosition).normalized;

        Debug.Log($"[Archer] spawnPos: {spawnPosition}, playerPos: {player.transform.position}, dir: {direction}");

        Quaternion arrowRotation = CalculateRotationFromDirection(direction);
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, arrowRotation);

        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
            arrowScript.SetDirection(direction, arrowSpeed);
        else
            Debug.Log("[Archer] Arrow script NOT found on prefab!");
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
        if (deathEffect != null) deathEffect.ResetEffect();

        transform.position = startPosition;

        inRangeStartTime = -1f;
        lastShotTime = Time.time - cooldown;

        if (animator != null) animator.SetBool("IsShooting", false);
    }
}