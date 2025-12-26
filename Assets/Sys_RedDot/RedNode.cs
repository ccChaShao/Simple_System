using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace System.RedDot.RunTime
{
    public class RedDotUpdate : UnityEvent<RedNode> { }
        
    public class RedNode
    {
        public string name { get; private set; }
    
        // 红点数量
        private int m_Count;
        public int count => m_Count;

        // 父节点
        private RedNode m_ParentNode;
        public RedNode parentNode => m_ParentNode;
    
        // 子节点表
        private Dictionary<string, RedNode> m_ChilNodeDic = new();
        public Dictionary<string, RedNode> chilNodeDic => m_ChilNodeDic;

        // 更新回调
        private RedDotUpdate m_OnRedDotUpdate = new();
        public RedDotUpdate onRedDotUpdate => m_OnRedDotUpdate;
        
        public int level => m_ParentNode == null ? 0 : m_ParentNode.level + 1;
    
        public bool isLeaf => m_ChilNodeDic == null || m_ChilNodeDic.Count <= 0;

        public RedNode(string name)
        {
            this.name = name;
            this.m_ParentNode = null;
        }

        public RedNode(string name, RedNode parentNode)
        {
            this.name = name;
            this.m_ParentNode = parentNode;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RedNode AddChildNode(string name)
        {
            RedNode redNode;
            if (!m_ChilNodeDic.TryGetValue(name, out redNode))
            {
                redNode = new(name);
                m_ChilNodeDic[name] = redNode;
            }

            return redNode;
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveChildNode(string name)
        {
            return m_ChilNodeDic.Remove(name);
        }

        /// <summary>
        /// 获取所有父节点
        /// </summary>
        /// <returns></returns>
        public List<RedNode> GetAllParentNodes()
        {
            if (!isLeaf)
                return null;
            
            List<RedNode> result = new();
            RedNode parentNode = m_ParentNode;
            while (parentNode != null)
            {
                result.Add(parentNode);
                parentNode = parentNode.m_ParentNode;
            }

            return result;
        }

        /// <summary>
        /// 重新计算数量
        /// </summary>
        public void RecalculatetCount()
        {
            int chilCount = 0;
            
            if (!isLeaf)
            {
                foreach (var kv in m_ChilNodeDic)
                {
                    chilCount += kv.Value.count;
                }
            }

            if (m_Count != chilCount)
            {
                m_Count = chilCount;
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

            if (m_Count != count)
            {
                m_Count = count;
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
            
            if (m_Count != 0)
            {
                m_Count = 0;
                NotifyUpdate();
            }
        }

        /// <summary>
        /// 通知更新
        /// </summary>
        private void NotifyUpdate()
        {
            m_OnRedDotUpdate?.Invoke(this);
            if (m_ParentNode != null)
            {
                m_ParentNode.RecalculatetCount();
            }
        }
    }
}
