using System.IO;
using UnityEngine;

namespace Serbull.Builder
{
    public static class BuilderSettings
    {
        private class SettingsData
        {
            public string BuildPath;
            public string KeystorePass;
            public string KeyaliasPass;
            public bool UseKeystore;
            public bool ApkDebugBuild = false;
            public bool ApkCheatBuild = true;
            public bool ApkTimePrefix = true;
            public bool AabBuildVersionUp = true;
        }

        private static SettingsData _data;

        private static readonly string _dataPath = Application.persistentDataPath + "/fast-builder.data";

        static BuilderSettings()
        {
            if (File.Exists(_dataPath))
            {
                var text = File.ReadAllText(_dataPath);
                _data = JsonUtility.FromJson<SettingsData>(text);
            }
            else
            {
                _data = new SettingsData();
                SaveSettingsData();
            }
        }

        private static void SaveSettingsData()
        {
            var text = JsonUtility.ToJson(_data);
            File.WriteAllText(_dataPath, text);
        }

        public static string GetBuildPath()
        {
            return _data.BuildPath;
        }

        public static void SetBuildPath(string path)
        {
            _data.BuildPath = path;
            SaveSettingsData();
        }

        public static string GetKeystorePassword()
        {
            return _data.KeystorePass;
        }

        public static void SetKeystorePassword(string pass)
        {
            _data.KeystorePass = pass;
            SaveSettingsData();
        }

        public static string GetKeyaliasPassword()
        {
            return _data.KeyaliasPass;
        }

        public static void SetKeyaliasPassword(string pass)
        {
            _data.KeyaliasPass = pass;
            SaveSettingsData();
        }

        public static bool UseKeystore
        {
            get
            {
                return _data.UseKeystore;
            }
            set
            {
                _data.UseKeystore = value;
                SaveSettingsData();
            }
        }

        public static bool ApkDebugBuild
        {
            get
            {
                return _data.ApkDebugBuild;
            }
            set
            {
                _data.ApkDebugBuild = value;
                SaveSettingsData();
            }
        }

        public static bool ApkCheatBuild
        {
            get
            {
                return _data.ApkCheatBuild;
            }
            set
            {
                _data.ApkCheatBuild = value;
                SaveSettingsData();
            }
        }

        public static bool ApkTimePrefix
        {
            get
            {
                return _data.ApkTimePrefix;
            }
            set
            {
                _data.ApkTimePrefix = value;
                SaveSettingsData();
            }
        }

        public static bool AabBuildVersionUp
        {
            get
            {
                return _data.AabBuildVersionUp;
            }
            set
            {
                _data.AabBuildVersionUp = value;
                SaveSettingsData();
            }
        }
    }
}