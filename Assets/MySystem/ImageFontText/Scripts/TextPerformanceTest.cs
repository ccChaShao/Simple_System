using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MySystem.ImageFont
{
    enum TestMode
    {
        None,
        Text,
        TextMeshProUGUI,
        MeshText,
    }
    
    public class TextPerformanceTest : MonoBehaviour
    {
        [Header("测试环境设置")]
        public int textCount = 1000;
        public float spawnInterval = 0.1f;
        public float testDuration = 10.0f;
        public float textMoveSpeed = 50.0f;
        public Camera mainCamera;
        public RectTransform insContent;

        [Header("预制体引用")] 
        public GameObject textPrefab;
        public GameObject textMeshProPrefab;
        public GameObject meshTextPrefab;

        [Header("资源引用")] 
        public List<ImageFontData> randomFontDatas = new();

        [Header("UI引用")] 
        public Slider countSlider;
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI countLabel;
        public Button textTestButton;
        public Button textMeshProTestButton;
        public Button meshTextTestButton;
        
        // 对应的对象池
        private List<GameObject> m_ActiveTexts = new();
        private Queue<GameObject> m_TextPool = new();
        private Queue<GameObject> m_TextMeshProPool = new();
        private Queue<GameObject> m_MeshTextPool = new();
        
        // 测试数据
        private float m_Fps;
        private float m_FrameTimer;
        private int m_FrameCount;
        private float m_TestStartTime;
        private float m_MinFPS = float.MaxValue; // 最大帧率
        private float m_AvgFPS = 0.0f; // 最小帧率
        private int m_TotalFrams = 0;
        private TestMode m_TestMode = TestMode.None;

        private void OnEnable()
        {
            countSlider.onValueChanged.AddListener(OnSliderValueChaged);
            textTestButton.onClick.AddListener(StartTextTest);
            textMeshProTestButton.onClick.AddListener(StartTextMeshProTest);
            meshTextTestButton.onClick.AddListener(StartMeshTextTest);
        }

        private void OnDisable()
        {
            countSlider.onValueChanged.RemoveAllListeners();
            textTestButton.onClick.RemoveAllListeners();
            textMeshProTestButton.onClick.RemoveAllListeners();
            meshTextTestButton.onClick.RemoveAllListeners();
        }

        private void Start()
        {
            OnSliderValueChaged(countSlider.value);
        }

        private void Update()
        {
            // 计算FPS
            CalculateFPS();
        
            // 更新活跃文本
            UpdateActiveTexts();
        }

        private void CalculateFPS()
        {
            m_FrameCount++;
            m_FrameTimer += Time.unscaledDeltaTime;

            // 每秒记录一次
            if (m_FrameTimer >= 1.0f)
            {
                m_Fps = m_FrameCount / m_FrameTimer;
                m_FrameCount = 0;
                m_FrameTimer = 0;

                if (m_TestMode != TestMode.None)
                {
                    UpdatePerformanceStats();
                }
            }
            
            UpdateResultText();
        }  

        private void UpdateResultText() {
            if (!resultText)
            {
                return;    
            }

            float avgFps = m_TestMode == TestMode.None ? 0 : m_AvgFPS;
            
            resultText.text = $"当前模式: {m_TestMode}\nFPS: {m_Fps:F1}\n最小FPS: {m_MinFPS:F1}\n平均FPS: {avgFps:F1}\n活动文本: {m_ActiveTexts.Count}";
        } 
    
        public void StartTextTest() {
            if (m_TestMode == TestMode.Text)
            {
                ClearAll();
                return;
            }
            StartCoroutine(RunTest(TestMode.Text, textPrefab, m_TextPool));
        }
    
        public void StartTextMeshProTest() {
            if (m_TestMode == TestMode.TextMeshProUGUI)
            {
                ClearAll();
                return;
            }
            StartCoroutine(RunTest(TestMode.TextMeshProUGUI, textMeshProPrefab, m_TextMeshProPool));
        }
    
        public void StartMeshTextTest() {
            if (m_TestMode == TestMode.MeshText)
            {
                ClearAll();
                return;
            }
            StartCoroutine(RunTest(TestMode.MeshText, meshTextPrefab, m_MeshTextPool));
        }
    
        private IEnumerator RunTest(TestMode mode, GameObject prefab, Queue<GameObject> pool) {
            // 清理之前的测试
            CleanupTest();
            // 重置性能数据
            ClearTestValue();
            
            // 数据更新
            m_TestMode = mode;
        
            // 分批生成文本
            for (int i = 0; i < textCount; i++) {
                SpawnText(prefab, pool, i);
                yield return new WaitForSeconds(spawnInterval);
            }
        
            // 运行测试一段时间
            while (Time.time - m_TestStartTime < testDuration) {
                yield return null;
            }
        
            // 清理测试
            CleanupTest();
            m_TestMode = TestMode.None;
        
            // 显示最终结果
            ShowTestResult(mode);
        }      
        
        private void SpawnText(GameObject prefab, Queue<GameObject> pool, int index) {
            GameObject textObj = GetFromPool(prefab, pool);
            RectTransform rt = textObj.GetComponent<RectTransform>();
            
            rt.anchoredPosition = new Vector2(
                Random.Range(-Screen.width/2, Screen.width/2),
                Random.Range(-Screen.height/2, 0)
            );
            textObj.SetActive(true);
        
            // 设置文本内容
            int damage = Random.Range(100, 99999);
            switch (m_TestMode) {
                case TestMode.Text:
                    textObj.GetComponent<Text>().text = damage.ToString();
                    break;
                case TestMode.TextMeshProUGUI:
                    textObj.GetComponent<TMP_Text>().text = damage.ToString();
                    break;
                case TestMode.MeshText:
                    int randomIndex = Random.Range(0, randomFontDatas.Count);
                    ImageFontData randomFontData = randomFontDatas[randomIndex];
                    textObj.GetComponent<ImageFontText>().text = damage.ToString();
                    textObj.GetComponent<ImageFontText>().fontData = randomFontData;
                    break;
            }
        
            m_ActiveTexts.Add(textObj);
        }  
        
        private void UpdateActiveTexts() {
            for (int i = m_ActiveTexts.Count - 1; i >= 0; i--) {
                GameObject textObj = m_ActiveTexts[i];
                RectTransform rt = textObj.GetComponent<RectTransform>();
            
                // 移动文本
                rt.anchoredPosition = new(rt.anchoredPosition.x, rt.anchoredPosition.y + textMoveSpeed * Time.deltaTime);
                
                // 检查是否移出屏幕
                // Vector3 screenPos = mainCamera.WorldToViewportPoint(textObj.transform.position);
                if (rt.anchoredPosition.y > Screen.height * 1.2 / 2) {
                    m_ActiveTexts.RemoveAt(i);
                    ReturnToPool(textObj);
                }
            }
        }      
        
        private GameObject GetFromPool(GameObject prefab, Queue<GameObject> pool) {
            if (pool.Count > 0) {
                return pool.Dequeue();
            }

            return Instantiate(prefab, Vector3.zero, Quaternion.identity, insContent);
        }
        
        private void ReturnToPool(GameObject obj) {
            obj.SetActive(false);
            obj.transform.position = Vector3.zero;
        
            switch (m_TestMode) {
                case TestMode.Text:
                    m_TextPool.Enqueue(obj);
                    break;
                case TestMode.TextMeshProUGUI:
                    m_TextMeshProPool.Enqueue(obj);
                    break;
                case TestMode.MeshText:
                    m_MeshTextPool.Enqueue(obj);
                    break;
            }
        }
        
        private void CleanupTest() {
            
            foreach (var textObj in m_ActiveTexts) {
                Destroy(textObj);
            }
            m_ActiveTexts.Clear();
        
            ClearPool(m_TextPool);
            ClearPool(m_TextMeshProPool);
            ClearPool(m_MeshTextPool);
            
            void ClearPool(Queue<GameObject> pool) {
                while (pool.Count > 0) {
                    Destroy(pool.Dequeue());
                }
            }
        }

        private void ClearTestValue()
        {
            m_TestMode = TestMode.None;
            m_MinFPS = float.MaxValue;
            m_AvgFPS = 0f;
            m_TotalFrams = 0;
            m_TestStartTime = Time.time;
        }

        private void ClearAll()
        {
            CleanupTest();
            ClearTestValue();
            StopAllCoroutines();
        }

        private void UpdatePerformanceStats()
        {
            m_TotalFrams++;
            
            // 平均帧率记录
            m_AvgFPS = ((m_AvgFPS * (m_TotalFrams - 1)) + m_Fps) / m_TotalFrams;
            
            // 最小帧率记录
            m_MinFPS = (m_MinFPS > m_Fps) ? m_Fps : m_MinFPS;
        }
        
        private void ShowTestResult(TestMode mode) {
            
            string result = $"{mode} 测试结果:\n" +
                            $"平均FPS: {m_AvgFPS:F2}\n" +
                            $"最低FPS: {m_MinFPS:F2}\n" +
                            $"峰值内存: {System.GC.GetTotalMemory(false) / 1024 / 1024:F2}MB";
        
            if (resultText != null) 
                resultText.text = result;
        
            UnityEngine.Debug.Log(result);
        }

        private void OnSliderValueChaged(float val)
        {
            textCount = Mathf.RoundToInt(val);
            if (countLabel != null) {
                countLabel.text = $"文本数量: {textCount}";
            }
        }
    }
}
