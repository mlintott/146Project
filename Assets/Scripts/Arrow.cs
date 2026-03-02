using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float collisionRadius = 0.5f;

    private Vector3 direction;
    private bool isInitialized = false;
    private GameObject player;
    private PlayerHealth playerHealth;
    private MoveScript playerMoveScript;

    void Start()
    {
        Destroy(gameObject, lifetime);
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log($"[Arrow] player found: {player != null}, playerHealth found: {playerHealth != null}");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerMoveScript = player.GetComponent<MoveScript>();
        }
    }

    void Update()
    {
        if (isInitialized)
        {
            transform.position += direction * speed * Time.deltaTime;
            CheckForPlayerCollision();
        }
    }

    private void CheckForPlayerCollision()
    {
        if (player == null || playerHealth == null) return;

        bool isColliding = false;

        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider != null && playerCollider.bounds.Contains(transform.position))
        {
            isColliding = true;
        }
        else
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= collisionRadius)
            {
                isColliding = true;
            }
        }

        if (isColliding)
        {
            Debug.Log("[Arrow] Hit player, dealing damage!");
            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 dir, float arrowSpeed)
    {
        direction = dir.normalized;
        speed = arrowSpeed;
        isInitialized = true;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}