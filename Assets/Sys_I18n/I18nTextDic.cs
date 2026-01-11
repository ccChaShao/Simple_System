using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace System.I18n.RunTime
{
    public struct I18nTextItem
    {
        public string key;
        public string text;

        public I18nTextItem(string key, string text)
        {
            this.key = key;
            this.text = text;
        }
    }
    
    public class I18nTextDic
    {
        public const uint TICK_INTERVAL = 10;
        public const string KEY_SPLITTER = "|^|";
        
        private Dictionary<string, I18nTextItem> m_i18nDic = new();

        // 后续用于清理闲置字符
        private float m_gcTickTime = 0.0f;

        // 定义一个字符串索引器
        public string this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        public void Update()
        {
            
        }

        public bool LoadDicFromStringData(string data)
        {
            try
            {
                int splitterLen = KEY_SPLITTER.Length;
                HashSet<string> keys = new ();

                // 这里用using是为了函数执行完后，自动释放stringreader资源
                using StringReader reader = new(data);

                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    int tempIndex = line.IndexOf(KEY_SPLITTER, StringComparison.Ordinal);
                    if (tempIndex < 0)
                    {
                        Debug.LogWarning($"[LoadTableFromStringData] : no find splitter ! {line}");
                        continue;
                    }
                    
                    string key = line.Substring(0, tempIndex).Trim();
                    string value = line.Substring(tempIndex + splitterLen); // 需要加上分隔符的长度

                    if (keys.Contains(key))
                    {
                        Debug.LogWarning($"[LoadTableFromStringData] : key is contains ! {line}");
                        continue;
                    }

                    keys.Add(key);
                    m_i18nDic[key] = new I18nTextItem(key, value);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[LoadTableFromStringData] : load fail ! {e.Message}");
                return false;
            }

            return true;
        }

        public bool ContainsKey(string key)
        {
            return m_i18nDic.ContainsKey(key);
        }

        public bool Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            
            m_i18nDic[key] = new I18nTextItem(key, value);
            
            return true;
        }

        public string Get(string key)
        {
            if (!ContainsKey(key))
            {
                return "";
            }
            return m_i18nDic[key].text;
        }
    }
}