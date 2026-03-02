using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer _renderer;
    [SerializeField] private EdgeCollider2D _collider;

    private readonly List<Vector2> _colliderPointsLocal = new List<Vector2>();

    void Awake()
    {
        // Trigger so we get OnTriggerEnter2D
        _collider.isTrigger = true;

        // Ensure trigger events fire reliably
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
    }

    public void SetPosition(Vector2 worldPos)
    {
        if (!CanAppend(worldPos)) return;

        // LineRenderer uses world positions
        _renderer.positionCount++;
        _renderer.SetPosition(_renderer.positionCount - 1, worldPos);

        // EdgeCollider2D uses local positions
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        _colliderPointsLocal.Add(localPos);
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
        // Works whether the collider is on the root or a child
        ArcherBehavior archer = other.GetComponent<ArcherBehavior>();
        if (archer == null) archer = other.GetComponentInParent<ArcherBehavior>();

        if (archer != null)
        {
            archer.KillFromLine();
        }
    }
}