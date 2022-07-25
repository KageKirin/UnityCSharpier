using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
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

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload
        )
        {
            if (importedAssets.Length == 0)
                return;

            if (!importedAssets.Any(assetPath => assetPath.EndsWith(".cs")))
                return;

            var scriptFiles = importedAssets
                .Where(assetPath => assetPath.EndsWith(".cs"))
                .Select(assetPath => Path.Join(RootPath, assetPath))
                .Select(assetPath => $"\"{assetPath}\"")
                .Aggregate((a, b) => $"{a} {b}");

            if (String.IsNullOrWhiteSpace(scriptFiles))
                return;

            try
            {
                using (var process = new System.Diagnostics.Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.FileName = CSharpierSettings.instance.m_CSharpierPath;
                    process.StartInfo.WorkingDirectory = RootPath;
                    process.StartInfo.Arguments = scriptFiles;

                    process.Start();
                    process.WaitForExit();

                    var stdout = process.StandardOutput.ReadToEnd();
                    var stderr = process.StandardError.ReadToEnd();

                    if (!String.IsNullOrEmpty(stdout))
                        Debug.Log($"CSharpier: {stdout}");

                    if (!String.IsNullOrEmpty(stderr))
                        Debug.LogError($"CSharpier: {stderr}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"CSharpier: {e.Message}");
            }
        }
    }
} // kagekirin.csharpier
