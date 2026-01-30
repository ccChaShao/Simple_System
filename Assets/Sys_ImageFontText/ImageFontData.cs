using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MySystem.ImageFont
{
    [CreateAssetMenu(fileName = "New ImageFont Data", menuName = "MySystem/UI/ImageFont Data")]
    public class ImageFontData : ScriptableObject
    {
        [Serializable]
        public class ChatSprioteMapping
        {
            public char character;
            public Sprite sprite;
        }
    
        [OnValueChanged(nameof(RebuildSpriteDict))]
        public List<ChatSprioteMapping> mappings = new ();

        private Dictionary<char, Sprite> m_SpriteDict = new();

        public Sprite GetSprite(char character)
        {
            if (m_SpriteDict.Count <= 0)
            {
                m_SpriteDict.Clear();
                
                foreach (var mapping in mappings)
                {
                    if (!m_SpriteDict.ContainsKey(mapping.character))
                    {
                        m_SpriteDict[mapping.character] = mapping.sprite;
                    }
                }
            }

            m_SpriteDict.TryGetValue(character, out Sprite sprite);
            return sprite;
        }

        private void RebuildSpriteDict()
        {
            m_SpriteDict.Clear();
            
            foreach (var mapping in mappings)
            {
                if (!m_SpriteDict.ContainsKey(mapping.character))
                {
                    m_SpriteDict[mapping.character] = mapping.sprite;
                }
            }
        }
    }
}
