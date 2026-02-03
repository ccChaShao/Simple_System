using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace MySystem.RedDot.RunTime
{
    /// <summary>
    /// 红点表现类——用于存储节点的表现数据
    /// </summary>
    [Serializable]
    public class RedDotView
    {
        [TitleGroup("属性")] public string path;
        
        [TitleGroup("属性")] public RedDotType type;

        [TitleGroup("属性")] public RedDotAnchorPreset anchorPreset;
        
        private RedDotNode m_ReddotNode;
        
        private RedDotModule m_RedDotModule;
        
        private UnityAction<RedDotNode> m_OnRedDotUpdate;

        public RedDotView(string path, RedDotType type, RedDotAnchorPreset anchorPreset)
        {
            this.path = path;
            this.type = type;
            this.anchorPreset = anchorPreset;
        }

        public void SetData(RedDotModule module, UnityAction<RedDotNode> onRedDotUpdate)
        {
            this.m_ReddotNode = module.GerOrCreateRedDotNode(path, false);
            this.m_RedDotModule = module;
            this.m_OnRedDotUpdate = onRedDotUpdate;
        }

        public void OnEnter()
        {
            m_RedDotModule.AddRedDotNodeViewCallBack(m_ReddotNode, m_OnRedDotUpdate);
        }

        public void OnExit()
        {
            m_RedDotModule.RemoveRedDotNodeViewCallBack(m_ReddotNode, m_OnRedDotUpdate);
        }
    }
}