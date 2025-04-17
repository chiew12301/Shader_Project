#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GranularApplyWindow : EditorWindow
{
    private static List<PlayModeSave.SavedComponent> savedComponents = new List<PlayModeSave.SavedComponent>();

    [MenuItem("Tools/KurumiC Playmode Save")]
    public static void ShowWindow()
    {
        GranularApplyWindow window = GetWindow<GranularApplyWindow>("KurumiC Playmode Save");
        window.Show();
    }

    public static void UpdateSaveComponents()
    {
        savedComponents = new List<PlayModeSave.SavedComponent>(PlayModeSave.savedComponents);
    }

    public static void ClearSaveComponents()
    {
        savedComponents.Clear();
    }

    private void OnGUI()
    {
        GUILayout.Label("Saved Components in Play Mode", EditorStyles.boldLabel);

        if (savedComponents.Count == 0)
        {
            GUILayout.Label("No saved components.");
            return;
        }

        foreach (var savedComponent in savedComponents)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label(savedComponent.targetObject.name + " - " + savedComponent.componentName);

            foreach (var field in savedComponent.fields)
            {
                GUILayout.Label($"{field.fieldName}: {field.fieldValue}");
            }

            GUILayout.EndVertical();
        }

        if(EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("You can apply changes only after exiting Play Mode.", MessageType.Info);     
            return;     
        }

        if (GUILayout.Button("Apply All Changes"))
        {
            PlayModeSave.ApplyChanges(savedComponents);
            savedComponents.Clear();
        }

        if (GUILayout.Button("Clear Saved Changes"))
        {
            savedComponents.Clear();
        }
    }
}
#endif