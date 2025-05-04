using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Unity.EditorCoroutines.Editor;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace BUILDTOOLS
{
    public class BuildTools : EditorWindow
    {
        private string m_fileName = "Default";
        private const string m_issScriptPath = "InstallerScripts/MyInstaller.iss";
        private bool m_createInstallerForWindows = true;
        private bool m_createInstallerForMacOS = true;


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

            GUILayout.Label("Key in your file name. (Default will automatically be settings name)", EditorStyles.boldLabel);
            this.m_fileName = GUILayout.TextField(this.m_fileName, 40);

            GUILayout.Label("Kindly select the platform you wish to build.", EditorStyles.boldLabel);
            int numberEnabled = 0;
            foreach(BuildTarget target in this.m_availableTarget)
            {
                this.m_targetsToBuild[target] = EditorGUILayout.Toggle(target.ToString(), this.m_targetsToBuild[target]);

                if(this.m_targetsToBuild[target])numberEnabled++;
            }

            bool windowsSelected = m_targetsToBuild.ContainsKey(BuildTarget.StandaloneWindows64) && m_targetsToBuild[BuildTarget.StandaloneWindows64];
            if (windowsSelected)
            {
                EditorGUILayout.Space();
                m_createInstallerForWindows = EditorGUILayout.Toggle("Create Window Installer", m_createInstallerForWindows);
            }

            bool macosSelected = m_targetsToBuild.ContainsKey(BuildTarget.StandaloneOSX) && m_targetsToBuild[BuildTarget.StandaloneOSX];
            if (macosSelected)
            {
                EditorGUILayout.Space();
                m_createInstallerForMacOS = EditorGUILayout.Toggle("Create DMG", m_createInstallerForMacOS);
            }

            if (numberEnabled > 0)
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

            string fileName = this.m_fileName;

            if(fileName == "Default")
            {
                fileName = PlayerSettings.productName;
            }

            switch(tar)
            {
                case BuildTarget.Android:
                    string apkName = fileName + ".apk";
                    options.locationPathName = System.IO.Path.Combine("Builds", tar.ToString(), apkName);
                    break;
                case BuildTarget.StandaloneWindows64:
                    options.locationPathName = System.IO.Path.Combine("Builds", tar.ToString(), fileName + ".exe");
                    break;
                case BuildTarget.StandaloneLinux64:
                    options.locationPathName = System.IO.Path.Combine("Builds", tar.ToString(), fileName + ".x86_64");
                    break;
                default:
                    options.locationPathName = System.IO.Path.Combine("Builds", tar.ToString(), fileName);
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

                if (tar == BuildTarget.StandaloneWindows64 && this.m_createInstallerForWindows)
                {
                    string version = PlayerSettings.bundleVersion;
                    string exeName = PlayerSettings.productName;
                    string buildFolder = Path.GetDirectoryName(options.locationPathName);
                    CreateInstaller(version, exeName, buildFolder);
                }
                else if(tar == BuildTarget.StandaloneOSX && this.m_createInstallerForMacOS)
                {
                    string appPath = options.locationPathName + ".app";
                    string dmgPath = Path.Combine("Builds", tar.ToString(), fileName + ".dmg");

                    CreateMacOSDmg(appPath, dmgPath);
                }

                return true;
            }

            this.DebugErrorInConsole($"Build for {tar.ToString()} failed!");
            return false;
        }

        private void CreateInstaller(string version, string exeName, string buildPath)
        {
            DebugInConsole($"Creating installer with version: {version}");
            string issTemplate = File.ReadAllText(m_issScriptPath);
            string issContent = issTemplate
                .Replace("{AppVersion}", version)
                .Replace("{SourceDir}", Path.GetFullPath(buildPath).Replace("\\", "\\\\"))
                .Replace("{ExeName}", exeName);

            string tempIssPath = Path.Combine(buildPath, "TempInstaller.iss");
            File.WriteAllText(tempIssPath, issContent);

            Process process = new Process();
            process.StartInfo.FileName = "ISCC.exe"; // Inno Setup Compiler must be in PATH
            process.StartInfo.Arguments = $"\"{tempIssPath}\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();

            File.Delete(tempIssPath);
            DebugInConsole($"Installer source path: {buildPath}");
            DebugInConsole($"Expected EXE: {exeName}.exe");
        }

        private void CreateMacOSDmg(string appPath, string dmgPath)
        {
            string volumeName = Path.GetFileNameWithoutExtension(appPath);
            string tempMount = "/Volumes/" + volumeName;

            string command = $"hdiutil create -volname \"{volumeName}\" -srcfolder \"{appPath}\" -ov -format UDZO \"{dmgPath}\"";

            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                DebugInConsole($"macOS .dmg created at: {dmgPath}");
            }
            else
            {
                DebugErrorInConsole($"Failed to create .dmg:\n{error}");
            }
        }

        private void DebugWarningInConsole(string text)
        {
            UnityEngine.Debug.LogWarning($"[BUILD WARNING]: {text}");
        }

        private void DebugErrorInConsole(string text)
        {
            UnityEngine.Debug.LogError($"[BUILD ERROR]: {text}");
        }

        private void DebugInConsole(string text)
        {
            UnityEngine.Debug.Log($"[BUILD MESSAGE]: {text}");
        }
    }
}