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
        
        
        [TitleGroup("组件"), ReadOnly, ShowInInspector] private CanvasGroup m_CanvasGroup;

        [TitleGroup("组件"), ReadOnly, ShowInInspector] private TextMeshProUGUI m_Text;
        
        [TitleGroup("组件"), ReadOnly, ShowInInspector] private Image m_Image;
        
        // 引用
        private RedDotModule m_RedDotModule;
        private RedDotView m_RedDotView;

        private void Awake()
        {
            m_RedDotModule = Manager.Instance.redDotModule;
            m_Image = GetComponentInChildren<Image>();
            m_Text = GetComponentInChildren<TextMeshProUGUI>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RedDotView = new ();
        }

        private void OnDestroy()
        {
            m_RedDotView.OnExit();
        }

        public void SetData(string path ,RedDotType type = RedDotType.Number)
        {
            this.type = type;
            this.path = path;
            m_RedDotView.SetData(m_RedDotModule, path, OnRedDotDataUpdate);
            m_RedDotView.OnEnter();
        }

        public void RefreshView(RedDotNode node)
        {
            if (node == null)
            {
                m_CanvasGroup.alpha = 0;
                return;
            }
            if (node.count > 0)
            {
                m_Text.text = node.count.ToString();
                m_Text.gameObject.SetActive(type == RedDotType.Number);
            }
            m_CanvasGroup.alpha = node.count > 0 ? 1f : 0f;
        }

        public void OnRedDotDataUpdate(RedDotNode node)
        {
            RefreshView(node);
        }
    }
}
