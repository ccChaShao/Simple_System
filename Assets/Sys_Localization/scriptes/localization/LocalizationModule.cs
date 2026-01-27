using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace System.Localization
{
    [Serializable]
    public class LocalizationModule
    {
        public static LocalizationSettings settings => LocalizationSettings.Instance;

        public void Initalize()
        {
            
        }

        public static LocalizationSettingItem GetLocalizationSettingItem()
        {
            if (!settings)
            {
                return null;
            }
            
            return settings.localizationItem;
        }

        public static LocalizationSettingItem GetLocalizationSettingItem(SystemLanguage localizationType)
        {
            if (!settings)
            {
                return null;
            }

            return settings.GetLacalizationItem(localizationType);
        }
    }
}