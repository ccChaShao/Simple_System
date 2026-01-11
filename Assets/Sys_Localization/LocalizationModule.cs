namespace System.Localization
{
    [Serializable]
    public class LocalizationModule
    {
        private static LocalizationType m_localizationType;
        
        public static LocalizationType localizationType => m_localizationType;

        public void Initalize()
        {
            
        }
    }
}