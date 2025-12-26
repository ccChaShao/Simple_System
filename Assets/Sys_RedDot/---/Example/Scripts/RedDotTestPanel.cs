/*
* @Author: Charlie
* @Email: 203981473@qq.com
* @Description: RedDotTestPanel class for testing RedDot functionality with UI buttons.
* @Date: 2025-04-16 11:04:40
* @Modify:
*/

using UnityEngine;
using UnityEngine.UI;

namespace DL.RedDot.Example
{
    public class RedDotTestPanel : MonoBehaviour
    {
        public Button task;
        public Button email;
        public Button message;


        public Button addtask;
        public Button addemail;
        public Button addmessage;

        public Button detask;
        public Button deemail;
        public Button demessage;


        private int m_TaskCount;
        int m_EmailCount;
        int m_MessageCount;

        private bool m_Init;
        
        private void Create()
        {
            if (m_Init) return;
            Bind();
            m_Init = true;
        }

        public void Show()
        {
            Create();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Bind()
        {
            Debug.Log("注册绑定");
            task.onClick.AddListener(() =>
            {
                m_TaskCount = 0;
                Manager.inst.DRedDotModule.ClearRedDotNode("root_task");
            });
            email.onClick.AddListener(() =>
            {
                m_EmailCount = 0;
                Manager.inst.DRedDotModule.ClearRedDotNode("root_email");
            });
            message.onClick.AddListener(() =>
            {
                m_MessageCount = 0;
                Manager.inst.DRedDotModule.ClearRedDotNode("root_message");
            });

            addtask.onClick.AddListener(() =>
            {
                m_TaskCount += 1;
                Manager.inst.DRedDotModule.UpdateRedDotCount("root_task", m_TaskCount);
            });
            addemail.onClick.AddListener(() =>
            {
                m_EmailCount += 1;
                Manager.inst.DRedDotModule.UpdateRedDotCountDelay("root_email", m_EmailCount);
            });
            addmessage.onClick.AddListener(() =>
            {
                m_MessageCount += 1;
                Manager.inst.DRedDotModule.UpdateRedDotCountDelay("root_message", m_MessageCount);
            });

            detask.onClick.AddListener(() =>
            {
                m_TaskCount -= 1;
                Manager.inst.DRedDotModule.UpdateRedDotCount("root_task", m_TaskCount);
            });
            deemail.onClick.AddListener(() =>
            {
                m_EmailCount -= 1;
                Manager.inst.DRedDotModule.UpdateRedDotCountDelay("root_email", m_EmailCount);
            });
            demessage.onClick.AddListener(() =>
            {
                m_MessageCount -= 1;
                Manager.inst.DRedDotModule.UpdateRedDotCountDelay("root_message", m_MessageCount);
            });
        }
        

    }
}
