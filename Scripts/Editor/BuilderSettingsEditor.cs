using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Serbull.Builder
{
    public class BuilderSettingsEditor : EditorWindow
    {
        private static string _buildPath;
        private static bool _apkDebugBuild;
        private static bool _useKeystore;
        private static string _keystoreName;
        private static string _keyaliasName;
        private static string _keystorePassword;
        private static string _keyaliasPassword;

        [MenuItem("Builder/Settings...",false, 300)]
        private static void ShowWindow()
        {
            _buildPath = BuilderSettings.GetBuildPath();
            _apkDebugBuild = BuilderSettings.ApkDebugBuild;
            _useKeystore = BuilderSettings.UseKeystore;
            _keystoreName = PlayerSettings.Android.keystoreName;
            _keyaliasName = PlayerSettings.Android.keyaliasName;
            _keystorePassword = BuilderSettings.GetKeystorePassword();
            _keyaliasPassword = BuilderSettings.GetKeyaliasPassword();

            GetWindow(typeof(BuilderSettingsEditor), false, "Builder Settings");
        }

        private void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Build path", GUILayout.Width(100));
            var buildPath = EditorGUILayout.TextField(_buildPath, GUILayout.ExpandWidth(true));
            if (buildPath != _buildPath)
            {
                _buildPath = buildPath;
                BuilderSettings.SetBuildPath(buildPath);
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Select Folder", GUILayout.ExpandWidth(true)))
            {
                buildPath = EditorUtility.OpenFolderPanel("Select Folder", "", "");
                if (!string.IsNullOrEmpty(buildPath))
                {
                    _buildPath = buildPath;
                    BuilderSettings.SetBuildPath(buildPath);
                }
            }

            GUILayout.Space(25);
            GUILayout.BeginHorizontal();
            GUILayout.Label("APK debug build", GUILayout.Width(100));
            var apkDebugBuild = GUILayout.Toggle(_apkDebugBuild, "");
            if (apkDebugBuild != _apkDebugBuild)
            {
                _apkDebugBuild = apkDebugBuild;
                BuilderSettings.ApkDebugBuild = apkDebugBuild;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Use keystore", GUILayout.Width(100));
            var useKeystore = GUILayout.Toggle(_useKeystore, "");
            if (useKeystore != _useKeystore)
            {
                _useKeystore = useKeystore;
                BuilderSettings.UseKeystore = useKeystore;
            }
            GUILayout.EndHorizontal();
            if (_useKeystore)
            {
                DrawInputBlock("Keystore name", ref _keystoreName, false);
                DrawInputBlock("Keyalias name", ref _keyaliasName, false);
                if(string.IsNullOrEmpty(_keyaliasName))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(105);
                    var style = new GUIStyle();
                    style.normal.textColor = Color.red;
                    GUILayout.Label("Key is empty! Select key in Player Settings", style);
                    GUILayout.EndHorizontal();
                }
                DrawInputBlock("Keystore password", ref _keystorePassword, true, () => BuilderSettings.SetKeystorePassword(_keystorePassword));
                DrawInputBlock("Keyalias password", ref _keyaliasPassword, true, () => BuilderSettings.SetKeyaliasPassword(_keyaliasPassword));
            }
        }

        private static void DrawInputBlock(string name, ref string value, bool editable, Action onEdit = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(100));
            GUI.enabled = editable;
            var currentValue = GUILayout.TextField(value, GUILayout.ExpandWidth(true));
            GUI.enabled = true;
            if (editable && currentValue != value)
            {
                value = currentValue;
                onEdit?.Invoke();
            }
            GUILayout.EndHorizontal();
        }
    }

    
}