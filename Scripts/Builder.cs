#if UNITY_EDITOR
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
        [MenuItem("Builder/☢ Build Android .apk",false,200)]
        public static void BuildAndroidApk()
        {
            BuildAndroid(true);
        }

        [MenuItem("Builder/▶ Build Android .aab",false, 201)]
        public static void BuildAndroidAab()
        {
            BuildAndroid(false);
        }

        private static void BuildAndroid(bool apk)
        {
            var buildPath = BuilderSettings.GetBuildPath();
            if (string.IsNullOrEmpty(buildPath))
            {
                buildPath = EditorUtility.OpenFolderPanel("Select Folder", "", "");
                if (string.IsNullOrEmpty(buildPath))
                {
                    UnityEngine.Debug.LogError($"FAST BUILDER: build path is null");
                    return;
                }
            }
                
            UnityEngine.Debug.Log(buildPath);
            var fileName = Application.productName;
            int buildNumber = 0;

            EditorUserBuildSettings.buildAppBundle = !apk;

            if (apk)
            {
                buildNumber = PlayerPrefs.GetInt("fast-builder-apk-build-number", 1);
                fileName += $"-{buildNumber}.apk";
            }
            else
            {
                fileName += $"-{PlayerSettings.bundleVersion}({PlayerSettings.Android.bundleVersionCode}).aab";
            }

            var useKey = BuilderSettings.UseKeystore;
            PlayerSettings.Android.useCustomKeystore = useKey;
            if (useKey)
            {
                // PlayerSettings.Android.keystoreName = BuilderSettings.GetKeystorePassword();
                // PlayerSettings.Android.keyaliasName = BuilderSettings.GetKeyaliasPassword();
                PlayerSettings.keystorePass = BuilderSettings.GetKeystorePassword();
                PlayerSettings.keyaliasPass = BuilderSettings.GetKeyaliasPassword();
            }

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GetEnabledScenePaths(),
                target = BuildTarget.Android,
                locationPathName = buildPath + fileName,
                options = apk ? BuildOptions.Development : BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                UnityEngine.Debug.Log($"FAST BUILDER: succesful build! {fileName}");

                if (apk)
                {
                    PlayerPrefs.SetInt("fast-builder-apk-build-number", buildNumber + 1);
                }

                OpenFolderWithPackage(buildPath);
            }
        }

        private static void OpenFolderWithPackage(string path)
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            string command = "open";
#else
            string command = "explorer";
#endif
            string argument = "\"" + path + "\"";

            ProcessStartInfo processInfo = new ProcessStartInfo(command, argument);
            Process.Start(processInfo);
        }

        private static string[] GetEnabledScenePaths()
        {
            return EditorBuildSettings.scenes.Select(e => e.path).ToArray();
        }
    }
}
#endif