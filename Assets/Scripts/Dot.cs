using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// This class is describing dot logic
/// </summary>
[RequireComponent(typeof(DotIOHandler))]
public class Dot : MonoBehaviour, IComparable<Dot>
{
    [SerializeField] 
    private SpriteRenderer[] sprites;
        
    private DotGridCoords _coords;
    private DotColorType _dotType;

    public DotIOHandler dotIO { get; private set; }
    public ObjectAnimation dotAnimation { get; private set; }

    private bool _isAnimationPlaying;

    private void Awake()
    {
        dotIO = GetComponent<DotIOHandler>();
        dotAnimation = GetComponentInChildren<ObjectAnimation>();
    }
    
    #region get-set methods
    public DotGridCoords GetCoords()
    {
        return _coords;
    }

    public void SetCoords(DotGridCoords coords)
    {
        _coords = coords;
    }

    public DotColorType GetDotType()
    {
        return _dotType;
    }

    private void SetDotType(DotColorType dotType)
    {
        _dotType = dotType;
        if (_dotType != DotColorType.Clear)
        {
            //sets color to all sprites in dot
            foreach (var s in sprites)
            {
                s.color = GameData.DotColors[dotType];
            }
        }
    }
    #endregion

    public void ClearDotType()
    {
        _isAnimationPlaying = true;
        _dotType = DotColorType.Clear;
        transform.DOScale(Vector3.zero, 0.2f).onComplete += () =>
        {
            _isAnimationPlaying = false;
        };
    }
    
    public void Spawn(Vector2 targetPosition, float delay)
    {
        StartCoroutine(DoSpawn(targetPosition, delay));
    }
    /// <summary>
    /// Spawn method, it's a coroutine, wait for tween process ends 
    /// </summary>
    /// <param name="targetPosition">Dot begin position</param>
    /// <param name="delay">Delay to dot's falling animation</param>
    /// <returns></returns>
    private IEnumerator DoSpawn(Vector2 targetPosition, float delay)
    {
        while (_isAnimationPlaying)
        {
            yield return null;
        }
        var types = Enum.GetValues(typeof(DotColorType));
        var newType = (DotColorType)UnityEngine.Random.Range(1, types.Length);
        
        SetDotType(newType);
        
        transform.localScale = Vector3.one;
        MoveToPosition(targetPosition, delay);
        
        targetPosition.y = GameData.BEGIN_DOT_POS_Y;
        transform.localPosition = targetPosition;
        
        foreach (Transform child in transform)
        {
            child.transform.localScale = Vector3.one;
        }
    }
    /// <summary>
    /// Tween method for dot motion 
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="delay"></param>
    public void MoveToPosition(Vector2 targetPosition, float delay = 0f)
    {
        _isAnimationPlaying = true;

        var moveTweener = transform.DOMove(targetPosition, 0.4f);
        moveTweener.SetEase(Ease.OutBounce).SetDelay((GameData.ANIMATION_DELAY * _coords.xPos) + delay);
        moveTweener.onComplete += () =>
        {
            _isAnimationPlaying = false;
        };
    }
    /// <summary>
    /// Check neighbor of dot
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsValidNeighbor(Dot other)
    {
        if (this == other || other.GetDotType() != _dotType)
        {
            return false;
        }
        else
        {
            var otherCoords = other.GetCoords();
            
            var rowDiff = Mathf.Abs(otherCoords.xPos - _coords.xPos);
            var columnDiff = Mathf.Abs(otherCoords.yPos - _coords.yPos);

            // return if neighbor is diagonal
            if (rowDiff > 0 && columnDiff > 0)
            {
                return false;
            }
            // return if neighbor itsn't neighbor
            if (rowDiff > 1 || columnDiff > 1)
            {
                return false;
            }
            return true;
        }
    }

    
    public int CompareTo(Dot other)
    {
        var isSameRow = _coords.xPos.CompareTo(other._coords.xPos);
        if (isSameRow != 0)
        {
            return isSameRow;
        }

        return _coords.yPos.CompareTo(other._coords.yPos);
    }
}

/// <summary>
/// struct for dot coordinates in grid 
/// </summary>
public struct DotGridCoords
{
    public byte xPos, yPos;

    public DotGridCoords(byte x = 0, byte y = 0)
    {
        xPos = x;
        yPos = y;
    }
}

/// <summary>
/// Dot color types
/// </summary>
public enum DotColorType
{
    Clear,
    Red,
    Green,
    Blue,
    Cyan,
    Yellow,
    Magenta
}
