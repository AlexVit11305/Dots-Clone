using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid for dots, fill, init, sign dots, remove dots from grid
/// </summary>
public class DotGrid : MonoBehaviour
{
    [SerializeField] 
    private GameObject dotPrefab;

    [SerializeField] 
    private byte rows = GameData.ROWS_COUNT;
    [SerializeField] 
    private byte columns = GameData.COLUMNS_COUNT;
    [SerializeField] 
    private float spacing = GameData.DOT_SPACING;
    [SerializeField]
    private float dotPPU = 100f;

    private List<Dot> dotsInGrid;
    
    public delegate void OnDotOperation(Dot dot);
    public event OnDotOperation DotSelected;
    
    private void Awake()
    {
        dotsInGrid = new List<Dot>();
        Fill();
    }

    // Start is called before the first frame update
    void Start()
    {
        ExecuteDotOperation((dot) =>
        {
            var targetPosition = GetPositionForCoordinates(dot.GetCoords());
            InitDot(dot);
            dot.Spawn(targetPosition, 0.5f);
        });
    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0))
            CheckSelectedDots();
    }
    /// <summary>
    /// Fill grid
    /// </summary>
    private void Fill()
    {
        // Need to manually loop for dot creation
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                CreateDot(new DotGridCoords(row, column));
            }
        }
    }
    /// <summary>
    /// Check dot in connections and remove it all have the same type
    /// </summary>
    private void CheckSelectedDots()
    {
        var currConnectionSystem = GameController.Instance.dotConnectionController;
        var connections = currConnectionSystem.GetConnections();
        //if no connection or exactly one dot
        if(connections == null)
            return;
        
        if(connections.Count < 2)
            currConnectionSystem.ClearConnections();
        
        var dotsNeedToRemovedInColumn = new byte[columns];

        // if connection is square, mark all dots of the same type
        if (currConnectionSystem.IsLoopConnection())
        {
            currConnectionSystem.ClearConnections();
            foreach (var dot in dotsInGrid)
            {
                if (dot.GetDotType() == currConnectionSystem.GetConnectionType())
                {
                    connections.Add(dot);
                }
            }
        }
        
        GameController.Instance.UpdateScore(connections.Count);
        
        // mark all dots in connection
        foreach (var dot in connections)
        {
            dotsNeedToRemovedInColumn[dot.GetCoords().yPos]++;
            dot.ClearDotType(); // Clear dot status
        }

        // update position in all dots of all columns (if needs)
        for (byte i = 0; i < columns; i++)
        {
            if (dotsNeedToRemovedInColumn[i] == 0)
            {
                continue;
            }
            ExecuteDotOperation(i, (dot) =>
            {
                if (dot.GetCoords().xPos != 0 && dot.GetDotType() != DotColorType.Clear)
                {
                    var fallDist = GetBlankDotsUnderneath(dot);
                    var newCoords = new DotGridCoords( (byte)(dot.GetCoords().xPos - fallDist), i);
                    dot.SetCoords(newCoords);
                    dot.MoveToPosition(GetPositionForCoordinates(dot.GetCoords()), 0f);
                }
            });
        }

        // for each column, recycle dots
        for (byte i = 0; i < columns; i++)
        {
            var removedCount = dotsNeedToRemovedInColumn[i];
            for (byte j = 0; j < removedCount; j++)
            {
                // The lowest empty row
                var row = (byte)(rows - (removedCount - j));
                var lastDotIndex = connections.Count - 1;
                var dot = connections[lastDotIndex];

                connections.RemoveAt(lastDotIndex);
                dot.SetCoords(new DotGridCoords(row, i));
                dot.Spawn(GetPositionForCoordinates(dot.GetCoords()), 0f);
            }
        }

        // clear connections
        currConnectionSystem.ClearConnections();
    }
    
    private void CreateDot(DotGridCoords coords)
    {
        var dot = Instantiate(dotPrefab).GetComponent<Dot>();
        dot.name = "dot_" + coords.xPos + "_" + coords.yPos;
        dot.transform.parent = transform;
        dot.SetCoords(coords);;
        dotsInGrid.Add(dot);
    }
    
    private void InitDot(Dot dot)
    {
        var connectionController = GameController.Instance.dotConnectionController;
        
        dot.dotIO.DotSelected += connectionController.AddConnection;
        
        connectionController.DotConnected += (target) =>
        {
            if(target == dot)
                dot.dotAnimation.Run();
        };
    }

    private byte GetBlankDotsUnderneath(Dot dot)
    {
        byte count = 0;
        ExecuteDotOperation(dot.GetCoords().yPos, (other) =>
        {
            if (other.GetDotType() == DotColorType.Clear && other.GetCoords().xPos < dot.GetCoords().xPos)
            {
                count++;
            }
        });
        return count;
    }
    private Vector2 GetPositionForCoordinates(DotGridCoords coords)
    {
        var adjustedDotSize = (GameData.DOT_SPRITE_SIZE / dotPPU) + spacing;
        var worldPosition = Vector2.zero;

        // Set to "zero position" (bottom left dot position)
        worldPosition.x = -adjustedDotSize * ((columns - 1) / 2f);
        worldPosition.y = -adjustedDotSize * ((rows - 1) / 2f);

        // Add offset from zero position via dot coordinate
        worldPosition.x += adjustedDotSize * coords.yPos;
        worldPosition.y += adjustedDotSize * coords.xPos;

        return worldPosition;
    }
    
    //methods for affect of all dots in grid
    private void ExecuteDotOperation(OnDotOperation callback)
    {
        if(callback == null)
            return;
        for (int i = 0; i < dotsInGrid.Count; i++)
        {
            callback.Invoke(dotsInGrid[i]);
        }
    }

    private void ExecuteDotOperation(byte column, OnDotOperation callback)
    {
        if(callback == null)
            return;
        for (int i = 0; i < dotsInGrid.Count; i++)
        {
            if(dotsInGrid[i].GetCoords().yPos == column)
                callback.Invoke(dotsInGrid[i]);
        }
    }
}


