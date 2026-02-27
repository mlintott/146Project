using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    private float holdStartTime;
    private Vector2 startWorldPosition;

    private bool hasDragged = false;

    public float holdDurationThreshold = 0.2f;
    public float dragThreshold = 0.1f; // WORLD units (same scale as RESOLUTION)

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        Vector2 currentWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            holdStartTime = Time.time;
            startWorldPosition = currentWorldPos;
            hasDragged = false;
        }

        if (Input.GetMouseButton(0))
        {
            float dragDistance = Vector2.Distance(startWorldPosition, currentWorldPos);

            if (dragDistance > dragThreshold)
            {
                hasDragged = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            float heldTime = Time.time - holdStartTime;

            if (!hasDragged && heldTime < holdDurationThreshold)
            {
                MoveTo(currentWorldPos);
            }
        }
    }

    private void MoveTo(Vector2 worldPos)
    {
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        Debug.Log("Moved!");
    }
}
