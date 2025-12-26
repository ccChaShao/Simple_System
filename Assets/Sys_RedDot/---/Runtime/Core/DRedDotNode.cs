/*
* @Author: Charlie
* @Email: 203981473@qq.com
* @Description: DRedDotNode class for managing RedDot nodes and their relationships.
* @Date: 2025-04-16 11:04:22
* @Modify:
*/
using System.Collections.Generic;

namespace DL.RedDot.Runtime
{
    /// <summary>
    /// RedDot Node
    /// </summary>
    public class DRedDotNode
    {
        public string Name { get; private set; }
        private int m_Count;
        private DRedDotNode m_Parent;
        private Dictionary<string, DRedDotNode> m_Children = new();
        
        public bool IsLeaf => m_Children == null || m_Children.Count == 0;
        // Level
        public int Level => m_Parent == null ? 0 : m_Parent.Level + 1;
        public int Count => m_Count;

        /// <summary>
        /// Called when the view is updated, this will be updated uniformly in the module
        /// </summary>
        public event DRedDotUpdateView OnUpdateViewEvent;

        public Dictionary<string, DRedDotNode> Children => m_Children;

        public DRedDotNode(string name, DRedDotNode parent)
        {
            Name = name;
            m_Parent = parent;
        }
        
        /// <summary>
        /// Get the key of the current node
        /// </summary>
        /// <returns></returns>
        public string GetFullKey()
        {
            var result = Name;
            var node = m_Parent;
            while (node != null)
            {
                result = $"{node.Name}_{result}";
                node = node.m_Parent;
            }
            return result;
        }
        
        /// <summary>
        /// Add a child node
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DRedDotNode AddChild(string name)
        {
            DRedDotNode node;
            if (m_Children.ContainsKey(name))
            {
                node = m_Children[name];
            }
            else
            {
                node = new DRedDotNode(name, this);
                m_Children.Add(name, node);
            }

            return node;
        }
        
        /// <summary>
        /// Remove a child node
        /// </summary>
        /// <param name="name"></param>
        public void RemoveChild(string name)
        {
            if (m_Children.ContainsKey(name))
            {
                m_Children.Remove(name);
            }
        }
        
        /// <summary>
        /// Return all affected nodes (excluding self)
        /// </summary>
        /// <returns></returns>
        public List<DRedDotNode> GetAllAffectedParentNodes()
        {
            // Non-leaf nodes are not processed
            if (!IsLeaf) return null;
            List<DRedDotNode> list = new List<DRedDotNode>();
            var node = m_Parent;
            while (node != null)
            {
                list.Add(node);
                node = node.m_Parent;
            }
            return list;
        }
        
        /// <summary>
        /// Recalculate the number of child nodes
        /// If it is a leaf, use count
        /// </summary>
        /// <param name="count"></param>
        public void UpdateRedDotDelay(int count = 0)
        {
            // Leaf node
            if (IsLeaf)
            {
                m_Count = count;
            }
            else
            {
                var childrenCount = 0;
                foreach (var node in m_Children)
                {
                    childrenCount += node.Value.m_Count;
                }
                m_Count = childrenCount;
            }
            // Refresh view
            OnUpdateViewEvent?.Invoke(this);
        }
        
        public void UpdateRedDotCount(int count)
        {
            // Non-leaf nodes cannot be updated
            if (!IsLeaf) return;
            if (m_Count != count)
            {
                this.m_Count = count;
                OnUpdateViewEvent?.Invoke(this);
                // Notify upper layer of change
                if (m_Parent != null)
                {
                    m_Parent.UpdateRedDot();
                }
            }
        }

        public void ClearRedDotCountSilently()
        {
            m_Count = 0; // Directly manipulate the field
            OnUpdateViewEvent?.Invoke(this); // Only update the view
        }
        
        // Update the current node
        public void UpdateRedDot()
        {
            var childrenCount = 0;
            foreach (var node in m_Children)
            {
                childrenCount += node.Value.m_Count;
            }

            if (m_Count != childrenCount)
            {
                m_Count = childrenCount;
                NotifyUp();
            }

        }

        // Notify up
        private void NotifyUp()
        {
            // Update the current view
            OnUpdateViewEvent?.Invoke(this);
            // Notify the upper layer to update
            if (m_Parent != null)
            {
                m_Parent.UpdateRedDot();
            }
        }

    }
}
