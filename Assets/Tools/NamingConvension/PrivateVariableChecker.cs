using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class PrivateVariableChecker : EditorWindow
{
    [MenuItem("Tools/Check Private Variables")]
    public static void ShowWindow()
    {
        GetWindow<PrivateVariableChecker>("Check Private Variables");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Check and Fix Private Variables"))
        {
            CheckAndFixPrivateVariables();
        }
    }

    private static void CheckAndFixPrivateVariables()
    {
        string[] scripts = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        foreach (string script in scripts)
        {
            string content = File.ReadAllText(script);
            string updatedContent = FixPrivateVariableNames(content);

            if (content != updatedContent)
            {
                File.WriteAllText(script, updatedContent);
                Debug.Log($"Updated private variable names in {script}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Private variable check and fix complete.");
    }

    private static string FixPrivateVariableNames(string content)
    {
        // This regex matches private variables that don't start with "m_"
        Regex privateVariableRegex = new Regex(@"private\s+[^\s]+\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*;");
        MatchCollection matches = privateVariableRegex.Matches(content);
        foreach (Match match in matches)
        {
            string variableName = match.Groups[1].Value;
            if (!variableName.StartsWith("m_"))
            {
                string newVariableName = "m_" + variableName;
                content = Regex.Replace(content, $@"\b{variableName}\b", newVariableName);
            }
        }
        return content;
    }
}