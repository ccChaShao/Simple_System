using System.Collections.Generic;

namespace System.RedDot.RunTime
{
    /// <summary>
    /// 红点模块——红点系统核心模块
    /// </summary>
    public class RedDotModule
    {
        private RedDotNode m_RootNode;
        private List<RedDotNode> m_AllNodes = new();
        private string m_Pattern;

        // 需要刷新的节点缓存
        private HashSet<RedDotNode> m_DirtyViewNodes = new(); // 父节点缓存
        private Dictionary<RedDotNode, int> m_ChangeLeafNodes = new(); // 叶子节点缓存

        public void Initalize(string rootKey, string pattern)
        {
            this.m_Pattern = pattern;
            
        }

        public void AddRedDotNodeViewCallBack(string key, RedDotUpdate onRedDotUpdate)
        {
            
        }

        public void RemoveRedDotNodeViewCallBack(string key, RedDotUpdate onRedDotUpdate)
        {
            
        }
    }
}