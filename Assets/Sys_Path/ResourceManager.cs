using System.IO;
using UnityEditor;
using UnityEngine.Device;

namespace System.Resource
{
    public class ResourceManager
    {
        public static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            //TODO 之后这里要宏编译区分编辑器 / 移动端 
            return LoadAsset_Editor<T>(path);
        }
        
        public static T LoadAsset_Editor<T>(string path) where T : UnityEngine.Object
        {
            string fullPath = Path.GetFullPath(Application.dataPath + "/../" + path);
            if (File.Exists(fullPath))
            {
                return AssetDatabase.LoadAssetAtPath<T>(fullPath);
            }

            return null;
        }
    }
}