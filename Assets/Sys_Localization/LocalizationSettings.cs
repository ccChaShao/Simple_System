using System.Collections.Generic;
using System.Resource;
using Sirenix.OdinInspector;
using UnityEngine;

namespace System.Localization
{
    [CreateAssetMenu(menuName = "System/Localization/CreateSetting")]
    public class LocalizationSettings : ScriptableObject
    {
        private static LocalizationSettings m_instance;
        
        public static LocalizationSettings Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = GetOrCreateSettings();
                }
                return m_instance;
            }
        }
        
        [LabelText("默认语言类型")]
        public LocalizationType defaultLocalizationType;

        public LocalizationSettingItem defaultLocalizationItem => GetLacalizationItem(defaultLocalizationType);
        
        [LabelText("语言配置") ,ListDrawerSettings]
        public List<LocalizationSettingItem> localeSettings = new();
        
        private const string SRC_ROOT_DIR = "config/localization";

        private static string SETTING_PATH => $"{SRC_ROOT_DIR}/Localization_Settings.asset";


        public static LocalizationSettings GetOrCreateSettings()
        {
            var setting = ResourceManager.LoadAsset<LocalizationSettings>(SETTING_PATH);
            //TODO 这里应该加一个自动创建的处理

            return setting;
        }

        public LocalizationSettingItem GetLacalizationItem(LocalizationType type)
        {
            for (int i = 0; i < localeSettings.Count; i++)
            {
                var item = localeSettings[i];
                if (item.localizationType == type)
                {
                    return item;
                }
            }

            return null;
        }
    }
}