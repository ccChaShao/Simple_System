using System;
using System.Collections;
using System.Collections.Generic;
using System.RedDot.Example;
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
        redDotModule.Initalize("_");
    }

    private void UpdateModule()
    {
        if (redDotModule != null)
        {
            redDotModule.Update();
        }
    }

    //TODO 这部分应该放在游戏业务逻辑管理器中。
    public void BindRedDot(RedDotBindData dotBindData)
    {
        // 创建数据
        RedDotNode node = redDotModule.CreateRedDotNode(dotBindData.path);
        if (node == null)
        {
            return;
        }
        // 创建红点
        RedDotItem item = dotBindData.rectTransform.GetComponentInChildren<RedDotItem>(); // 仅在该节点查询
        if (item == null)
        {
            GameObject ins = Instantiate(redDotPrefab, dotBindData.rectTransform);
            item = ins.GetComponent<RedDotItem>();
        }
        item.SetData(dotBindData.path, dotBindData.type);
        item.RefreshView(node);
    }
}
