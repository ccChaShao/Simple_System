using System.Collections;
using System.Collections.Generic;
using MySystem.ImageFont;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("MySystem/UI/Image Font Text")]
public class ImageFontText : MaskableGraphic
{
    [SerializeField] private ImageFontData m_FontData;
    public ImageFontData fontData
    {
        get { return m_FontData; }
        set { m_FontData = value; SetAllDirty();}   // 设置新数据时标记脏数据，触发重建
    }

    [SerializeField, TextArea(3, 10)] private string m_Text;
    public string text
    {
        get { return m_Text; }
        set { m_Text = value; SetAllDirty();}   // 文本变化时标记脏数据，触发重建
    }

    // 确保使用正确的纹理进行渲染
    public override Texture mainTexture
    {
        get
        {
            if (m_FontData == null)
            {
                return base.mainTexture;
            }
            Sprite sprite = m_FontData.GetSprite('1');
            if (sprite == null)
            {
                return base.mainTexture;
            }
            return sprite.texture;
        }
    }

    // 辅助方法：向VertexHelper中添加一个四边形（两个三角形）
    // （字体的网格是左上角为起点，图片的网格是左下角为起点）
    private void AddQuad(VertexHelper vh, Vector3 bottomLeft, Vector3 bottomRight, Vector3 topLeft, Vector3 topRight, Color color, Vector4 uv)
    {
        int startVertexIndex = vh.currentIndexCount;
    }
}
