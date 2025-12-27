using System;
using System.Collections;
using System.Collections.Generic;
using System.RedDot.RunTime;
using Sirenix.OdinInspector;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager m_Instance;
    public static Manager Instance => m_Instance;
    
    [BoxGroup("RedDot")] public RedDotModule redDotModule;
    [BoxGroup("RedDot")] public GameObject redDotPrefab;
    
    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        
        InitModule();
    }

    private void Update()
    {
        UpdateModule();
    }

    private void InitModule()
    {
        redDotModule = new RedDotModule();
        redDotModule.Initalize(redDotPrefab, "_");
    }

    private void UpdateModule()
    {
        if (redDotModule != null)
        {
            redDotModule.Update();
        }
    }
}
