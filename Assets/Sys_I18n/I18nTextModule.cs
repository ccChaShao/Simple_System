using System.Collections;
using System.Collections.Generic;
using System.Resource;
using System.Text;
using UnityEngine;

namespace System.I18n.RunTime
{
    public class I18nTextModule
    {
        public static I18nTextDic i18nTextDic;

        public static void Load()
        {
            
        }
        
        public static string GetSrcFile(string locale)
        {
            // return SrcRootDir + $"/{locale}/texts.bytes";
            return string.Empty;
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
    }
}
