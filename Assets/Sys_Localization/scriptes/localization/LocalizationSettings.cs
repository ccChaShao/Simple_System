using System;
using System.Collections.Generic;
using System.Resource;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MySystem.Localization
{
    [CreateAssetMenu(menuName = "System/Localization/CreateSetting")]
    public class LocalizationSettings : ScriptableObject
    {
        static LocalizationSettings _instance;
        public static LocalizationSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetOrCreateSettings();
                }
                return _instance;
            }
            private set => _instance = value;
        }
        
        [LabelText("当前语言类型"), OnValueChanged(nameof(LocalizationTypeChanged))]
        public SystemLanguage localizationType;
        
        [LabelText("语言配置") ,ListDrawerSettings]
        public List<LocalizationSettingItem> localeSettings = new();
        
        public LocalizationSettingItem localizationItem => GetLacalizationItem(localizationType);
        
        public Action OnLocalizationTypeChanged = null;
        
        private static string SRC_ROOT_DIR = "config/localization";

        private static string SETTING_PATH => $"{SRC_ROOT_DIR}/Localization_Settings.asset";
        
        private static LocalizationSettings GetOrCreateSettings()
        {
            LocalizationSettings settings = ResourceManager.LoadAsset<LocalizationSettings>(SETTING_PATH);

            if (!settings)
            {
                settings = CreateInstance<LocalizationSettings>();
                
                LocalizationSettingItem settingItem = CreateInstance<LocalizationSettingItem>();
                settingItem.language = SystemLanguage.Chinese;
                settingItem.languageName = "中文";
                settingItem.languageFile = "zh_cn";
                
                settings.localeSettings.Add(settingItem);
            }
        
            return settings;
        }

        public LocalizationSettingItem GetLacalizationItem(SystemLanguage type)
        {
            for (int i = 0; i < localeSettings.Count; i++)
            {
                var item = localeSettings[i];
                if (item.language == type)
                {
                    return item;
                }
            }

            return null;
        }

        public void LocalizationTypeChanged()
        {
            OnLocalizationTypeChanged?.Invoke();
        }
    }
}