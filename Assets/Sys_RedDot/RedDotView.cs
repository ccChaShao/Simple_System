using UnityEngine.Events;

namespace System.RedDot.RunTime
{
    /// <summary>
    /// 红点表现类——用于存储节点的表现数据
    /// </summary>
    public class RedDotView
    {
        private string m_Path;
        public string path => m_Path;
        
        private RedDotNode m_ReddotNode;
        private RedDotModule m_RedDotModule;
        private UnityAction<RedDotNode> m_OnRedDotUpdate;

        public void SetData(RedDotModule module, string path, UnityAction<RedDotNode> onRedDotUpdate)
        {
            this.m_RedDotModule = module;
            this.m_Path = path;
            this.m_OnRedDotUpdate = onRedDotUpdate;
        }

        public void OnEnter()
        {
            m_RedDotModule.AddRedDotNodeViewCallBack(m_Path, m_OnRedDotUpdate);
        }

        public void OnExit()
        {
            m_RedDotModule.RemoveRedDotNodeViewCallBack(m_Path, m_OnRedDotUpdate);
        }
    }
}