using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace System.RedDot.RunTime
{
    public enum RedDotType
    {
        Dot, // Dot shape
        Number // Number
    }
    
    
    [Serializable]
    public class RedDotBindData
    {
        public string path;
        public RedDotType type = RedDotType.Dot;
        public RectTransform rectTransform;

        public RedDotBindData(string path, RectTransform rectTransform)
        {
            this.path = path;
            this.rectTransform = rectTransform;
        }
    }
}
