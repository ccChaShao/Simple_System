using UnityEngine.Events;

namespace System.RedDot.RunTime
{
    /// <summary>
    /// 红点表现类——用于存储节点的表现数据
    /// </summary>
    public class RedDotView
    {
        public string path;
        
        private RedDotNode m_ReddotNode;
        
        private RedDotModule m_RedDotModule;
        
        private UnityAction<RedDotNode> m_OnRedDotUpdate;

        public void SetData(RedDotModule module, string path, UnityAction<RedDotNode> onRedDotUpdate)
        {
            this.path = path;
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