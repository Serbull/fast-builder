using UnityEngine;
using UnityEditor;
using System;

namespace Serbull.Builder
{
    public class BuilderSettingsEditor : EditorWindow
    {
        private static string _buildPath;
        private static bool _apkDebugBuild;
        private static bool _apkCheatBuild;
        private static bool _apkTimePrefix;
        private static bool _aabBuildVersionUp;
        private static bool _addBundleToVersion;
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
            _apkCheatBuild = BuilderSettings.ApkCheatBuild;
            _apkTimePrefix = BuilderSettings.ApkTimePrefix;
            _aabBuildVersionUp = BuilderSettings.AabBuildVersionUp;
            _addBundleToVersion = BuilderSettings.AddBundleToVersion;

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

            var version = PlayerSettings.bundleVersion;
            DrawInputBlock("Version", ref version, true, () => PlayerSettings.bundleVersion = version);

            var bundle = PlayerSettings.Android.bundleVersionCode.ToString();
            DrawInputBlock("Bundle version", ref bundle, true, () =>
            {
                if (int.TryParse(bundle, out int res))
                {
                    PlayerSettings.Android.bundleVersionCode = res;
                }
            });

            GUILayout.Space(10);

            DrawCheckmarkBlock("APK debug build", ref _apkDebugBuild, (x) => BuilderSettings.ApkDebugBuild = x, "define DEBUG");
            DrawCheckmarkBlock("APK cheat build", ref _apkCheatBuild, (x) => BuilderSettings.ApkCheatBuild = x, "define GAME_CHEATS");
            DrawCheckmarkBlock("APK time prefix", ref _apkTimePrefix, (x) => BuilderSettings.ApkTimePrefix = x);
            var versionCode = PlayerSettings.Android.bundleVersionCode + (BuilderSettings.AabBuildVersionUp ? 1 : 0);
            var aabBuildVersionUpExtra = PlayerSettings.Android.bundleVersionCode + (BuilderSettings.AabBuildVersionUp ? $" -> {versionCode}" : null);
            DrawCheckmarkBlock("AAB build version up", ref _aabBuildVersionUp, (x) => BuilderSettings.AabBuildVersionUp = x, $"Bundle version {aabBuildVersionUpExtra}");
            var versionExtra = BuilderSettings.GetVersionWithBundle(versionCode, BuilderSettings.AddBundleToVersion);
            DrawCheckmarkBlock("Add bundle to version", ref _addBundleToVersion, (x) => BuilderSettings.AddBundleToVersion = x, $"Version {versionExtra}");
            DrawCheckmarkBlock("Use keystore", ref _useKeystore, (x) => BuilderSettings.UseKeystore = x);

            if (_useKeystore)
            {
                DrawInputBlock("Keystore name", ref _keystoreName, false);
                DrawInputBlock("Keyalias name", ref _keyaliasName, false);
                if(string.IsNullOrEmpty(_keyaliasName))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(140);
                    var style = new GUIStyle();
                    style.normal.textColor = Color.red;
                    GUILayout.Label("Key is empty! Select key in Player Settings", style);
                    GUILayout.EndHorizontal();
                }
                DrawInputBlock("Keystore password", ref _keystorePassword, true, () => BuilderSettings.SetKeystorePassword(_keystorePassword));
                DrawInputBlock("Keyalias password", ref _keyaliasPassword, true, () => BuilderSettings.SetKeyaliasPassword(_keyaliasPassword));
            }
        }

        private static void DrawCheckmarkBlock(string name, ref bool value, Action<bool> onEdited, string extraText = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(130));
            var currentValue = GUILayout.Toggle(value, "");
            if (value != currentValue)
            {
                value = currentValue;
                onEdited.Invoke(value);
            }
            if (extraText != null)
            {
                var style = new GUIStyle();
                style.normal.textColor = Color.grey;
                GUILayout.Label(extraText, style);
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawInputBlock(string name, ref string value, bool editable, Action onEdit = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name, GUILayout.Width(130));
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