using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace kagekirin.csharpier
{
    public class CSharpierSettings : ScriptableObject
    {
        private const string k_SettingsPath = "Assets/Editor/User/CSharpierSettings.asset";
        private const string k_CSharpierIgnorePath = ".csharpierignore";

        [SerializeField]
        public string m_CSharpierPath;

        public string m_CSharpierIgnoreContents = ""; // not serialized on purpose

        internal static CSharpierSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<CSharpierSettings>(k_SettingsPath);

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<CSharpierSettings>();
#if UNITY_EDITOR_WIN
                settings.m_CSharpierPath = @"dotnet-csharpier.exe";
#elif UNITY_EDITOR_OSX
                settings.m_CSharpierPath = @"$(HOME)/.dotnet/tools/dotnet-csharpier";
#elif UNITY_EDITOR_LINUX
                settings.m_CSharpierPath = @"$(HOME)/.dotnet/tools/dotnet-csharpier";
#else
                settings.m_CSharpierPath = @"dotnet-csharpier";
#endif

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
                settings.m_CSharpierIgnoreContents = "could not find .csharpierignore";
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
} // namespace kagekirin.csharpier
