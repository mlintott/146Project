using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class DrawManager : MonoBehaviour {
    private Camera _cam;
    [SerializeField] private Line _linePrefab;
 
    public const float RESOLUTION = .1f;
 
    private Line _currentLine;

    
    private Vector2 _startPos;
    private bool _isDragging;
    void Start()
    {
         _cam = Camera.main;   
    }
 
 
    void Update() {
        Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            _startPos = mousePos;
            _isDragging = true;
        }

        if (Input.GetMouseButton(0) && _isDragging)
        {
            float distance = Vector2.Distance(_startPos, mousePos);

            // Only create line once threshold is crossed
            if (_currentLine == null && distance > RESOLUTION)
            {
                _currentLine = Instantiate(_linePrefab, _startPos, Quaternion.identity);
                _currentLine.SetPosition(_startPos);
            }

            if (_currentLine != null)
            {
                _currentLine.SetPosition(mousePos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _currentLine = null;
            _isDragging = false;
        }
    }
}