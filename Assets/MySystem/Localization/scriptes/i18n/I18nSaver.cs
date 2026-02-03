using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

namespace System.I18n.RunTime
{
    public class I18nSaver
    {
        public static void WriteDicToFile<TKey, TValue>(Dictionary<TKey, TValue> dic, string filePath)
        {
            string dicJsonStr = JsonMapper.ToJson(dic);

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, dicJsonStr);
        }

        public static Dictionary<TKey, TValue> ReadDicFromFile<TKey, TValue>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string dicJsonStr = File.ReadAllText(filePath);
                
                Dictionary<TKey, TValue> dic = JsonMapper.ToObject<Dictionary<TKey, TValue>>(dicJsonStr);

                return dic;
            }
            else
            {
                Debug.LogError("[ReadDicFromFile] : not exists ! " + filePath);
            }

            return null;
        }
    }
}