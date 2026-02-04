using System.Collections;
using System.Collections.Generic;
using MySystem.ImageFont;
using UnityEngine;
using UnityEngine.UI;

namespace MySystem.ImageFont
{
    [AddComponentMenu("MySystem/UI/Image Font Text"), ExecuteAlways]
    public class ImageFontText : Graphic
    {
        [SerializeField] private ImageFontData m_FontData;
        public ImageFontData fontData
        {
            get => m_FontData;
            set { 
                m_FontData = value;
                // 字库内容变动，全dirty；
                SetAllDirty();
            }
        }

        [SerializeField, TextArea(3, 10)] private string m_Text = "";
        public string text
        {
            get => m_Text;
            set
            {
                m_Text = value; 
                // 文本改变，布局和顶点都可能变；
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }

        [SerializeField] private float m_CharSpacing = 0.0f;
        public float charSpacing
        {
            get{ return m_CharSpacing; }
            set { m_CharSpacing = value; SetAllDirty(); }
        }
        
        [SerializeField] private float m_LineSpacing = 0.0f;

        public float lineSpacing
        {
            get{ return m_LineSpacing; }
            set { m_LineSpacing = value;SetAllDirty(); }
        }

        public float pixelsPerUnit
        {
            get
            {
                // 优先使用FontData中第一个有效Sprite的pixelsPerUnit
                float spritePixelsPerUnit = 100; // 默认值
                if (fontData != null)
                {
                    foreach (var mapping in fontData.mappings)
                    {
                        if (mapping != null)
                        {
                            spritePixelsPerUnit = mapping.sprite.pixelsPerUnit;
                            break;
                        }
                    }
                }

                // 如果有Canvas，则使用Canvas的referencePixelsPerUnit；否则使用默认值。
                float referencePixelsPerUnit = 100;
                if (canvas != null)
                {
                    referencePixelsPerUnit = canvas.referencePixelsPerUnit;
                }

                return spritePixelsPerUnit / referencePixelsPerUnit;
            }
        }

        // 确保使用正确的纹理进行渲染
        public override Texture mainTexture
        {
            get
            {
                if (m_FontData != null)
                {
                    foreach (var mapping in m_FontData.mappings)
                    {
                        if (mapping.sprite != null && mapping.sprite.texture != null)
                        {
                            return mapping.sprite.texture;
                        }
                    }
                }
                
                return s_WhiteTexture;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            SetAllDirty();
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

            Rect myRect = rectTransform.rect;
            Vector2 myPivot = rectTransform.pivot;

            // 1：计算多少行；
            List<LineInfo> lines = new();
            LineInfo currentLine = new();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    lines.Add(currentLine);
                    currentLine = new LineInfo();
                    continue;
                }
        
                Sprite sprite = fontData.GetSprite(c);
                if (sprite == null)
                {
                    continue;
                }
                
                // 关键修正：所有尺寸计算，必须在预计算阶段就除以pixelsPerUnit，转换为Canvas单位
                float charWidth = sprite.rect.width / pixelsPerUnit;        // 计算有多少个unity单位；
                float charHeight = sprite.rect.height / pixelsPerUnit;      // 计算有多少个unity单位；
        
                // 换行
                if (((currentLine.width + charWidth) > myRect.width) && currentLine.chars.Count > 0)
                {
                    lines.Add(currentLine);
                    currentLine = new LineInfo();
                }
        
                currentLine.chars.Add(new CharInfo { character = c, sprite = sprite, width = charWidth, height = charHeight });
                currentLine.width += charWidth + (currentLine.chars.Count > 1 ? m_CharSpacing : 0);
                currentLine.height = Mathf.Max(currentLine.height, charHeight);
            }
            lines.Add(currentLine);
            
            // 2：计算高度
            float totalHeight = 0;
            foreach (var line in lines)
            {
                float lineHeight = (line.height > 0 ? line.height : fontData.defaultLineHeight / pixelsPerUnit);
                totalHeight += lineHeight;
            }
            totalHeight += Mathf.Max(0, lines.Count - 1) * m_LineSpacing;
            
            // 3：顶点创建
            // startY的计算方式：顶部下移多少可以做到垂直居中（先去到上侧顶点，然后下沉多少）
            // 从轴心点到上侧顶点的距离 - 居中偏移量（上下各留了多少空余量）
            float startY = ((1 - myPivot.y) * myRect.height) - ((myRect.height - totalHeight) * 0.5f); 
            float currentY = startY; 
            foreach (var line in lines)
            {
                float lineHeight = (line.height > 0 ? line.height : fontData.defaultLineHeight / pixelsPerUnit);
                // startX的计算方式：左侧右移多少可以做到平行居中（先去到左侧顶点，然后右移多少）
                // 从轴心点到左顶的距离 + 居中偏移量（左右各留了多少余量）
                float startX = (-myPivot.x * myRect.width) + ((myRect.width - line.width) * 0.5f);
                float currentX = startX;
        
                foreach (var charInfo in line.chars)
                {
                    Sprite sprite = charInfo.sprite;
                    Vector4 outerUV = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
        
                    // Y坐标的计算需要考虑行高和字符自身高度的差异，以实现底部对齐 (0.0f)
                    float yOffset = (lineHeight - charInfo.height) * 0.0f;
                    Vector3 bottomLeft = new Vector3(currentX, currentY - lineHeight + yOffset);
                    Vector3 topLeft = new Vector3(currentX, currentY - lineHeight + yOffset + charInfo.height);
                    Vector3 topRight = new Vector3(currentX + charInfo.width, currentY - lineHeight + yOffset + charInfo.height);
                    Vector3 bottomRight = new Vector3(currentX + charInfo.width, currentY - lineHeight + yOffset);
        
                    AddQuad(vh, bottomLeft, topLeft, topRight, bottomRight, color, outerUV);
        
                    currentX += charInfo.width + m_CharSpacing; // X向右渲染；
                }
        
                currentY -= (lineHeight + m_LineSpacing); // Y向下渲染；
            }
        
            // // 记录当前绘制光标的位置
            // float currentX = 0.0f;
            //
            // foreach (char c in m_Text)
            // {
            //     Sprite sprite = m_FontData.GetSprite(c);
            //     if (sprite == null)
            //     {
            //         Debug.Log("[OnPopulateMesh] : sprite is null !" + c);
            //         continue;
            //     }
            //     
            //     // 获取sprite的UV坐标和尺寸
            //     Vector4 outerUV = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
            //     float charWidth = sprite.rect.width;
            //     float charHeight = sprite.rect.height;
            //     
            //     // 计算当前字符的四个顶点的位置
            //     Vector3 bottomLeft = new(currentX, 0.0f);
            //     Vector3 topLeft = new(currentX, charHeight);
            //     Vector3 topRight = new(currentX + charWidth, charHeight);
            //     Vector3 bottomRight = new(currentX + charWidth, 0.0f);
            //     
            //     // 将这个字符的网络（一个quad）添加到顶点帮助器中
            //     AddQuad(vh, bottomLeft, topLeft, topRight, bottomRight, color, outerUV);
            //
            //     // 移动光标到下一个字符起始位置
            //     currentX += charWidth;
            // }
        }
        
        /// <summary>
        /// 添加正方形
        /// </summary>
        private void AddQuad(VertexHelper vh, Vector3 bottomLeft, Vector3 topLeft, Vector3 topRight, Vector3 bottomRight, Color32 color, Vector4 uv)
        {
            // 1. 记录当前的定点数量
            int vertIndex = vh.currentVertCount;   
            
            // 2. UV定点映射（左下开始）
            vh.AddVert(bottomLeft, color, new Vector2(uv.x, uv.y));     // 左下
            vh.AddVert(topLeft, color, new Vector2(uv.x, uv.w));        // 左上
            vh.AddVert(topRight, color, new Vector2(uv.z, uv.w));       // 右上
            vh.AddVert(bottomRight, color, new Vector2(uv.z, uv.y));    // 右下
            
            // 3. 使用上面的顶点添加三角形
            vh.AddTriangle(vertIndex, vertIndex + 1, vertIndex + 2);    // 三角形 1
            vh.AddTriangle(vertIndex + 2, vertIndex + 3, vertIndex);    // 三角形 2
        }

        #region Helper Classes
        private class CharInfo { public char character; public Sprite sprite; public float width; public float height; }
        private class LineInfo { public float width = 0f; public float height = 0f; public List<CharInfo> chars = new List<CharInfo>(); }
        #endregion
    }
}
