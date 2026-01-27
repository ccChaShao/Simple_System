using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Localization;
using System.Resource;
using System.Text;
using LitJson;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace System.I18n.RunTime
{
    [Serializable]
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

        public void Initalize()
        {
            ReLoad();
        }

        public void Update()
        {
            if (m_i18nTextDic != null)
            {
                m_i18nTextDic.Update();
            }
        }
        
        public static void EnsureLoad()
        {
            if (m_i18nTextDic == null)
            {
                ReLoad();
            }
        }
        
        public static void ReLoad()
        {
            m_i18nTextDic = new();
            LocalizationSettingItem item = LocalizationModule.GetLocalizationSettingItem();
            if (!item)
            {
                Debug.LogWarning("[ReLoad] : 暂不支持该目标语言类型。");
                return;
            }
            
            string srcFileAssetsPath = GetSrcFilePath(item.languageFile);
            
            // string srcFileJsonPath = Path.Combine(Application.persistentDataPath, $"{item.languageFile}.json");
            // string srcFilePath = Path.Combine(Application.dataPath, srcFileAssetsPath);
            // string srcFileMD5 = MD5FileValidator.ComputeFileMD5(srcFilePath);
            
            // // 先检查本地JSON
            // if (File.Exists(srcFileJsonPath))
            // {
            //     string dicJsonStr = File.ReadAllText(srcFileJsonPath);
            //     I18nJsonData jsonData = JsonMapper.ToObject<I18nJsonData>(dicJsonStr);
            //     bool isValid = string.Equals(jsonData.MD5, srcFileMD5, StringComparison.OrdinalIgnoreCase);
            //     if (isValid)
            //     {
            //         Debug.Log("charsiew : [ReLoad] : ------------------------ MD5检查通过。");
            //         m_i18nTextDic.SetI18nDic(jsonData.i18nDic);
            //         return;
            //     }
            // }
            
            // 重新解析bytes文件
            string rawSourceText = LoadRawSourceText(srcFileAssetsPath);
            if (rawSourceText != null)
            {
                bool loadSuc = m_i18nTextDic.LoadDicFromStringData(rawSourceText);
                Debug.Log("charsiew : [ReLoad] : ---------------------- \n" + rawSourceText);
                // if (loadSuc)
                // {
                //     try
                //     {
                //         string directory = Path.GetDirectoryName(srcFileJsonPath);
                //         if (!Directory.Exists(directory))
                //         {
                //             Directory.CreateDirectory(directory);
                //         }
                //
                //         I18nJsonData jsonData = new I18nJsonData();
                //         jsonData.MD5 = srcFileMD5;
                //         jsonData.i18nDic = m_i18nTextDic.i18nDic;
                //         
                //         StringBuilder sb = new StringBuilder();
                //         JsonWriter writer = new JsonWriter(sb);
                //         
                //         // 关键设置：启用美化打印
                //         writer.PrettyPrint = true;
                //         // 可选设置：自定义缩进（例如使用2个空格）
                //         writer.IndentValue = 2;
                //         
                //         JsonMapper.ToJson(jsonData, writer);
                //
                //         File.WriteAllText(srcFileJsonPath, sb.ToString(), Encoding.UTF8);
                //     }
                //     catch (Exception e)
                //     {
                //         Debug.LogError(e.Message);
                //         throw;
                //     }
                // }
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

        public static string LoadRawSourceText(TextAsset textAsset)
        {
            if (textAsset != null)
            {
                Encoding.UTF8.GetString(textAsset.bytes);
            }

            return string.Empty;
        }

        [Button("获取文本",ButtonSizes.Large)]
        public static string GetText(string key)
        {
            EnsureLoad();
            return m_i18nTextDic[key];
        }

        [Button("设置文本",ButtonSizes.Large)]
        public static void SetText(string key, string value)
        {
            EnsureLoad();
            m_i18nTextDic[key] = value;
        }
    }
}
