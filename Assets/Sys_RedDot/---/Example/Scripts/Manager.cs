/*
* @Author: Charlie
* @Email: 203981473@qq.com
* @Description: Manager class for handling RedDot module and UI interactions.
* @Date: 2025-04-16 11:04:01
* @Modify:
*/

using System.Collections.Generic;
using DL.RedDot.Runtime;
using UnityEngine;

#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace DL.RedDot.Example
{
    public class Manager : MonoBehaviour
    {
        private static Manager _instance;
        public RedDotTestPanel panel;
        public DRedDotModule DRedDotModule;
        public RedDotTestPanel redDotTestPanelPrefab;
        public static Manager inst => _instance;
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        public List<string> nodeKeys = new List<string>()
        {
            "root_task",
            "root_email",
            "root_message",
        };

        void Start()
        {
            DRedDotModule = new DRedDotModule();
            DRedDotModule.Initialize("root", nodeKeys, "_");
            
            if (panel == null)
            {
                panel = Instantiate(redDotTestPanelPrefab, FindObjectOfType<Canvas>().transform);
                panel.name = "RedDotTestPanel";
            }
            
            if (show) 
            {
                panel.Show();
            }
            else
            {
                panel.Hide();
            }
        }

        public bool show = false;
        void Update()
        {
#if UNITY_INPUT_SYSTEM
            if (Keyboard.current.qKey.wasPressedThisFrame)
#else
            if (Input.GetKeyDown(KeyCode.Q))
#endif

            {
                if (show) 
                {
                    panel.Hide();
                }
                else
                {
                    panel.Show();
                }

                show = !show;
            }
            
            DRedDotModule.Update();
        }
        
#if UNITY_EDITOR
        public string nodeStr;
        public int count;

        // [InspectorButton("改变指定叶子节点数据")]
        public void ResetCount()
        {
            Manager.inst.DRedDotModule.UpdateRedDotCountDelay(nodeStr, count);
        }

        // [InspectorButton("清除指定节点的所有子节点")]
        public void ClearCount()
        {
            Manager.inst.DRedDotModule.ClearRedDotNode(nodeStr);
        }

        // [InspectorButton("显示维护的节点树")]
        public void RedDotModuleDebug()
        {
            Manager.inst.DRedDotModule.RedDotModuleDebug();
        }

        // [InspectorButton("添加打印回调")]
        public void AddRedDotNodeViewCallBack()
        {
            Manager.inst.DRedDotModule.AddRedDotNodeViewCallBack(nodeStr, (dotNode) => { Debug.Log($"{dotNode.Name}触发更新视图回调"); });
        }

#endif
    }
    
            
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Manager))]
    public class ManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("改变指定叶子节点数据"))
            {
                if (target is Manager item)
                {
                    item.ResetCount();
                }

                UnityEditor.EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button("清除指定节点的所有子节点"))
            {
                if (target is Manager item)
                {
                    item.ClearCount();
                }

                UnityEditor.EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button("显示维护的节点树"))
            {
                if (target is Manager item)
                {
                    item.RedDotModuleDebug();
                }

                UnityEditor.EditorUtility.SetDirty(target);
            }
            
            if (GUILayout.Button("添加打印回调"))
            {
                if (target is Manager item)
                {
                    item.AddRedDotNodeViewCallBack();
                }

                UnityEditor.EditorUtility.SetDirty(target);
            }
        }
    }


#endif
    

}

