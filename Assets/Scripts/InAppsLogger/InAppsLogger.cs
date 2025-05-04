using System.Collections.Generic;
using UnityEngine;

namespace KC_Custom
{
    public class InAppsLogger : MonoBehaviour
    {
        [SerializeField] private Font m_textFont = null;
        private List<string> m_logMessages = new List<string>();
        private Vector2 m_scrollPosition = Vector2.zero;
        private Rect m_windowRect = new Rect(10, 10, 500, 300);
        private GUIStyle m_logStyle;

        //=========================================

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void OnGUI()
        {
            if(this.m_logStyle == null)
            {
                this.InitializeGUIStyle();
            }

            this.m_windowRect.x = Mathf.Clamp(this.m_windowRect.x, 0, Screen.width - this.m_windowRect.width);
            this.m_windowRect.y = Mathf.Clamp(this.m_windowRect.y, 0, Screen.height - this.m_windowRect.height);

            this.m_windowRect = GUI.Window(123456, this.m_windowRect, this.DrawWindow, "Logger");
        }

        //=========================================

        private void InitializeGUIStyle()
        {
            this.m_logStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 30,
                wordWrap = true,
                font = this.m_textFont != null ? this.m_textFont : GUI.skin.font
            };
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            this.m_logMessages.Add(logString);
            if(this.m_logMessages.Count > 100)
            {
                this.m_logMessages.RemoveAt(0);
            }
        }
        
        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();
            this.m_scrollPosition = GUILayout.BeginScrollView(this.m_scrollPosition, GUILayout.Width(this.m_windowRect.width - 20), GUILayout.Height(this.m_windowRect.height - 40));
            foreach(string msg in this.m_logMessages)
            {
                GUILayout.Label(msg, this.m_logStyle);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, this.m_windowRect.width, 20));
        }

        //=========================================
    }
}