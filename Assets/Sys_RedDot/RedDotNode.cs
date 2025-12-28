using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace System.RedDot.RunTime
{
    public class RedDotUpdate : UnityEvent<RedDotNode> { }
        
    /// <summary>
    /// 红点节点类——处理节点的基本数据
    /// </summary>
    public class RedDotNode
    {
        public string path { get; private set; }
        public int count { get; private set; }

        // 父节点
        private RedDotNode m_ParentDotNode;
        public RedDotNode parentDotNode;
    
        // 子节点表
        private Dictionary<string, RedDotNode> m_ChilNodeDic = new();
        public Dictionary<string, RedDotNode> chilNodeDic => m_ChilNodeDic;

        // 更新回调
        private RedDotUpdate m_OnRedDotUpdate = new();
        public RedDotUpdate onRedDotUpdate => m_OnRedDotUpdate;
        
        public int level => m_ParentDotNode == null ? 0 : m_ParentDotNode.level + 1;
    
        public bool isLeaf => m_ChilNodeDic == null || m_ChilNodeDic.Count <= 0;

        public RedDotNode(string path, RedDotNode parentDotNode)
        {
            this.path = path;
            this.m_ParentDotNode = parentDotNode;
            parentDotNode?.SetChildNode(this);
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        public void SetChildNode(RedDotNode node)
        {
            m_ChilNodeDic[node.path] = node;
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        public bool RemoveChildNode(string path)
        {
            return m_ChilNodeDic.Remove(path);
        }

        /// <summary>
        /// 获取所有父节点
        /// </summary>
        /// <returns></returns>
        public List<RedDotNode> GetAllParentNodes()
        {
            if (!isLeaf)
                return null;
            
            List<RedDotNode> result = new();
            RedDotNode parentDotNode = this.m_ParentDotNode;
            while (parentDotNode != null)
            {
                result.Add(parentDotNode);
                parentDotNode = parentDotNode.m_ParentDotNode;
            }

            return result;
        }

        /// <summary>
        /// 重新计算数量
        /// </summary>
        public void RecalculatetCount()
        {
            int result = 0;
            
            if (!isLeaf)
            {
                foreach (var kv in m_ChilNodeDic)
                {
                    result += kv.Value.count;
                }
            }
            else
            {
                result = count;
            }

            if (count != result)
            {
                count = Math.Clamp(result, 0, 9999);
                NotifyUpdate();
            }
        }
        
        /// <summary>
        /// 设置数量
        /// </summary>
        public void SetCount(int count)
        {
            if (!isLeaf)
                return;

            if (this.count != count)
            {
                this.count = Math.Clamp(count, 0, 9999);
                NotifyUpdate();
            }
        }

        /// <summary>
        /// 清理数量
        /// </summary>
        public void ClearCount()
        {
            if (!isLeaf)
                return;
            
            if (count != 0)
            {
                count = 0;
                NotifyUpdate();
            }
        }

        /// <summary>
        /// 通知更新
        /// </summary>
        private void NotifyUpdate()
        {
            m_OnRedDotUpdate?.Invoke(this);
            if (m_ParentDotNode != null)
            {
                m_ParentDotNode.RecalculatetCount();
            }
        }
    }
}
