using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPool<T>
{
    private readonly Stack<T> pool = new Stack<T>();
    
    public int countActive { get; protected set; }
    public int countInactive { get { return pool.Count; } }
    public int count { get { return countActive + countInactive; } }

    protected void Init(uint initCount)
    {
        if (initCount > 0)
        {
            var objs = new T[initCount];
            for (int i = 0; i < initCount; i++)
            {
                objs[i] = Get();
            }
            for (int i = 0; i < initCount; i++)
            {
                Return(objs[i]);
            }
        }
    }

    public T[] Get(int count)
    {
        if (count <= 0)
        {
            throw new System.ArgumentOutOfRangeException("count");
        }
        var array = new T[count];
        for (int i = 0; i < count; i++)
        {
            array[i] = Get();
        }
        return array;
    }

    /// <summary>
    /// Same as <see cref="Get(int)"/> without allocating a new array
    /// </summary>
    public void Get(ref T[] array)
    {
        if (array == null)
        {
            throw new System.ArgumentNullException("array");
        }
        if (array.Length == 0)
        {
            throw new System.IndexOutOfRangeException("array.Length must be > 0");
        }
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Get();
        }
    }

    public T Get()
    {
        if (pool.Count == 0)
        {
            var newObj = CreateNew();
            ProcessNew(newObj);
            return newObj;
        }
        else
        {
            T obj = pool.Pop();
            OnGetFromPool(obj);
            return obj;
        }
    }

    protected virtual T CreateNew()
    {
        return System.Activator.CreateInstance<T>();
    }

    /// <summary>
    /// Used by inheriting classes to
    /// run code post object creation
    /// </summary>
    protected abstract void ProcessNew(T obj);
    protected abstract void OnGetFromPool(T obj);
    protected abstract void OnReturnToPool(T obj);

    public void Return(T obj)
    {
        if (pool.Count > 0 && ReferenceEquals(pool.Peek(), obj))
        {
            Debug.LogError("Trying to destroy object that is already released to pool.");
        }
        else
        {
            OnReturnToPool(obj);
            pool.Push(obj);
            countActive--;
        }
    }
    
}

public class Pool : AbstractPool<PoolObject>
{
    private PoolObject _prefabObj;
    private Transform _parentObj;

    public Pool(PoolObject prefabObj) : this(prefabObj, null, 0) { }
    public Pool(PoolObject prefabObj, uint initCount) : this(prefabObj, null, initCount) { }
    public Pool(PoolObject prefabObj, Transform parent) : this(prefabObj, parent, 0) { }
    public Pool(PoolObject prefabObj, Transform parent, uint initCount)
    {
        this._prefabObj = prefabObj;
        this._parentObj = parent == null ? new GameObject("Pool" + "_" + prefabObj.name).transform : parent;
        Init(initCount);
    }

    protected override void ProcessNew(PoolObject obj)
    {
        obj.Init(_parentObj);
    }

    protected override PoolObject CreateNew()
    {
        return Object.Instantiate(_prefabObj);
    }

    protected override void OnGetFromPool(PoolObject obj)
    {
        obj.Activate();
    }

    protected override void OnReturnToPool(PoolObject obj)
    {
        obj.Deactivate();
    }
}
