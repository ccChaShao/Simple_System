using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LRUObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("对象池配置")] 
    public T prefab; // 预制体资源
    public int initialCapacity = 10; // 初始化容量
    public int maxCapacity = 50; // 最大容量（空闲 + 使用中）
    public float cleanupInterval = 30.0f; // 清理间隔（秒）

    [Header("LRU配置")] 
    public int lruThreshold = 20; // LRU清理阈值
    public bool autoClenup = true; // 自动清理开关
    
    // 对象池集合
    private Queue<T> availableObjects = new(); // 用于记录空闲的对象
    // lru算法核心：（对象+使用时间） + （记录最旧对象）
    private Dictionary<T, DateTime> usedObjects = new(); // 用于记录使用中的对象
    private LinkedList<T> lruLinkedList = new(); // 用于记录lru信息

    private float lastCleanupTime = 0f;
    private Transform poolContainer;     // 对象容器

    private void Awake()
    {
        poolContainer = new GameObject($"{typeof(T).Name}_pool_container").transform;
        poolContainer.SetParent(transform);

        InitializePool();
    }

    private void Update()
    {
        if (autoClenup && Time.time - lastCleanupTime > cleanupInterval)
        {
            TryCleanupLRUObjects();
            lastCleanupTime = Time.time;
        }
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < initialCapacity; i++)
        {
            CreatePoolObject();
        }
    }

    /// <summary>
    /// 创建对象
    /// </summary>
    private T CreatePoolObject()
    {
        if (prefab == null)
        {
            Debug.LogError("[CreateNewObject] : prefab is null ?");
            return null;
        }
        
        T obj = Instantiate(prefab, poolContainer);
        obj.gameObject.SetActive(false);
        obj.gameObject.name = $"{typeof(T).Name}_pool_object";

        availableObjects.Enqueue(obj);
        
        return obj;
    }

    /// <summary>
    /// 销毁对象
    /// </summary>
    private void DestroyPoolObject(T obj)
    {
        if (obj == null)
        {
            return;
        }

        // 清理使用中集合
        usedObjects.Remove(obj);
        
        // 从可用队列中移除
        Queue<T> newQueue = new();
        while (availableObjects.Count > 0)
        {
            T deqObj = availableObjects.Dequeue();
            if (deqObj != obj)
            {
                newQueue.Enqueue(deqObj);
            }
        }
        availableObjects = newQueue;
        
        // 从LRU列表中移除
        RemoveFromLRUList(obj);
        
        // 执行销毁
        Destroy(obj);
    }

    /// <summary>
    /// 获取一个对象池对象
    /// </summary>
    private T GetPoolObject()
    {
        T obj = null;

        // 优先用空闲的；
        if (availableObjects.Count > 0)
        {
            obj = availableObjects.Dequeue();
        }
        // 其次创建对象；
        else if (usedObjects.Count < maxCapacity)
        {
            obj = CreatePoolObject();
        }

        if (obj != null)
        {
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(null);
            // 使用记录
            usedObjects[obj] = DateTime.Now;
            // LRU更新
            SetLastInLRUList(obj);
        }

        return obj;
    }

    /// <summary>
    /// 释放一个对象池对象
    /// </summary>
    private void ReleasePoolObject(T obj)
    {
        if (obj == null)
        {
            return;
        }
        
        // 禁用对象
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(poolContainer);
        
        // 从使用字典中移除
        usedObjects.Remove(obj);
        
        // 从LRU列表中移除
        RemoveFromLRUList(obj);
        
        // 放入空闲队列
        availableObjects.Enqueue(obj);
    }

    /// <summary>
    /// 获取LRU对象（清理太久不使用的部分对象）
    /// </summary>
    private T GetLRUObject()
    {
        if (lruLinkedList.Count <= 0)
        {
            return null;
        }

        T lruObject = lruLinkedList.First.Value;
        
        // 从当前位置清理
        lruLinkedList.RemoveFirst();

        return lruObject;
    }
    
    /// <summary>
    /// 更新LRU列表
    /// </summary>
    private void SetLastInLRUList(T obj)
    {
        // 移除旧位置
        RemoveFromLRUList(obj);
        
        // 添加到末尾（最近使用）
        lruLinkedList.AddLast(obj);
    }
    
    /// <summary>
    /// 从LRU列表中移除
    /// </summary>
    private void RemoveFromLRUList(T obj)
    {
        var node = lruLinkedList.Find(obj);
        
        if (node != null)
        {
            lruLinkedList.Remove(node);
        }
    }

    /// <summary>
    /// 尝试清理LRU对象
    /// </summary>
    private void TryCleanupLRUObjects()
    {
        // 还没到清理阈值
        if (usedObjects.Count <= lruThreshold)
        {
            return;
        }
        
        int removeCount = usedObjects.Count - lruThreshold;
        List<T> toRemove = new ();
        
        // 获取最久没有使用（前面几个）的对象
        LinkedListNode<T> currentNode = lruLinkedList.First;
        for (int i = 0; i < removeCount && currentNode != null; i++)
        {
            toRemove.Add(currentNode.Value);
            currentNode = currentNode.Next;
        }

        foreach (var t in toRemove)
        {
            DestroyPoolObject(t);
        }
    }
}
