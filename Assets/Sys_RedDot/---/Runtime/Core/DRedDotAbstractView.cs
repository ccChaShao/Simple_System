 /*
* @Author: Charlie
* @Email: 203981473@qq.com
* @Description: DRedDotAbstractView class for managing RedDot view updates and callbacks.
* @Date: 2025-04-16 11:04:57
* @Modify:
*/

namespace DL.RedDot.Runtime
{
    /// <summary>
    /// Delegate for updating the view of a red dot module node
    /// </summary>
    public delegate void DRedDotUpdateView(DRedDotNode node);

    /// <summary>
    /// Representation of a RedDot View
    /// </summary>
    public class DRedDotAbstractView
    {
        public DRedDotType Type;
        public string Key;
        public string Number;
        private DRedDotModule m_GetDRedDotModule;
        private DRedDotNode m_DRedDotNode;

        private event DRedDotUpdateView OnUpdateViewEvent;

        /// <summary>
        /// Need to specify the module, key, type, and display callback
        /// </summary>
        /// <param name="module"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void Init(DRedDotModule module, string key, DRedDotType type, DRedDotUpdateView callback)
        {
            Key = key;
            Type = type;
            m_GetDRedDotModule = module;
            m_DRedDotNode = m_GetDRedDotModule.GetRedDotNode(Key);
            OnUpdateViewEvent += callback;
        }

        /// <summary>
        /// Modify the display type and force refresh the view once
        /// </summary>
        /// <param name="type"></param>
        public void UpdateType(DRedDotType type)
        {
            if (type == Type) return;

            this.Type = type;
            // Force refresh
            if (OnUpdateViewEvent != null) OnUpdateViewEvent(m_DRedDotNode);
        }

        /// <summary>
        /// Called when the interface is displayed, register the red dot callback
        /// </summary>
        public void OnEnter()
        {
            if (OnUpdateViewEvent != null) OnUpdateViewEvent(m_DRedDotNode);
            m_GetDRedDotModule.AddRedDotNodeViewCallBack(m_DRedDotNode, OnUpdateViewEvent);
        }

        /// <summary>
        /// Called when the interface is closed, remove the red dot callback
        /// </summary>
        public void OnExit()
        {
            m_GetDRedDotModule.RemoveRedDotNodeViewCallBack(m_DRedDotNode, OnUpdateViewEvent);
        }
    }
}