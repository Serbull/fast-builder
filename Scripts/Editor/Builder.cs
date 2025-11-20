using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Serbull.Builder
{
    public static class Builder
    {
        [MenuItem("Builder/☢ Build Android .apk", false, 200)]
        public static void BuildAndroidApk()
        {
            BuildAndroid(true);
        }

        [MenuItem("Builder/▶ Build Android .aab", false, 201)]
        public static void BuildAndroidAab()
        {
            BuildAndroid(false);
        }

        private static void BuildAndroid(bool apk)
        {
            CheckDefineSymbols(apk);

            var buildPath = BuilderSettings.GetBuildPath();
            if (string.IsNullOrEmpty(buildPath))
            {
                buildPath = EditorUtility.OpenFolderPanel("Select Folder", "", "");
                if (!string.IsNullOrEmpty(buildPath))
                {
                    BuilderSettings.SetBuildPath(buildPath);
                }
                else
                {
                    UnityEngine.Debug.LogError($"FAST BUILDER: build path is null");
                    return;
                }
            }

            if (!apk && BuilderSettings.AabBuildVersionUp)
            {
                PlayerSettings.Android.bundleVersionCode++;
            }

            var version = BuilderSettings.GetVersionWithBundle(PlayerSettings.Android.bundleVersionCode, BuilderSettings.AddBundleToVersion);
            PlayerSettings.bundleVersion = version;

            EditorUserBuildSettings.buildAppBundle = !apk;

            var fileName = $"{Application.productName}-{PlayerSettings.bundleVersion}.";
            fileName += apk ? "apk" : "aab";

            var useKey = BuilderSettings.UseKeystore;
            PlayerSettings.Android.useCustomKeystore = useKey;
            if (useKey)
            {
                PlayerSettings.Android.keystorePass = BuilderSettings.GetKeystorePassword();
                PlayerSettings.Android.keyaliasPass = BuilderSettings.GetKeyaliasPassword();
            }
            var fullPath = buildPath + "/" + fileName;
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GetEnabledScenePaths(),
                target = BuildTarget.Android,
                locationPathName = fullPath,
                options = apk && BuilderSettings.ApkDebugBuild ? BuildOptions.Development : BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                UnityEngine.Debug.Log($"FAST BUILDER: succesful build! {fileName}");

                if (!apk)
                {
                    if (!BuilderSettings.AddBundleToVersion)
                    {
                        var addFileName = $"-({PlayerSettings.Android.bundleVersionCode})";
                        var newFilePath = fullPath.Insert(fullPath.Length - 4, addFileName);
                        System.IO.File.Move(fullPath, newFilePath);
                    }
                }
                else if (BuilderSettings.ApkTimePrefix)
                {
                    var addFileName = $"-{DateTime.Now:HHmm}";
                    var newFilePath = fullPath.Insert(fullPath.Length - 4, addFileName);
                    System.IO.File.Move(fullPath, newFilePath);
                }

                OpenFolderWithPackage(buildPath);
            }
        }

        public static void OpenFolderWithPackage(string path)
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            string command = "open";
#else
            string command = "explorer";
#endif
            string argument = "\"" + path + "\"";

            ProcessStartInfo processInfo = new(command, argument);
            Process.Start(processInfo);
        }

        private static string[] GetEnabledScenePaths()
        {
            return EditorBuildSettings.scenes.Select(e => e.path).ToArray();
        }

        private static void CheckDefineSymbols(bool apk)
        {
            bool useCheats = apk && BuilderSettings.ApkCheatBuild;

#if UNITY_6000_0_OR_NEWER
            var defines = PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android);
#else
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);    
#endif

            var defineList = new List<string>(defines.Split(';'));
            bool existDefine = defines.Contains("GAME_CHEATS");

            if (useCheats && !existDefine)
            {
                defineList.Add("GAME_CHEATS");
            }
            else if (!useCheats && existDefine)
            {
                defineList.Remove("GAME_CHEATS");
            }

#if UNITY_6000_0_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android, defineList.ToArray());
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineList.ToArray());
#endif
        }
    }
}
