using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace kagekirin.csharpier
{
    [FilePath("CSharpierSettings.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class CSharpierSettings : ScriptableSingleton<CSharpierSettings>
    {
#if UNITY_EDITOR_WIN
        private const string k_CSharpierExe = "dotnet-csharpier.exe";
#else
        private const string k_CSharpierExe = "dotnet-csharpier";
#endif // UNITY_EDITOR_WIN

        private const string k_CSharpierIgnorePath = ".csharpierignore";

        private const string k_CSharpierDefaultContents =
            @"## .csharpierignore for {0}

## never format packages
Packages/
Library/
UserSettings/
";

        private const string k_CSharpierIgnoreDoesNotExist =
            "click button to create default .csharpierignore";

        [SerializeField]
        [ContextMenuItem("Set from environment", "SetCSharpierPathFromEnv")]
        public string m_CSharpierPath;

        [SerializeField]
        [ContextMenuItem("Create .csharpierignore", "CreateDefaultCSharpierIgnore")]
        public string m_CSharpierIgnoreContents = "";

        public void OnValidate()
        {
            if (String.IsNullOrWhiteSpace(m_CSharpierIgnoreContents))
            {
                if (File.Exists(k_CSharpierIgnorePath))
                {
                    m_CSharpierIgnoreContents = File.ReadAllText(k_CSharpierIgnorePath);
                }
                else
                {
                    m_CSharpierIgnoreContents = k_CSharpierIgnoreDoesNotExist;
                }
            }
            else if (m_CSharpierIgnoreContents != k_CSharpierIgnoreDoesNotExist)
            {
                FlushCSharpierIgnore(m_CSharpierIgnoreContents);
            }
        }

        internal static SerializedObject GetSerializedSettings()
        {
            CSharpierSettings.instance.Save(true);
            return new SerializedObject(CSharpierSettings.instance);
        }

        private void SetCSharpierPathFromEnv()
        {
            m_CSharpierPath = LocateCSharpierTool();
        }

        private void CreateDefaultCSharpierIgnore()
        {
            CreateCSharpierIgnore();
            m_CSharpierIgnoreContents = File.ReadAllText(k_CSharpierIgnorePath);
        }

        private static void FlushCSharpierIgnore(string contents)
        {
            File.WriteAllText(k_CSharpierIgnorePath, contents);
        }

        public static void CreateCSharpierIgnore()
        {
            if (!File.Exists(k_CSharpierIgnorePath))
            {
                FlushCSharpierIgnore(
                    string.Format(k_CSharpierDefaultContents, PlayerSettings.productName)
                );
            }
        }

        public static string LocateCSharpierTool()
        {
            try
            {
                // look in $PATH
                var PATH =
                    Environment.GetEnvironmentVariable("PATH")
                    ?? Environment.GetEnvironmentVariable("Path");

                if (!String.IsNullOrEmpty(PATH))
                {
#if UNITY_EDITOR_WIN
                    var pathes = PATH.Split(";");
#else
                    var pathes = PATH.Split(":");
#endif // UNITY_EDITOR_WIN

                    foreach (var path in pathes)
                    {
                        var csharpierPath = Path.Join(path, k_CSharpierExe);
                        if (File.Exists(csharpierPath))
                        {
                            return csharpierPath;
                        }
                    }
                }

                // look in $HOME
                var HOME =
                    Environment.GetEnvironmentVariable("HOME")
                    ?? Environment.GetEnvironmentVariable("Home");

                if (!String.IsNullOrEmpty(HOME))
                {
                    var csharpierPath = Path.Join(
                        Path.Join(HOME, ".dotnet"),
                        "tools",
                        k_CSharpierExe
                    );
                    if (File.Exists(csharpierPath))
                    {
                        return csharpierPath;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"locating CSharpier: {e.Message}");
            }

            return k_CSharpierExe;
        }
    }
} // namespace kagekirin.csharpier
