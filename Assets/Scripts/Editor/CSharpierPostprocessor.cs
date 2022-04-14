using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace kagekirin.csharpier
{
    class CSharpierPostprocessor : AssetPostprocessor
    {
        static string RootPath
        {
            get => Path.GetDirectoryName(Application.dataPath);
        }

        void OnPreprocessAsset()
        {
            CSharpierSettings CSharpierSettings = CSharpierSettings.GetOrCreateSettings();

            if (assetPath.EndsWith(".cs"))
            {
                try
                {
                    using (var process = new System.Diagnostics.Process())
                    {
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.FileName = CSharpierSettings.m_CSharpierPath;
                        process.StartInfo.WorkingDirectory = RootPath;
                        process.StartInfo.Arguments = assetPath;

                        process.Start();
                        process.WaitForExit();
                        Debug.Log($"CSharpier: {process.StandardOutput.ReadToEnd()}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"CSharpier: {e.Message}");
                }
            }
        }
    }
} // kagekirin.csharpier
