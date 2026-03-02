using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer _renderer;
    [SerializeField] private EdgeCollider2D _collider;

    private readonly List<Vector2> _colliderPointsLocal = new List<Vector2>();
    public bool IsDead { get; private set; } = false;

    void Awake()
    {
        Debug.Assert(_renderer != null, "[Line] LineRenderer not assigned!", this);
        Debug.Assert(_collider != null, "[Line] EdgeCollider2D not assigned!", this);

        _collider.isTrigger = true;
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("[Line] No Rigidbody2D found — adding one automatically.", this);
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
    }

    public void SetPosition(Vector2 worldPos)
    {
        if (IsDead)
        {
            Debug.Log("[Line] SetPosition called on dead line!");
            return;
        }
        if (!CanAppend(worldPos)) return;

        _renderer.positionCount++;
        _renderer.SetPosition(_renderer.positionCount - 1, worldPos);

        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        _colliderPointsLocal.Add(localPos);

        if (_colliderPointsLocal.Count >= 2)
            _collider.points = _colliderPointsLocal.ToArray();
    }

    private bool CanAppend(Vector2 worldPos)
    {
        if (_renderer.positionCount == 0) return true;
        Vector2 last = _renderer.GetPosition(_renderer.positionCount - 1);
        return Vector2.Distance(last, worldPos) > DrawManager.RESOLUTION;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsDead) return;
        if (other.CompareTag("Player")) return;

        Debug.Log($"[Line] Hit collider: {other.name}", this);

        ArcherBehavior archer = other.GetComponent<ArcherBehavior>();
        if (archer == null) archer = other.GetComponentInParent<ArcherBehavior>();

        if (archer != null)
        {
            Debug.Log("[Line] Killed archer -> clearing line", this);
            archer.KillFromLine();
            ClearAndDestroy();
        }
    }

    private void ClearAndDestroy()
{
    IsDead = true;
    _renderer.positionCount = 0;
    _renderer.enabled = false;
    gameObject.SetActive(false); // force hide everything
    _collider.enabled = false;
    Invoke(nameof(DestroyNow), 0.05f);
}

    private void DestroyNow()
    {
        Destroy(gameObject);
    }
}