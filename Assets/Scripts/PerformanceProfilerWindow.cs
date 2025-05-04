using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;

namespace KC_CUSTOM
{
    public class PerformanceProfilerWindow : MonoBehaviour
    {
        [Header("Set Display")]
        [SerializeField] private bool m_activeDisplay = true;

        private float m_deltaTime = 0.0f;
        private Vector2 m_scrollPosition;
        private Rect m_windowRect = new Rect(10, 10, 150, 220);

        //===================================================
        private void Update()
        {
            // Update deltaTime
            this.m_deltaTime += (Time.unscaledDeltaTime - this.m_deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            if(!this.m_activeDisplay) return;
            this.m_windowRect = GUI.Window(0, this.ClampToScreen(this.m_windowRect), this.DrawWindow, "KC Game Stats");
        }

        private void DrawWindow(int windowID)
        {
            // Create a style for the text
            GUIStyle style = new GUIStyle();
            style.fontSize = 8;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperLeft;

            // Calculate FPS
            float fps = 1.0f / this.m_deltaTime;
            GUI.Label(new Rect(10, 20, 20, 20), "FPS: " + Mathf.Ceil(fps).ToString(), style);

            // Get approximate CPU usage
            float cpuUsage = this.GetCpuUsage();
            GUI.Label(new Rect(10, 40, 20, 20), "CPU Usage (approx): " + cpuUsage.ToString("F2") + "%", style);

            // Get memory usage
            float memoryUsage = this.GetMemoryUsage();
            GUI.Label(new Rect(10, 60, 20, 20), "Memory Usage: " + memoryUsage.ToString("F2") + " MB", style);

            // Get total allocated memory
            float totalAllocatedMemory = this.GetTotalAllocatedMemory();
            GUI.Label(new Rect(10, 80, 20, 20), "Allocated Memory: " + totalAllocatedMemory.ToString("F2") + " MB", style);

            // Display Batches, Tris, Verts, Set Pass Calls, and Shadow Casters
            GUI.Label(new Rect(10, 100, 20, 20), "Batches: " + UnityStats.batches, style);
            GUI.Label(new Rect(10, 120, 20, 20), "Tris: " + UnityStats.triangles, style);
            GUI.Label(new Rect(10, 140, 20, 20), "Verts: " + UnityStats.vertices, style);
            GUI.Label(new Rect(10, 160, 20, 20), "Set Pass Calls: " + UnityStats.setPassCalls, style);
            GUI.Label(new Rect(10, 180, 20, 20), "Shadow Casters: " + UnityStats.shadowCasters, style);

            // Make the window draggable
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        //===================================================

        private float GetCpuUsage()
        {
            // Approximate CPU usage based on frame time
            return Mathf.Clamp((1.0f / this.m_deltaTime) / SystemInfo.processorCount * 100, 0, 100);
        }

        private float GetMemoryUsage()
        {
            // Get used memory
            return Profiler.GetTotalReservedMemoryLong() / (1024 * 1024);
        }

        private float GetTotalAllocatedMemory()
        {
            // Get total allocated memory
            return Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
        }

        // Helper method to clamp window position to screen
        private Rect ClampToScreen(Rect rect)
        {
            rect.x = Mathf.Clamp(rect.x, 0, Screen.width - rect.width);
            rect.y = Mathf.Clamp(rect.y, 0, Screen.height - rect.height);
            return rect;
        }
    }
}