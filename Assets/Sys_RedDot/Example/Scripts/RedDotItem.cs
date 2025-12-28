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
        [TitleGroup("属性")] public string path;
        
        [TitleGroup("属性")] public RedDotType type;

        [TitleGroup("属性")] public RedDotAnchorPreset anchorPreset;
        
        
        [TitleGroup("组件"), ReadOnly, ShowInInspector] private CanvasGroup m_CanvasGroup;

        [TitleGroup("组件"), ReadOnly, ShowInInspector] private TextMeshProUGUI m_Text;
        
        [TitleGroup("组件"), ReadOnly, ShowInInspector] private Image m_Image;
        
        // 引用
        private RectTransform m_RectTransform;
        private RedDotModule m_RedDotModule;
        private RedDotView m_RedDotView;

        private void Awake()
        {
            m_RedDotModule = Manager.Instance.redDotModule;
            m_RectTransform = GetComponent<RectTransform>();
            m_Image = GetComponentInChildren<Image>();
            m_Text = GetComponentInChildren<TextMeshProUGUI>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RedDotView = new ();
        }

        private void OnDestroy()
        {
            m_RedDotView.OnExit();
        }

        public void SetData(string path ,RedDotType type = RedDotType.Number, RedDotAnchorPreset anchorPreset = RedDotAnchorPreset.TopRight)
        {
            this.type = type;
            this.path = path;
            this.anchorPreset = anchorPreset;
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
            ApplyAnchorPreset(m_RectTransform, anchorPreset);
        }
        
        private void ApplyAnchorPreset(RectTransform rectTransform, RedDotAnchorPreset preset)
        {
            switch (preset)
            {
                case RedDotAnchorPreset.TopLeft:
                    rectTransform.anchorMin = new (0, 1);
                    rectTransform.anchorMax = new (0, 1);
                    break;
                case RedDotAnchorPreset.TopRight:
                    rectTransform.anchorMin = new (1, 1);
                    rectTransform.anchorMax = new (1, 1);
                    break;
                case RedDotAnchorPreset.MiddleCenter:
                    rectTransform.anchorMin = new(0.5f, 0.5f);
                    rectTransform.anchorMax = new(0.5f, 0.5f);
                    break;
                case RedDotAnchorPreset.BottomLeft:
                    rectTransform.anchorMin = new (0, 0);
                    rectTransform.anchorMax = new (0, 0);
                    break;
                case RedDotAnchorPreset.BottomRight:
                    rectTransform.anchorMin = new (1, 0);
                    rectTransform.anchorMax = new (1, 0);
                    break;
            }
            rectTransform.anchoredPosition3D = Vector3.zero;
        }

        public void OnRedDotDataUpdate(RedDotNode node)
        {
            RefreshView(node);
        }
    }
}
