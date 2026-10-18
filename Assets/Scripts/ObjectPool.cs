using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly Queue<T> available = new Queue<T>();
    private readonly T prefab;
    private readonly Transform parent;

    public ObjectPool(T prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
    }

    public T Get()
    {
        T item = available.Count > 0 ? available.Dequeue() : Object.Instantiate(prefab, parent);
        item.gameObject.SetActive(true);
        return item;
    }

    public void Release(T item)
    {
        item.gameObject.SetActive(false);
        available.Enqueue(item);
    }

    // Destroys every unused copy. Called before the assets behind them are released.
    public void Clear()
    {
        while (available.Count > 0)
        {
            T item = available.Dequeue();
            if (item != null) Object.Destroy(item.gameObject);
        }
    }
}