using Sirenix.OdinInspector;
using UnityEngine;

namespace MySystem.Localization
{
    [CreateAssetMenu(fileName = "New Localization Item", menuName = "MySystem/Localization/CreateSettingItem")]
    public class LocalizationSettingItem : ScriptableObject
    {
        [LabelText("类型")] public SystemLanguage language;

        [LabelText("显示名称")] public string languageName;
        
        [LabelText("文件名称")] public string languageFile;
    }
}