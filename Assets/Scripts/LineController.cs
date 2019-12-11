using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    //prefabs for lines
    public PoolObject linePrefab;
    //private fields
    private Pool _linePool;
    private List<LineRenderer> _activeLines = new List<LineRenderer>();

    private IDotConnectionController _dotDotConnectionController;

    void Awake()
    {
        _linePool = new Pool(linePrefab, null, GameData.MIN_OBJECT_COUNT_POOL);
    }

    void Start()
    {
        _dotDotConnectionController = GameController.Instance.dotConnectionController;
    }
    
    private void ReturnLine(LineRenderer line)
    {
        line.startColor = Color.clear;
        line.endColor = Color.clear;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.zero);
        _linePool.Return(line.gameObject.GetComponent<PoolObject>());
        _activeLines.Remove(line);
    }

    private void Update()
    {       
        var connections = _dotDotConnectionController.GetConnections();

        while (connections.Count > _activeLines.Count)
        {
            _activeLines.Add(_linePool.Get().GetComponent<LineRenderer>());
        }
        while (connections.Count < _activeLines.Count)
        {
            ReturnLine(_activeLines[0]);
        }

        if (connections.Count > 0)
        {
            DrawConnections(connections);
        }
        
        
    }

    private void DrawConnections(List<Dot> connections)
    {
        LineRenderer line = null;
        var currDotType = _dotDotConnectionController.GetConnectionType();
        var currDrawColor = GameData.DotColors[currDotType];


        for (var i = 0; i < connections.Count; i++)
        {
            line = _activeLines[i];
            line.startColor = currDrawColor;
            line.endColor = currDrawColor;
            line.SetPosition(0, connections[i].transform.position);

            // if itsn't last connection
            if (i != connections.Count - 1)
            {
                // set next position to next connection
                line.SetPosition(1, connections[i + 1].transform.position);
            }
        }
        var pointer = GetPointerWorldPosition();
        line.SetPosition(1, pointer);
    }

    private void ClearConnections()
    {
        while (_activeLines.Count > 0)
        {
            ReturnLine(_activeLines[0]);
        }
    }

    /// <summary>
    /// Get the current pointer position in world (z: 0)
    /// </summary>
    /// <returns>Mouse position or first touch position</returns>
    private Vector2 GetPointerWorldPosition()
    {
        var screen = Vector2.zero;
        if (Input.touchSupported)
        {
            screen = Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero;
        }
        else
        {
            screen = Input.mousePosition;
        }
        return Camera.main.ScreenToWorldPoint(screen);
    }
}
