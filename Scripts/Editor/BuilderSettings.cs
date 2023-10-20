using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Serbull.Builder
{
    public static class BuilderSettings
    {
        public static string GetBuildPath()
        {
            return PlayerPrefs.GetString("fast-builder-path");
        }

        public static void SetBuildPath(string path)
        {
            PlayerPrefs.SetString("fast-builder-path", path);
        }

        public static string GetKeystorePassword()
        {
            return PlayerPrefs.GetString("fast-builder-keystore-pass");
        }

        public static void SetKeystorePassword(string pass)
        {
            PlayerPrefs.SetString("fast-builder-keystore-pass", pass);
        }

        public static string GetKeyaliasPassword()
        {
            return PlayerPrefs.GetString("fast-builder-keyalias-pass");
        }

        public static void SetKeyaliasPassword(string pass)
        {
            PlayerPrefs.SetString("fast-builder-keyalias-pass", pass);
        }

        public static bool UseKeystore
        {
            get
            {
                return PlayerPrefs.GetInt("fast-builder-use-keystore") == 1;
            }
            set
            {
                PlayerPrefs.SetInt("fast-builder-use-keystore", value ? 1 : 0);
            }
        }
    }
}