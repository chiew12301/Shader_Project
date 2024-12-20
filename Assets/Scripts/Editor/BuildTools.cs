#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Unity.EditorCoroutines.Editor;

namespace BUILDTOOLS
{
    public class BuildTools : EditorWindow
    {
        private string m_fileName = "Default";
        private string m_customFilePath = "Default";
        private bool m_useCustomFileName = false;
        private bool m_useCustomFilePath = false;

        [MenuItem("Tools/Build Tools")]
        public static void OnShowTools()
        {
            const int width = 800;
            const int height = 800;

            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;

            EditorWindow.GetWindow<BuildTools>("KC Custom Build Tools").position = new Rect(x, y, width, height);;
        }

        private BuildTargetGroup GetTargetGroupForTarget(BuildTarget target) => target switch
        {
            BuildTarget.StandaloneOSX => BuildTargetGroup.Standalone,
            BuildTarget.StandaloneWindows => BuildTargetGroup.Standalone,
            BuildTarget.iOS => BuildTargetGroup.iOS,
            BuildTarget.Android => BuildTargetGroup.Android,
            BuildTarget.StandaloneWindows64 => BuildTargetGroup.Standalone,
            BuildTarget.WebGL => BuildTargetGroup.WebGL,
            BuildTarget.StandaloneLinux64 => BuildTargetGroup.Standalone,
            _ => BuildTargetGroup.Unknown
        };

        private Dictionary<BuildTarget, bool> m_targetsToBuild = new Dictionary<BuildTarget, bool>();
        private List<BuildTarget> m_availableTarget = new List<BuildTarget>();

        private void OnEnable()
        {
            this.m_availableTarget.Clear();
            var buildTargets = System.Enum.GetValues(typeof(BuildTarget));
            foreach(BuildTarget btvalue in buildTargets)
            {
                if(!BuildPipeline.IsBuildTargetSupported(GetTargetGroupForTarget(btvalue), btvalue))continue;

                this.m_availableTarget.Add(btvalue);

                if(!this.m_targetsToBuild.ContainsKey(btvalue)) this.m_targetsToBuild[btvalue] = false;
            }

            if(this.m_targetsToBuild.Count > this.m_availableTarget.Count)
            {
                List<BuildTarget> targetsToRemove = new List<BuildTarget>();
                foreach(BuildTarget targets in this.m_targetsToBuild.Keys)
                {
                    if(!this.m_availableTarget.Contains(targets)) targetsToRemove.Add(targets);
                }

                foreach(BuildTarget targets in targetsToRemove)
                {
                    this.m_targetsToBuild.Remove(targets);
                }
            }
        }

        private void OnGUI() 
        {
            GUILayout.Label("Muti-Platform Builds", EditorStyles.boldLabel);

            this.m_useCustomFileName = EditorGUILayout.Toggle("Custom File Name?", this.m_useCustomFileName);

            if(this.m_useCustomFileName)
            {
                GUILayout.Label("Key in your file name. (Default will automatically be settings name)", EditorStyles.boldLabel);
                this.m_fileName = GUILayout.TextField(this.m_fileName, 40);
            }

            this.m_useCustomFilePath = EditorGUILayout.Toggle("Custom File Path?", this.m_useCustomFilePath);

            if(this.m_useCustomFilePath)
            {
                GUILayout.Label("Key in your file path. (Default will automatically file path)", EditorStyles.boldLabel);
                this.m_customFilePath = GUILayout.TextField(this.m_customFilePath, 100);
            }


            GUILayout.Label("Kindly select the platform you wish to build.", EditorStyles.boldLabel);
            int numberEnabled = 0;
            foreach(BuildTarget target in this.m_availableTarget)
            {
                this.m_targetsToBuild[target] = EditorGUILayout.Toggle(target.ToString(), this.m_targetsToBuild[target]);

                if(this.m_targetsToBuild[target])numberEnabled++;
            }

            if(numberEnabled > 0)
            {
                string prompt = numberEnabled == 1 ? "Build Only 1 Platform." : $"Build {numberEnabled} Platforms.";
                if(GUILayout.Button(prompt))
                {
                    List<BuildTarget> selectedTargets = new List<BuildTarget>();
                    foreach(BuildTarget target in this.m_availableTarget)
                    {
                        if(this.m_targetsToBuild[target]) selectedTargets.Add(target);
                    }

                    EditorCoroutineUtility.StartCoroutine(PerformBuild(selectedTargets), this);
                }
            }
        }

        private IEnumerator PerformBuild(List<BuildTarget> targetsToBuild)
        {
            int buildAllProgressID = Progress.Start("Build All", "Building all selected platforms", Progress.Options.Sticky);
            
            Progress.ShowDetails();

            yield return new EditorWaitForSeconds(1.0f);

            BuildTarget oriTarget = EditorUserBuildSettings.activeBuildTarget;

            for(int targetIndex = 0; targetIndex < targetsToBuild.Count; ++targetIndex)
            {
                BuildTarget buildTar = targetsToBuild[targetIndex];

                Progress.Report(buildAllProgressID, targetIndex + 1, targetsToBuild.Count);
                
                int buildTaskProgressID = Progress.Start($"Build {buildTar.ToString()}", null, Progress.Options.Sticky, buildAllProgressID);
                
                yield return new EditorWaitForSeconds(1.0f);

                if(!BuildIndividualTarget(buildTar))
                {
                    Progress.Finish(buildTaskProgressID, Progress.Status.Failed);
                    Progress.Finish(buildAllProgressID, Progress.Status.Failed);

                    if(EditorUserBuildSettings.activeBuildTarget != oriTarget)
                    {
                        EditorUserBuildSettings.SwitchActiveBuildTargetAsync(this.GetTargetGroupForTarget(oriTarget), oriTarget);
                    }

                    yield break;
                }

                Progress.Finish(buildTaskProgressID, Progress.Status.Succeeded);
                yield return new EditorWaitForSeconds(1.0f);
            }

            Progress.Finish(buildAllProgressID, Progress.Status.Succeeded);

            if(EditorUserBuildSettings.activeBuildTarget != oriTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTargetAsync(this.GetTargetGroupForTarget(oriTarget),oriTarget);
            }
//
            yield return null;
        }

        private bool BuildIndividualTarget(BuildTarget tar)
        {
            BuildPlayerOptions options = new BuildPlayerOptions();

            List<string> sceneList = new List<string>();
            foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                sceneList.Add(scene.path);
            }

            options.scenes = sceneList.ToArray();
            options.target = tar;
            options.targetGroup = this.GetTargetGroupForTarget(tar);

            this.DebugInConsole($"Start building for {tar.ToString()}");

            string fileName = this.m_fileName == "" ? "Default" : this.m_fileName;

            if(fileName == "Default")
            {
                fileName = PlayerSettings.productName;
            }

            string filePath = this.m_customFilePath == "" ? "Default" : this.m_customFilePath;

            if(filePath == "Default")
            {
                filePath = "Builds";
            }

            switch(tar)
            {
                case BuildTarget.Android:
                    string apkName = fileName + ".apk";
                    options.locationPathName = System.IO.Path.Combine(filePath, tar.ToString(), apkName);
                    break;
                case BuildTarget.StandaloneWindows64:
                    options.locationPathName = System.IO.Path.Combine(filePath, tar.ToString(), fileName + ".exe");
                    break;
                case BuildTarget.StandaloneLinux64:
                    options.locationPathName = System.IO.Path.Combine(filePath, tar.ToString(), fileName + ".x86_64");
                    break;
                default:
                    options.locationPathName = System.IO.Path.Combine(filePath, tar.ToString(), fileName);
                    break;
            }

            if(BuildPipeline.BuildCanBeAppended(tar, options.locationPathName) == CanAppendBuild.Yes)
            {
                options.options = BuildOptions.AcceptExternalModificationsToPlayer;
            }
            else if(BuildPipeline.BuildCanBeAppended(tar, options.locationPathName) == CanAppendBuild.Unsupported)
            {
                this.DebugWarningInConsole("Unspported");
                options.options = BuildOptions.None;
            }
            else
            {
                options.options = BuildOptions.None;
            }

            BuildReport report = BuildPipeline.BuildPlayer(options);

            if(report.summary.result == BuildResult.Succeeded)
            {
                this.DebugInConsole($"Build Succeeded at {options.locationPathName}");
                this.DebugInConsole($"Build for {tar.ToString()} completed in {report.summary.totalTime.Seconds}.");
                return true;
            }

            this.DebugErrorInConsole($"Build for {tar.ToString()} failed!");
            return false;
        }

        private void DebugWarningInConsole(string text)
        {
            Debug.LogWarning($"[BUILD WARNING]: {text}");
        }

        private void DebugErrorInConsole(string text)
        {
            Debug.LogError($"[BUILD ERROR]: {text}");
        }

        private void DebugInConsole(string text)
        {
            Debug.Log($"[BUILD MESSAGE]: {text}");
        }
    }
}
#endif
