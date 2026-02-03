using System.Collections.Generic;

namespace System.I18n.RunTime
{
    public struct I18nJsonData
    {
        public string MD5;
        public Dictionary<string, I18nTextItem> i18nDic;
    }
    
    public struct I18nTextItem
    {
        public string key;
        public string text;

        public I18nTextItem(string key, string text)
        {
            this.key = key;
            this.text = text;
        }
    }
}