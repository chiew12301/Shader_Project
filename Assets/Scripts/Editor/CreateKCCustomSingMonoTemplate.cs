using UnityEditor;
using UnityEngine;
using System.IO;

namespace KC_Custom
{
    public static class CreateKCCustomSingMonoTemplate
    {
        private const string templateFormat =
            @"using UnityEngine;
using KC_Custom;

namespace KC_Custom
{
    public class #SCRIPTNAME# : MonobehaviourSingleton<#SCRIPTNAME#>
    {
        //Variables

        //===========================================================

        //Override Functions
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        } 


        //===========================================================

        //Other Functions

        //===========================================================
    }
}";

        private static bool suppressAutoOpen = false;

        [MenuItem("Assets/Create/KC_CUSTOM Sing Mono Template", false, 80)]
        private static void CreateCustomScript()
        {
            string selectedPath = GetSelectedPath();

            if(string.IsNullOrEmpty(selectedPath))
            {
                Debug.LogError("Unable to determine the selected path.");
                return;
            }

            string defaultName = "NewScript";
            string filePath = Path.Combine(selectedPath, $"{defaultName}.cs");
            filePath = AssetDatabase.GenerateUniqueAssetPath(filePath);

            File.WriteAllText(filePath, templateFormat.Replace("#SCRIPTNAME#", defaultName));
            AssetDatabase.Refresh();

            Object newAsset = AssetDatabase.LoadAssetAtPath<Object>(filePath);
            Selection.activeObject = newAsset;
            EditorUtility.FocusProjectWindow();
            EditorApplication.delayCall += () =>
            {
                suppressAutoOpen = false; // Re-enable auto-open
                EditorApplication.ExecuteMenuItem("Assets/Rename");
            };
        }

        [UnityEditor.Callbacks.OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            if (suppressAutoOpen)
            {
                return true;
            }
            return false;
        }

        private static string GetSelectedPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
                return "Assets";

            if (Directory.Exists(path))
                return path;

            return Path.GetDirectoryName(path);
        }
    }
}