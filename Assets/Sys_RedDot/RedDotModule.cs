using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace System.RedDot.RunTime
{
    /// <summary>
    /// 红点模块——红点系统核心模块
    /// </summary>
    [Serializable]
    public class RedDotModule
    {
        public string pattern;
        
        // 红点表
        private Dictionary<string, RedDotNode> m_nodeDic = new();

        // 需要刷新的节点缓存
        private Dictionary<RedDotNode, int> m_ChangeLeafNodes = new(); // 叶子节点缓存
        private HashSet<RedDotNode> m_DirtyViewNodes = new(); // 父节点缓存

        public void Initalize(string pattern = "_")
        {
            this.pattern = pattern;
        }

        public void Update()
        {
            // 统一更新
            if (m_ChangeLeafNodes.Count <= 0)
                return;
            
            // step1：更新叶子节点
            foreach (var kv in m_ChangeLeafNodes)
            {
                kv.Key.SetCount(kv.Value);
            }
            
            // step2：更新受影响父节点
            List<RedDotNode> affectedNodes = m_DirtyViewNodes.OrderByDescending(node => node.level).ToList();
            foreach (var node in affectedNodes)
            {
                node.RecalculatetCount();
            }
            
            // 清理
            m_ChangeLeafNodes.Clear();
            m_DirtyViewNodes.Clear();
        }

        public RedDotNode GetRedDotNode(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (!m_nodeDic.ContainsKey(path))
                return null;
            
            return m_nodeDic[path];
        }

        public RedDotNode CreateRedDotNode(string path, bool withoutNodify)
        {
            RedDotNode node = CreateRedDotNode(path);

            if (!withoutNodify)
                UpdateAffectedNodes(path);

            return node;
        }

        public RedDotNode CreateRedDotNode(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            //TODO 这里遵循语义，会覆盖已有的红点，假如想要获取/创建，请用“GerOrCreateRedDotNode”。
            // if (m_nodeDic.ContainsKey(path))
            //     return m_nodeDic[path];
            
            string[] keys = SplitAndTrim(path, pattern).ToArray();
            
            if (keys.Length <= 0)
                return null;
            
            StringBuilder stringBuilder = new();
            RedDotNode parentNode = null;
            
            for (int i = 0; i < keys.Length; i++)
            {
                string append = i != 0 ? $"_{keys[i]}" : keys[i];
                string newPath = stringBuilder.Append(append).ToString();
                if (!m_nodeDic.ContainsKey(newPath))
                {
                    // 添加该路径节点
                    RedDotNode newNode = new RedDotNode(newPath, parentNode);
                    m_nodeDic[newPath] = newNode;
                    // 添加该路径剩余节点
                    if (i < keys.Length - 1)
                    {
                        newPath = stringBuilder.Append($"_{keys[i + 1]}").ToString();
                        CreateRedDotNode(newPath);
                    }
                    return newNode;
                }
                
                parentNode = m_nodeDic[newPath];
            }

            return null;
        }

        public RedDotNode GerOrCreateRedDotNode(string path, bool withoutNodifyIfCreate)
        {
            RedDotNode node = GetRedDotNode(path);
            if (node == null)
            {
                node = CreateRedDotNode(path, withoutNodifyIfCreate);
            }

            return node;
        }

        public bool SetRedDotNodeCount(string path, int count)
        {
            RedDotNode node = GetRedDotNode(path);
            if (node == null)
                return false;

            // 记录改动的节点的数量
            m_ChangeLeafNodes[node] = count; 
            
            // 记录所有受影响的父节点
            List<RedDotNode> affectedNodes = node.GetAllParentNodes();
            if (affectedNodes != null && affectedNodes.Count > 0)
            {
                m_DirtyViewNodes.UnionWith(affectedNodes);
            }
            
            return true;
        }

        public void UpdateAffectedNodes(string path)
        {
            RedDotNode node = GetRedDotNode(path);
            if (node == null)
                return;
            
            // 记录所有受影响的父节点
            List<RedDotNode> affectedNodes = node.GetAllParentNodes();
            if (affectedNodes != null && affectedNodes.Count > 0)
            {
                m_DirtyViewNodes.UnionWith(affectedNodes);
            }
        }

        public bool AddOnceRedDotNodeCount(string path)
        {
            RedDotNode node = GetRedDotNode(path);
            if (node == null)
                return false;
            
            return SetRedDotNodeCount(path, node.count + 1);
        }

        public bool DecOnceRedDotNodeCount(string path)
        {
            RedDotNode node = GetRedDotNode(path);
            if (node == null)
                return false;
            
            return SetRedDotNodeCount(path, node.count - 1);
        }

        public void AddRedDotNodeViewCallBack(string path, UnityAction<RedDotNode> onRedDotUpdate)
        {
            RedDotNode node = GetRedDotNode(path);
            if (node == null)
                return;
            node.onRedDotUpdate.AddListener(onRedDotUpdate);
        }

        public void AddRedDotNodeViewCallBack(RedDotNode node, UnityAction<RedDotNode> onRedDotUpdate)
        {
            if (node == null)
                return;
            node.onRedDotUpdate.AddListener(onRedDotUpdate);
        }

        public void RemoveRedDotNodeViewCallBack(string path, UnityAction<RedDotNode> onRedDotUpdate)
        {
            RedDotNode node = GetRedDotNode(path);
            if (node == null)
                return;
            node.onRedDotUpdate.RemoveListener(onRedDotUpdate);
        }

        public void RemoveRedDotNodeViewCallBack(RedDotNode node, UnityAction<RedDotNode> onRedDotUpdate)
        {
            if (node == null)
                return;
            node.onRedDotUpdate.RemoveListener(onRedDotUpdate);
        }
        
        public void RedDotModuleDebug()
        {
            HashSet<string> rootHashSet = new();
            foreach (var kv in m_nodeDic)
            {
                string[] splits = SplitAndTrim(kv.Key).ToArray();
                if (splits.Length <= 0)
                    continue;
                
                string rootPath = splits[0];
                if (rootHashSet.Contains(rootPath))
                    continue;
                
                RedDotNode node = GetRedDotNode(rootPath);
                if (node == null)
                    continue;
                
                var result = new List<string>();
                TraverseNodeDebug(node, 0, result);
                Debug.Log(string.Join("", result));
            }
        }

        public void TraverseNodeDebug(RedDotNode node, int level, List<string> result)
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

            var leaf = node.isLeaf ? "----leaf" : "";
            sb.Append($"{node.path}\t{node.count}\t{leaf}\n");
            result.Add(sb.ToString());
            foreach (var childNode in node.chilNodeDic.Values)
            {
                TraverseNodeDebug(childNode, level + 1, result);
            }
        }
        
        public IEnumerable<string> SplitAndTrim(string input, params string[] separators)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return Enumerable.Empty<string>();
            }

            
            if (separators == null || separators.Length == 0)
            {
                separators = new[] { " " }; // default space separator
            }
            
            return input.Split(separators, StringSplitOptions.None)
                .Select(s => s.Trim())             // trim whitespace
                .Where(s => !string.IsNullOrEmpty(s)); // filter empty items
        }
    }
}