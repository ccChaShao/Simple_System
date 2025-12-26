 /*
* @Author: Charlie
* @Email: 203981473@qq.com
* @Description: DRedDotModule class for managing the RedDot system and its nodes.
* @Date: 2025-04-16 11:04:10
* @Modify:
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DL.RedDot.Runtime
{
    /// <summary>
    /// Red dot module
    /// </summary>
    public class DRedDotModule
    {
        private DRedDotNode m_Root; //public parent node 
        private List<DRedDotNode> m_Nodes;
        private string m_Pattern;  
        
        //storage all need refresh node
        private HashSet<DRedDotNode> m_DirtyViewNodes = new HashSet<DRedDotNode>();
        private Dictionary<DRedDotNode, int> m_ChangedLeafNodes = new Dictionary<DRedDotNode, int>();
        
        // generate node id mapping  example "A_B_C"  => root_A_B_C
        /// <summary>
        /// generate red dot node tree
        /// node config structure : A$B$C  root = A  pattern = $ , A_B_C  root = A  pattern = _
        /// </summary>
        /// <param name="root">root node</param>
        /// <param name="nodeMap">all node mapping map</param>
        /// <param name="pattern">split char</param>
        public void Initialize(string root, List<string> nodeMap, string pattern = "_")
        {
            m_Pattern = pattern;
            m_Root = new(root, null);
            foreach (var node in nodeMap)
            {
                var currentNode = m_Root;
                var keys = SplitAndTrim(node, m_Pattern).ToArray();
                //we generally only maintain one root
                for (int i = 1; i < keys.Length; i++)
                {
                    var key = keys[i];
                    //so when configuring, you should carefully determine node naming
                    var newNode = currentNode.AddChild(key);
                    currentNode = newNode;
                }
            }

            // RedDotModuleDebug();
        }

        /// <summary>
        /// add callback for nodeStr node
        /// </summary>
        /// <param name="nodeStr"></param>
        /// <param name="callback"></param>
        public void AddRedDotNodeViewCallBack(string nodeStr, DRedDotUpdateView callback)
        {
            var node = GetRedDotNode(nodeStr);
            if (node != null)
            {
                node.OnUpdateViewEvent += callback;
            }
        }
        /// <summary>
        /// add callback for node node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="callback"></param>
        public void AddRedDotNodeViewCallBack(DRedDotNode node, DRedDotUpdateView callback)
        {
            if (node != null)
            {
                node.OnUpdateViewEvent += callback;
            }
        }
            
        /// <summary>
        /// remove callback for nodeStr node
        /// </summary>
        /// <param name="nodeStr"></param>
        /// <param name="callback"></param>
        public void RemoveRedDotNodeViewCallBack(string nodeStr, DRedDotUpdateView callback)
        {
            var node = GetRedDotNode(nodeStr);
            if (node != null)
            {
                node.OnUpdateViewEvent -= callback;
            }
        }
        /// <summary>
        /// remove callback for node node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="callback"></param>
        public void RemoveRedDotNodeViewCallBack(DRedDotNode node, DRedDotUpdateView callback)
        {
            if (node != null)
            {
                node.OnUpdateViewEvent -= callback;
            }
        }

        /// <summary>
        /// update node count
        /// this method will cache update node. all need update node will be executed uniformly at a certain time
        /// </summary>
        /// <param name="nodeStr"></param>
        /// <param name="count"></param>
        public void UpdateRedDotCountDelay(string nodeStr, int count)
        {
            var node = GetRedDotNode(nodeStr);
            if (node == null) return;
            m_ChangedLeafNodes[node] = count;           // 记录改动的节点的数量
                
            //get all affected nodes by node
            var items = node.GetAllAffectedParentNodes();
    
            // if items is not empty, then add them to m_DirtyViewNodes
            if (items != null && items.Count > 0)
            {
                m_DirtyViewNodes.UnionWith(items);      // 记录所有脏表现节点
            }
        }


        /// <summary>
        /// update node count
        /// this method will not cache update node. execute update directly and notify upward
        /// </summary>
        /// <param name="nodeStr"></param>
        /// <param name="count"></param>
        public void UpdateRedDotCount(string nodeStr, int count)
        {
            var node = GetRedDotNode(nodeStr);
            if (node != null)
            {
                node.UpdateRedDotCount(count);
            }
        }

        /// <summary>
        /// provide a method to clear all red dots under the root node
        /// </summary>
        public void ClearRootAllNode()
        {
            var list = GetRedDotNodeChildren(m_Root);
            foreach (var cNode in list)
            {
                cNode.ClearRedDotCountSilently();
            }
            m_Root.UpdateRedDot();
        }

        /// <summary>
        /// provide a method to clear all red dots under a certain node
        /// this node can be any node
        /// </summary>
        /// <param name="nodeStr"></param>
        public void ClearRedDotNode(string nodeStr)
        {
            var node = GetRedDotNode(nodeStr);
            if (node == null) return;
            //recursively traverse all its child nodes
            var list = GetRedDotNodeChildren(node);
            foreach (var cNode in list)
            {
                cNode.ClearRedDotCountSilently();
            }
            node.UpdateRedDot();
        }

        /// <summary>
        /// cache data update method
        /// update layer by layer from bottom to top. avoid invalid operations and improve performance
        /// </summary>
        public void Update()
        {
            // 统一更新
            if (m_ChangedLeafNodes.Count == 0) return;
            
            //sort by level
            //first update changed child nodes 先更新叶子节点
            foreach (var node in m_ChangedLeafNodes)
            {
                node.Key.UpdateRedDotDelay(node.Value);         
            }
            
            // 这里要由最底层开始更新，避免漏算子节点
            var nodes = m_DirtyViewNodes.OrderByDescending(node => node.Level).ToList();
            //update parent nodes 后更新相应的父母节点
            foreach (var node in nodes)
            {
                node.UpdateRedDotDelay();           
            }
            m_ChangedLeafNodes.Clear();
            m_DirtyViewNodes.Clear();
        }

        /// <summary>
        /// get corresponding node
        /// </summary>
        /// <param name="nodeStr"></param>
        /// <returns></returns>
        public DRedDotNode GetRedDotNode(string nodeStr)
        {
            if(string.IsNullOrEmpty(nodeStr)) return null;
            if (nodeStr == m_Root.Name) return m_Root;
            var node = m_Root;
            var keys = SplitAndTrim(nodeStr, m_Pattern).ToArray();
            
            for (int i = 1; i < keys.Length; i++)
            {
                var key = keys[i];
                //child node exist key
                if (!node.Children.ContainsKey(key))
                {
                    //log node not exist
                    return null;
                }
                // 向下查找
                //search downward
                node = node.Children[key];
            }
            return node;
        }
    
        /// <summary>
        /// get all child nodes of current node
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private List<DRedDotNode> GetRedDotNodeChildren(DRedDotNode root)
        {
            List<DRedDotNode> list = new List<DRedDotNode>();
            Traverse(root,list,false);
            return list;
 
        }
        
        /// <summary>
        /// public debug method. will print current node structure
        /// </summary>
        public void RedDotModuleDebug()
        {
            var result = new List<string>();
            TraverseNodeDebug(m_Root, 0, result);
            Debug.Log(string.Join("", result));
        }

        private void TraverseNodeDebug(DRedDotNode node, int level, List<string> result)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < level; i++)
            {
                sb.Append("|    ");
            }

            if (level > 0)
            {
                sb.Append("|----");
            }

            var leaf = node.IsLeaf ? "----leaf" : "";
            sb.Append($"{node.Name}\t{node.Count}\t{leaf}\n");
            result.Add(sb.ToString());
            foreach (var childNode in node.Children.Values)
            {
                TraverseNodeDebug(childNode, level + 1, result);
            }
        }

        //search method
        private void Traverse(DRedDotNode node, List<DRedDotNode> list, bool isLeaf = false)
        {
            if (node == null) return;

            Stack<DRedDotNode> stack = new Stack<DRedDotNode>();
            stack.Push(node);
            while (stack.Count > 0)
            {
                DRedDotNode currentNode = stack.Pop();

                foreach (var child in currentNode.Children.Values)
                {
                    if (isLeaf)
                    {
                        if (child.IsLeaf)
                        {
                            list.Add(child);
                        }
                    }
                    else
                    {
                        list.Add(child);
                    }
                    stack.Push(child);
                }
            }
        }
        
        //avoid dependency
        /// <summary>
        /// string split method. trim left and right spaces and empty elements avoid input error
        /// </summary>
        /// <param name="input"></param>
        /// <param name="separators">split char</param>
        /// <returns></returns>
        private static IEnumerable<string> SplitAndTrim(string input, params string[] separators)
        {
            // handle empty input
            if (string.IsNullOrWhiteSpace(input))
            {
                return Enumerable.Empty<string>();
            }

            // set default separator (use whitespace when not provided)
            if (separators == null || separators.Length == 0)
            {
                separators = new[] { " " }; // default space separator
            }
            // execute split and process result
            return input.Split(separators, StringSplitOptions.None)
                .Select(s => s.Trim())             // trim whitespace
                .Where(s => !string.IsNullOrEmpty(s)); // filter empty items
        }
    }
}

