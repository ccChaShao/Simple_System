using System.Collections;
using System.Collections.Generic;
using System.Resource;
using System.Text;
using UnityEngine;

namespace System.I18n.RunTime
{
    public class I18nTextModule
    {
        public const string SRC_ROOT_DIR = "config/i18n";
            
        private static I18nTextDic m_i18nTextDic;
        
        public static I18nTextDic i18nTextDic
        {
            get
            {
                EnsureLoad();
                return m_i18nTextDic;
            }
        }

        public static void OnTick()
        {
            if (m_i18nTextDic != null)
            {
                m_i18nTextDic.OnTick();
            }
        }
        
        public static void EnsureLoad()
        {
            if (m_i18nTextDic == null)
            {
                Load();
            }
        }
        public static void Load()
        {
            m_i18nTextDic = new();
            string srcFilePath = GetSrcFilePath("zh_cn");   //TODO 这里临时用zh_cn作为测试用
            string rawSourceText = LoadRawSourceText(srcFilePath);
            if (rawSourceText != null)
            {
                bool loadSuc = m_i18nTextDic.LoadDicFromStringData(srcFilePath);
            }
            else
            {
                Debug.LogError("[Load] : I18nTextModule LoadRawSourceText failed !");
            }
        }
        
        public static string GetSrcFilePath(string locale)
        {
            return SRC_ROOT_DIR + $"/{locale}/texts.bytes";
        }

        public static string LoadRawSourceText(string path)
        {
            TextAsset textAsset = ResourceManager.LoadAsset<TextAsset>(path);
            if (textAsset != null)
            {
                // 当文本资源的实际编码与Unity默认解析不同时，直接读取 TextAsset.text会产生乱码。通过 TextAsset.bytes获取原始字节数据，再使用正确的编码（如 Encoding.UTF8）进行手动解码，可以确保内容准确无误；
                return Encoding.UTF8.GetString(textAsset.bytes);
            }

            return String.Empty;
        }

        public static string GetText(string key)
        {
            EnsureLoad();
            return m_i18nTextDic[key];
        }

        public static void SetText(string key, string value)
        {
            EnsureLoad();
            m_i18nTextDic[key] = value;
        }
    }
}
