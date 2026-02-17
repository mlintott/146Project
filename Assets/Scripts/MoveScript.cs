using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    private float holdStartTime;
    private bool isHolding = false;
    public float holdDurationThreshold = 0.2f; // Time in seconds to qualify as a "hold"
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Only true on the first frame of the press
        {
            
            holdStartTime = Time.time;
            isHolding = true;
            StartCoroutine(CheckHold());
        }
        else if (Input.GetMouseButtonUp(0)) // Only true on the first frame of the release
        {
            if ( isHolding && Time.time - holdStartTime < holdDurationThreshold)
            {
                // Action for a short click (tap)

                Vector3 mousePosition = Input.mousePosition;
                
                // Convert the screen coordinates to world coordinates
                // The Z value determines how far from the camera the world point will be.
                // For a 2D game using an Orthographic camera, a value like 10 (or your camera's
                // Z position) often works, but the ScreenToWorldPoint method handles Z appropriately 
                // when the transform.position is later set to a Vector2 implicitly or explicitly.
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                
                // Ensure the player stays on the same Z-plane in a 2D environment
                // This is crucial to prevent the object from moving out of the camera's view.
                mousePosition.z = transform.position.z;

                // Set the player's position to the new mouse position
                transform.position = mousePosition;
                Debug.Log("Short Click Action Executed");
            }
            isHolding = false; // Stop the hold check
            StopCoroutine(CheckHold());
        }
    }

    private IEnumerator CheckHold()
    {
        // Wait for the specified threshold time
        yield return new WaitForSeconds(holdDurationThreshold);

        // If the button is still held after the time, it's a long press
        if (isHolding)
        {
            // Action for a long press (hold)
            Debug.Log("Long Hold Action Executed");
        }
    }

}
