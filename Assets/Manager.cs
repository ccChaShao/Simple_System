using System.I18n.RunTime;
using System.Localization;
using MySystem.RedDot.RunTime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using MySystem.RedDot.Example;

public class Manager : MonoBehaviour
{
    private static Manager m_Instance;
    public static Manager Instance => m_Instance;
    
    [BoxGroup("RedDot")] public RedDotModule redDotModule;
        
    [BoxGroup("RedDot")] public GameObject redDotPrefab;
    
    [BoxGroup("I18n")] public I18nTextModule i18nTextModule;

    [BoxGroup("Localization")] public LocalizationModule localizationModule;
    
    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        
        InitModule();
    }

    private void Update()
    {
        UpdateModule();
        
        InitTextMeshPro();
    }

    private void InitModule()
    {
        redDotModule = new ();
        redDotModule.Initalize();
        i18nTextModule = new ();
        i18nTextModule.Initalize();
        localizationModule = new();
        localizationModule.Initalize();
    }

    private void UpdateModule()
    {
        if (redDotModule != null)
            redDotModule.Update();
        if (i18nTextModule != null)
            i18nTextModule.Update();
    }

    //TODO 这部分应该放在游戏业务逻辑管理器中。
    public void BindRedDot(RedDotBindData dotBindData)
    {
        // 创建数据
        RedDotNode node = redDotModule.GerOrCreateRedDotNode(dotBindData.path, false);
        // 创建红点
        RedDotItem item = dotBindData.rectTransform.GetComponentInChildren<RedDotItem>(); // 仅在该节点查询
        if (item == null)
        {
            GameObject ins = Instantiate(redDotPrefab, dotBindData.rectTransform);
            item = ins.GetComponent<RedDotItem>();
        }
        item.SetData(dotBindData.path, dotBindData.type, dotBindData.anchorPreset);
        item.RefreshView(node);
    }

    private static void InitTextMeshPro()
    {
        TMP_Text.i18nGetFunc = i18nKey =>
        {
            string i18nVal = I18nTextModule.i18nTextDic[i18nKey];
            if (!string.IsNullOrEmpty(i18nVal))
            {
                return i18nVal;
            }
            return i18nKey;
        };
    }

    private static void OnSystemLanguageChanged()
    {
        // 重新解析字库
        I18nTextModule.ReLoad();
        // 字体重新加载
        TMPro_EventManager.SYSTEM_LANGUAGE_CHANGED_EVENT.Call();
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        InitTextMeshPro();

        LocalizationModule.settings.OnLocalizationTypeChanged -= OnSystemLanguageChanged;
        LocalizationModule.settings.OnLocalizationTypeChanged += OnSystemLanguageChanged;
        
        Debug.Log("========编辑器事件注册完成========");
    }
#endif
}
