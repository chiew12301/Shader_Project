using UnityEditor;
using UnityEngine;
using System.IO;

public class SingletonScriptGenerator : EditorWindow
{
    private string scriptName = "NewSingletonClass";

    [MenuItem("Tools/Create Singleton Script")]
    public static void ShowWindow()
    {
        GetWindow<SingletonScriptGenerator>("Create Singleton Script");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a new Singleton Script", EditorStyles.boldLabel);
        scriptName = EditorGUILayout.TextField("Script Name", scriptName);

        if (GUILayout.Button("Create Script"))
        {
            CreateSingletonScript(scriptName);
        }
    }

    private void CreateSingletonScript(string scriptName)
    {
        string directoryPath = "Assets/Scripts"; // Modify this path as needed
        string filePath = Path.Combine(directoryPath, scriptName + ".cs");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(filePath))
        {
            Debug.LogError("Script already exists. Choose a different name.");
            return;
        }

        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine("using KC_Custom;");
            writer.WriteLine();
            writer.WriteLine("public class " + scriptName + " : MonobehaviourSingleton<" + scriptName + ">");
            writer.WriteLine("{");
            writer.WriteLine("    // Add your singleton logic here");
            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
        Debug.Log("Script created at " + filePath);
    }
}
