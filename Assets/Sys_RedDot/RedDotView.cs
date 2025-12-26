namespace System.RedDot.RunTime
{
    /// <summary>
    /// 红点表现类——用于存储节点的表现数据
    /// </summary>
    public class RedDotView
    {
        public string redDotKey;
        public RedDotType redDotType;

        private RedDotNode m_ReddotNode;
        private RedDotModule m_RedDotModule;
        private RedDotUpdate m_OnRedDotUpdate;

        public void SetData(RedDotModule module, string redDotKey, RedDotType redDotType, RedDotUpdate onRedDotUpdate)
        {
            this.m_RedDotModule = module;
            this.redDotKey = redDotKey;
            this.redDotType = redDotType;
            this.m_OnRedDotUpdate = onRedDotUpdate;
        }

        public void OnEnter()
        {
            m_RedDotModule.AddRedDotNodeViewCallBack(redDotKey, m_OnRedDotUpdate);
        }

        public void OnExit()
        {
            m_RedDotModule.RemoveRedDotNodeViewCallBack(redDotKey, m_OnRedDotUpdate);
        }
    }
}