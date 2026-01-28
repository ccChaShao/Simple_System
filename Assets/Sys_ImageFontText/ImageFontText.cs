using System.Collections;
using System.Collections.Generic;
using MySystem.ImageFont;
using UnityEngine;
using UnityEngine.UI;

namespace MySystem.ImageFont
{
    [AddComponentMenu("MySystem/UI/Image Font Text"), ExecuteAlways]
    public class ImageFontText : MaskableGraphic
    {
        [SerializeField] private ImageFontData m_FontData;
        public ImageFontData fontData
        {
            get { return m_FontData; }
            set { m_FontData = value; SetAllDirty();}
        }

        [SerializeField, TextArea(3, 10)] private string m_Text = "";
        public string text
        {
            get {  return m_Text; }
            set { m_Text = value; SetAllDirty();}
        }

        [SerializeField] private float m_CharSpacing = 0.0f;
        public float charSpacing
        {
            get{ return m_CharSpacing; }
            set { m_CharSpacing = value; SetAllDirty(); }
        }

        // 确保使用正确的纹理进行渲染
        public override Texture mainTexture
        {
            get
            {
                if (m_FontData == null || m_FontData.mappings.Count == 0)
                {
                    return s_WhiteTexture;
                }

                foreach (var mapping in m_FontData.mappings)
                {
                    if (mapping.sprite != null && mapping.sprite.texture != null)
                    {
                        return mapping.sprite.texture;
                    }
                }
                
                return s_WhiteTexture;
            }
        }

        // 核心方法：UGUI再需要重建网格时自动调用此方法
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // 清除vh的旧网格数据
            vh.Clear();
            
            // 容错处理
            if (fontData == null || string.IsNullOrEmpty(m_Text))
            {
                return;
            }
        
            // 记录当前绘制光标的位置
            float currentX = 0.0f;
        
            foreach (char c in m_Text)
            {
                Sprite sprite = m_FontData.GetSprite(c);
                if (sprite == null)
                {
                    Debug.Log("[OnPopulateMesh] : sprite is null !" + c);
                    continue;
                }
                
                // 获取sprite的UV坐标和尺寸
                Vector4 outerUV = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
                float charWidth = sprite.rect.width;
                float charHeight = sprite.rect.height;
                
                // 计算当前字符的四个顶点的位置
                Vector3 bottomLeft = new(currentX, 0.0f);
                Vector3 topLeft = new(currentX, charHeight);
                Vector3 topRight = new(currentX + charWidth, charHeight);
                Vector3 bottomRight = new(currentX + charWidth, 0.0f);
                
                // 将这个字符的网络（一个quad）添加到顶点帮助器中
                AddQuad(vh, bottomLeft, topLeft, topRight, bottomRight, color, outerUV);
        
                // 移动光标到下一个字符起始位置
                currentX += charWidth;
            }
        }

        private void AddQuad(VertexHelper vh, Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, Color32 color, Vector4 uv)
        {
            int vertIndex = vh.currentVertCount;   
            
            // 1. UV定点映射（左下开始）
            vh.AddVert(bottomLeft, color, new Vector2(uv.x, uv.y));     // 左下
            vh.AddVert(topLeft, color, new Vector2(uv.x, uv.w));        // 左上
            vh.AddVert(topRight, color, new Vector2(uv.z, uv.w));       // 右上
            vh.AddVert(bottomRight, color, new Vector2(uv.z, uv.y));    // 右下
            
            // 2. 添加字体三角形（左上开始）                     // 记录当前起始顶点索引
            vh.AddTriangle(vertIndex, vertIndex + 1, vertIndex + 2);    // 三角形 1
            vh.AddTriangle(vertIndex + 2, vertIndex + 3, vertIndex);    // 三角形 2
        }
    }
}
