using Sirenix.OdinInspector;
using UnityEngine;

namespace System.Localization
{
    [CreateAssetMenu(menuName = "System/Localization/CreateSettingItem")]
    public class LocalizationSettingItem : ScriptableObject
    {
        [LabelText("语言类型")] public LocalizationType localizationType;

        [LabelText("语言显示名称")] public string localizationName;
        
        [LabelText("文件名称")] public string localizationFile;
    }
}