#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor.Search;

namespace KC_Custom
{
    public class KCUnuseAssetsCleaner : EditorWindow
    {
        private Vector2 m_scrollPos;
        private List<string> m_unusedAssets = new List<string>();
        private Dictionary<string, bool> m_selectedAssets = new Dictionary<string, bool>();
        private bool m_isScanning = false;
        private bool m_isDeleting = false;
        private bool m_selectAll = false;
        private string m_searchQuery = "";

        //====================================================

        [MenuItem("Tools/KC Delete Unused Assets")]
        public static void ShowWindow()
        {
            var window = GetWindow<KCUnuseAssetsCleaner>("KC Unused Assets Cleaner");
            window.ScanForUnusedAssets();
        }

        private void OnGUI()
        {
            if(this.m_isScanning)
            {
                EditorGUILayout.LabelField("Scanning assets...");
                return;
            }

            if(this.m_isDeleting)
            {
                EditorGUILayout.LabelField("Removing selected assets...");
                return;
            }

            if(this.m_unusedAssets.Count <= 0)
            {
                EditorGUILayout.LabelField("No unused assets found.");
                if(GUILayout.Button("Rescan"))
                {
                    this.ScanForUnusedAssets();
                }
                return;
            }

            EditorGUILayout.LabelField("Search", EditorStyles.boldLabel);
            string newSearch = EditorGUILayout.TextField(this.m_searchQuery);
            if (newSearch != this.m_searchQuery)
            {
                this.m_searchQuery = newSearch;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Unused Assets Found: " + this.m_unusedAssets.Count);
            EditorGUILayout.Space();

            if (this.m_unusedAssets.Count > 2)
            {
                EditorGUILayout.LabelField("Bulk Actions", EditorStyles.boldLabel);
                bool newSelectAll = EditorGUILayout.ToggleLeft("Select All", this.m_selectAll);
                if (newSelectAll != this.m_selectAll)
                {
                    this.m_selectAll = newSelectAll;
                    foreach (var key in this.m_unusedAssets)
                    {
                        this.m_selectedAssets[key] = this.m_selectAll;
                    }
                }
                EditorGUILayout.Space();
            }

            this.m_scrollPos = EditorGUILayout.BeginScrollView(this.m_scrollPos);
            foreach (var asset in this.m_unusedAssets)
            {
                if (!IsAssetVisible(asset))
                    continue;

                this.m_selectedAssets[asset] = EditorGUILayout.ToggleLeft(asset, this.m_selectedAssets[asset]);
            }
            EditorGUILayout.EndScrollView();


            EditorGUILayout.Space();
            if (GUILayout.Button("Delete Selected"))
            {
                DeleteSelectedAssets();
            }

            if (GUILayout.Button("Rescan"))
            {
                ScanForUnusedAssets();
            }
        }

        //====================================================

        private bool IsAssetVisible(string path)
        {
            if (string.IsNullOrEmpty(this.m_searchQuery))
                return true;

            return path.ToLower().Contains(this.m_searchQuery.ToLower());
        }

        private void ScanForUnusedAssets()
        {
            this.m_unusedAssets.Clear();
            this.m_selectedAssets.Clear();
            this.m_isScanning = true;
            Repaint();
            EditorCoroutineUtility.StartCoroutineOwnerless(ScanCoroutine());
        }

        private IEnumerator ScanCoroutine()
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            int count = allAssets.Length;
            for (int i = 0; i < count; i++)
            {
                string path = allAssets[i];

                if (!path.StartsWith("Assets") || Directory.Exists(path)) continue;

                // Check if it's used
                string[] deps = AssetDatabase.GetDependencies(path, true);

                bool isUsed = false;
                foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                {
                    if (!scene.enabled) continue;
                    string[] sceneDeps = AssetDatabase.GetDependencies(scene.path, true);
                    if (System.Array.Exists(sceneDeps, s => s == path))
                    {
                        isUsed = true;
                        break;
                    }
                }

                if (!isUsed && deps.Length == 1) // only depends on itself
                {
                    this.m_unusedAssets.Add(path);
                    this.m_selectedAssets[path] = false;
                }

                if (i % 20 == 0)
                {
                    EditorUtility.DisplayProgressBar("Scanning for Unused Assets", $"Checking {path}", (float)i / count);
                    yield return null;
                }
            }

            EditorUtility.ClearProgressBar();
            this.m_isScanning = false;
            Repaint();
        }

        private void DeleteSelectedAssets()
        {
            this.m_isDeleting = true;
            Repaint();
            EditorCoroutineUtility.StartCoroutineOwnerless(DeleteCoroutine());
        }

        private IEnumerator DeleteCoroutine()
        {
            List<string> toDelete = new List<string>();
            foreach (var kvp in this.m_selectedAssets)
            {
                if (kvp.Value)
                {
                    toDelete.Add(kvp.Key);
                }
            }

            int count = toDelete.Count;
            for (int i = 0; i < count; i++)
            {
                string path = toDelete[i];
                bool result = AssetDatabase.DeleteAsset(path);
                if (result)
                    Debug.Log($"Deleted: {path}");
                else
                    Debug.LogWarning($"Failed to delete: {path}");

                EditorUtility.DisplayProgressBar("Deleting Assets", $"Deleting {path}", (float)i / count);
                yield return null;
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();

            // Refresh list
            foreach (string path in toDelete)
            {
                this.m_unusedAssets.Remove(path);
                this.m_selectedAssets.Remove(path);
            }

            this.m_isDeleting = false;
            Repaint();
        }

        //====================================================
    }
}
#endif