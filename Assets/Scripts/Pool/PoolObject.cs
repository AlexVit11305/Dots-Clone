using UnityEngine;

public abstract class PoolObject : MonoBehaviour
{
    public abstract void Init(Transform parent);
    public abstract void Activate();
    public abstract void Deactivate();

    public GameObject GetObject()
    {
        return this.gameObject;
    }
}
