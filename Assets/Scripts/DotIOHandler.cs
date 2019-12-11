using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// dot input-output control system
/// </summary>
[RequireComponent(typeof(Dot))]
public class DotIOHandler : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    private static bool _hasSelection;

    Dot selfDot;
    
    //delegates for events
    public delegate void OnSelectionStarted();
    public delegate void OnDotSelected(Dot dot);
    public delegate void OnSelectionEnded();
    
    //selection events
    public event OnSelectionStarted SelectionStarted;
    public event OnDotSelected DotSelected;
    public event OnSelectionEnded SelectionEnded;
    
    private void Awake()
    {
        selfDot = GetComponent<Dot>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_hasSelection)
        {
            _hasSelection = true;
            SelectionStarted?.Invoke();
            OnPointerEnter(eventData);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hasSelection)
        {
            DotSelected?.Invoke(selfDot);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (_hasSelection)
        {
            _hasSelection = false;
            SelectionEnded?.Invoke();
        }
    }
}
