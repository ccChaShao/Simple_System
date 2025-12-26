 /*
* @Author: Charlie
* @Email: 203981473@qq.com
* @Description: RedDotItem class for managing individual RedDot UI elements.
* @Date: 2025-04-16 11:04:28
* @Modify:
*/

using System.Collections.Generic;
using DL.RedDot.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DL.RedDot.Example
{ 
    /// <summary>
    /// 表现层载体
    /// </summary>
    public class RedDotItem : MonoBehaviour
    {
        public DRedDotType type;
        public string key;

        private TextMeshProUGUI m_Number;  
        private Image m_Bg;
        
        public GameObject prefab;
        public GameObject redDot;
        
        //实例
        public DRedDotModule GetDRedDotModule;
        private DRedDotAbstractView m_View;
        
        private void Awake()
        {
            GetDRedDotModule = Manager.inst.DRedDotModule;
            redDot ??= Instantiate(prefab, transform);
            m_Bg = redDot.GetComponent<Image>();
            m_Number = redDot.GetComponentInChildren<TextMeshProUGUI>();
            
            m_View = new DRedDotAbstractView();
            m_View.Init(GetDRedDotModule, key, type, UpdateView);
        }

        private void OnEnable()
        {
            m_View.OnEnter();
        }
        
        private void OnDisable()
        {
           m_View.OnExit();
        }
        
        private void UpdateView(DRedDotNode node)
        {
            if (node.Count > 0)
            {
                m_Bg.gameObject.SetActive(true);
                m_Number.gameObject.SetActive(type == DRedDotType.Number);
                m_Number.text = node.Count.ToString();
            }
            else
            {
                m_Bg.gameObject.SetActive(false);
                m_Number.gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR
        private List<GameObject> tmplist = new();
        // [InspectorButton("Create")] 
        public void Create()
        {
            if (redDot == null)
            {
                redDot = Instantiate(prefab, transform);
                tmplist.Add(redDot);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(redDot);
                
            }
        }
        
        // [InspectorButton("Destroy")] 
        public void Destroy()
        {
            if (redDot != null)
            {
                tmplist.Remove(redDot);
                DestroyImmediate(redDot);
            }
        }
#endif
    }
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(RedDotItem))]
    public class RedDotItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create"))
            {
                UnityEditor.Undo.RecordObject(target, "Create");
                if (target is RedDotItem item)
                {
                    item.Create();
                }
                UnityEditor.EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button("Destroy"))
            {
                // 记录操作以便可以撤销
                UnityEditor.Undo.RecordObject(target, "Destroy");
                if (target is RedDotItem item)
                {
                    item.Destroy();
                }
                UnityEditor.EditorUtility.SetDirty(target);
            }
        }
    }
    
#endif
}