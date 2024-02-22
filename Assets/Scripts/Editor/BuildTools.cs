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
        [MenuItem("Tools/Build Tools")]
        public static void OnShowTools()
        {
            EditorWindow.GetWindow<BuildTools>();
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


            switch(tar)
            {
                case BuildTarget.Android:
                    string apkName = PlayerSettings.productName + ".apk";
                    options.locationPathName = System.IO.Path.Combine("Builds", tar.ToString(), apkName);
                    break;
                case BuildTarget.StandaloneWindows64:
                    options.locationPathName = System.IO.Path.Combine("Builds", tar.ToString(), PlayerSettings.productName + ".exe");
                    break;
                case BuildTarget.StandaloneLinux64:
                    options.locationPathName = System.IO.Path.Combine("Builds", tar.ToString(), PlayerSettings.productName + ".x86_64");
                    break;
                default:
                    options.locationPathName = System.IO.Path.Combine("Builds", tar.ToString(), PlayerSettings.productName);
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
