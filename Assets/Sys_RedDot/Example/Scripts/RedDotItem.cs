using System.Collections;
using System.Collections.Generic;
using System.RedDot.RunTime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace System.RedDot.Example
{
    public class RedDotItem : MonoBehaviour
    {
        [TitleGroup("属性")] public RedDotType type;
        
        [TitleGroup("属性")] public string path;
        

        [TitleGroup("组件")] public TextMeshProUGUI text;
        
        [TitleGroup("组件")] public Image image;
        
        // 引用
        private RedDotModule m_RedDotModule;
        private RedDotView m_RedDotView;

        private void Awake()
        {
            m_RedDotModule = Manager.Instance.redDotModule;
        }

        private void OnEnable()
        {
            Debug.Log("charsiew : [OnEnable] : ----------------------");
        }

        public void RefreshView()
        {
            
        }
    }
}
