using System.Collections.Generic;
using UnityEngine;

public class DrawManager : MonoBehaviour
{
    private Camera _cam;
    [SerializeField] private Line _linePrefab;
    public const float RESOLUTION = .1f;
    private Line _currentLine;
    private Vector2 _startPos;
    private bool _isDragging;
    [SerializeField] public const float MAXDISTANCE = 5f;

    void Start()
    {
        _cam = Camera.main;
    }

    void Update()
    {
        Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            _startPos = mousePos;
            _isDragging = true;
        }

        if (Input.GetMouseButton(0) && _isDragging)
        {
            float distanceNextPos = Vector2.Distance(_startPos, mousePos);
            float distanceToPlayer = Vector2.Distance(_startPos, transform.position);

            if (_currentLine == null && distanceNextPos > RESOLUTION && distanceToPlayer <= MAXDISTANCE)
            {
                _currentLine = Instantiate(_linePrefab, _startPos, Quaternion.identity);
                _currentLine.SetPosition(_startPos);
            }

            if (_currentLine != null)
            {
                if (_currentLine.IsDead)
                {
                    _currentLine = null;
                    _isDragging = false; // stop drawing when line kills enemy
                }
                else
                {
                    _currentLine.SetPosition(mousePos);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _currentLine = null;
            _isDragging = false;
        }
    }
}