using UnityEngine;

/// <summary>
/// class for line object
/// </summary>
public class LineObject : PoolObject
{
    public override void Init(Transform parent)
    {
        transform.parent = parent;
    }

    public override void Activate()
    {
        gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
