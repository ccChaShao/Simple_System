using System;
using System.Collections;
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
    
        // 红点数量
        private int m_Count;
        public int count => m_Count;

        // 父节点
        private RedDotNode _mParentDotNode;
        public RedDotNode ParentDotNode => _mParentDotNode;
    
        // 子节点表
        private Dictionary<string, RedDotNode> m_ChilNodeDic = new();
        public Dictionary<string, RedDotNode> chilNodeDic => m_ChilNodeDic;

        // 更新回调
        private RedDotUpdate m_OnRedDotUpdate = new();
        public RedDotUpdate onRedDotUpdate => m_OnRedDotUpdate;
        
        public int level => _mParentDotNode == null ? 0 : _mParentDotNode.level + 1;
    
        public bool isLeaf => m_ChilNodeDic == null || m_ChilNodeDic.Count <= 0;

        public RedDotNode(string path)
        {
            this.path = path;
            this._mParentDotNode = null;
        }

        public RedDotNode(string path, RedDotNode parentDotNode)
        {
            this.path = path;
            this._mParentDotNode = parentDotNode;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public RedDotNode AddChildNode(string path)
        {
            RedDotNode redDotNode;
            if (!m_ChilNodeDic.TryGetValue(path, out redDotNode))
            {
                redDotNode = new(path);
                m_ChilNodeDic[path] = redDotNode;
            }

            return redDotNode;
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
            RedDotNode parentDotNode = _mParentDotNode;
            while (parentDotNode != null)
            {
                result.Add(parentDotNode);
                parentDotNode = parentDotNode._mParentDotNode;
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
            if (_mParentDotNode != null)
            {
                _mParentDotNode.RecalculatetCount();
            }
        }
    }
}
