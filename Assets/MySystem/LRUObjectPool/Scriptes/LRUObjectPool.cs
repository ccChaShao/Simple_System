using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRUObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("对象池配置")] 
    public T prefab; // 预制体资源
    public int initialCapacity = 10; // 初始化容量
    public int maxCapacity = 50; // 最大容量
    public float cleanupInterval = 30.0f; // 清理间隔（秒）

    [Header("LRU配置")] 
    public int lruThreshold = 20; // LRU清理阈值
    public bool autoClenup = true; // 自动清理开关
    
    // 对象池集合
    private Queue<T> availableObjects = new();
    private Dictionary<T, DateTime> usedObjects = new();
    private LinkedList<T> lruList = new();

    private float lastCleanupTime = 0f;
    private Transform poolContainer;     // 对象容器
}
