using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
/// <summary>
/// Base class for animated objects
/// </summary>
public abstract class ObjectAnimation : MonoBehaviour
{
    public abstract void Run();
    public abstract void Stop();
    public abstract void Clear();
}
/// <summary>
/// Class for dot object animation
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class DotAnimation : ObjectAnimation
{
    private SpriteRenderer _sprite;
    
    private readonly Vector3 maxScale = new Vector3(2f, 2f, 1f);
    private Sequence _animSequence;
    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _animSequence = DOTween.Sequence();
    }

    public override void Run()
    {
        Clear();
        
        _animSequence.Kill();
        
        _animSequence.Append(transform.DOScale(maxScale, GameData.DOT_ANIMATION_TIME));
        _animSequence.Append(_sprite.DOFade(0f, GameData.DOT_ANIMATION_TIME));
        _animSequence.onComplete += Clear;
    }

    public override void Stop()
    {
        //stop animation (not necessary, yet)
    }

    public override void Clear()
    {
        var color = _sprite.color;
        color.a = 1f;
        _sprite.color = color;
        transform.localScale = Vector3.one;
    }
}
