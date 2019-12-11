using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void OnDonConnected(Dot dot);

public interface IDotConnectionController
{
    event OnDonConnected DotConnected;
    void AddConnection(Dot target);
    void ClearConnections();
    List<Dot> GetConnections();
    bool IsLoopConnection();
    DotColorType GetConnectionType();
}

public class DotConnectionController : IDotConnectionController
{
    public event OnDonConnected DotConnected;
    
    private List<Dot> _activeConnections = new List<Dot>();
    private DotColorType _currConnType;

    private bool _isLoop = false;

    public void AddConnection(Dot target)
    {
        var isValid = false;

        if (_activeConnections.Count == 0)
        {
            _isLoop = false;
            _currConnType = target.GetDotType();
            isValid = true;
        }
        else if (_activeConnections[_activeConnections.Count - 1] == target ||
                 _activeConnections[Mathf.Clamp(_activeConnections.Count - 2, 0, int.MaxValue)] == target)
        {
            // Remove last connection if we passed through last point
            _activeConnections.RemoveAt(_activeConnections.Count - 1);
        }
        else
        {
            isValid = _activeConnections[_activeConnections.Count - 1].IsValidNeighbor(target);
        }

        if (isValid)
        {
            _activeConnections.Add(target);
            DotConnected?.Invoke(target);
        }
        _isLoop = _activeConnections.Count != _activeConnections.Distinct().Count();
    }

    public void ClearConnections()
    {
        if (_activeConnections != null && _activeConnections.Count > 0)
        {
            _activeConnections.Clear();
        }
    }

    public List<Dot> GetConnections()
    {
        return _activeConnections;
    }

    public DotColorType GetConnectionType()
    {
        return _currConnType;
    }

    public bool IsLoopConnection()
    {
        return _isLoop;
    }
}
