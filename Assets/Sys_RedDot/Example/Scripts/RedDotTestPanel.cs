using System.Collections.Generic;
using System.RedDot.RunTime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace System.RedDot.Example
{
    public class RedDotTestPanel : MonoBehaviour
    {
        [BoxGroup("GM")] public TMP_InputField pathInputField;
        
        [BoxGroup("GM/Add - Dec")] public Button addButton;

        [BoxGroup("GM/Add - Dec")] public Button decButton;
        
        [BoxGroup("GM/Set")] public TMP_InputField setInputField;
        
        [BoxGroup("GM/Set")] public Button setButton;
        
        
        [BoxGroup("红点节点")] public List<RedDotBindData> testData;

        private Manager m_Manager;
        private RedDotModule m_RedDotModule;

        private void Awake()
        {
            addButton.onClick.AddListener(OnAddButtonClick);
            decButton.onClick.AddListener(OnDecButtonClick);
            setButton.onClick.AddListener(OnSetButtonClick);
        }

        private void Start()
        {
            m_Manager = Manager.Instance;
            m_RedDotModule = Manager.Instance.redDotModule;
            BindRedDotModule();
        }

        private void OnDestroy()
        {
            addButton.onClick.RemoveAllListeners();
            decButton.onClick.RemoveAllListeners();
        }

        private void BindRedDotModule()
        {
            foreach (var data in testData)
            {
                m_Manager.BindRedDot(data);
            }
        }

        private void OnAddButtonClick()
        {

            m_RedDotModule.AddOnceRedDotNodeCount(pathInputField.text);
        }

        private void OnDecButtonClick()
        {
            m_RedDotModule.DecOnceRedDotNodeCount(pathInputField.text);
        }

        private void OnSetButtonClick()
        {
            m_RedDotModule.SetRedDotNodeCount(pathInputField.text, int.Parse(setInputField.text));
        }

        [Button]
        private void RedDotDebugLog()
        {
            m_RedDotModule.RedDotModuleDebug();
        }
    }
}