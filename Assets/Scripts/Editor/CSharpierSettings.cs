using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace kagekirin.csharpier
{
    public class CSharpierSettings : ScriptableObject
    {
#if UNITY_EDITOR_WIN
        private const string k_CSharpierExe = "dotnet-csharpier.exe";
#else
        private const string k_CSharpierExe = "dotnet-csharpier";
#endif // UNITY_EDITOR_WIN


        private const string k_SettingsPath = "Assets/Editor/User/CSharpierSettings.asset";
        private const string k_CSharpierIgnorePath = ".csharpierignore";

        private const string k_CSharpierDefaultContents =
            @"## .csharpierignore for {0}

## never format packages
Packages/
Library/
UserSettings/
";

        [SerializeField]
        public string m_CSharpierPath;

        public string m_CSharpierIgnoreContents = ""; // not serialized on purpose

        internal static CSharpierSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<CSharpierSettings>(k_SettingsPath);

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<CSharpierSettings>();
                settings.m_CSharpierPath = k_CSharpierExe;

                if (!File.Exists(k_SettingsPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(k_SettingsPath));
                    AssetDatabase.CreateAsset(settings, k_SettingsPath);
                }

                AssetDatabase.SaveAssets();
            }

            if (File.Exists(k_CSharpierIgnorePath))
            {
                settings.m_CSharpierIgnoreContents = File.ReadAllText(k_CSharpierIgnorePath);
            }
            else
            {
                settings.m_CSharpierIgnoreContents =
                    "click button to create default .csharpierignore";
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public static void CreateCSharpierIgnore()
        {
            if (!File.Exists(k_CSharpierIgnorePath))
            {
                File.WriteAllText(
                    k_CSharpierIgnorePath,
                    string.Format(k_CSharpierDefaultContents, PlayerSettings.productName)
                );
            }
        }

    }
} // namespace kagekirin.csharpier
